using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MicroStarter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ConfigItemModel _configItemModel;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        Task.Run(() => { LoadListConfigAsync(); });
        //底部状态栏
        SetupImageStart();
    }

    private void SetupImageStart()
    {
        var targetIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ResourceAssembly.Location);
        if (targetIcon != null)
        {
            StatusStartImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                targetIcon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
    
    private async Task<ConfigItem> LoadDataAsync()
    {
        // 异步数据加载
        var mainConfigData = ConfigManager.GetInstance().LoadConfig();

        foreach (var tabPageData in mainConfigData.TabRootData!)
        {
            if (tabPageData.TabItemDataList != null)
            {
                foreach (var tabItemData in tabPageData.TabItemDataList)
                {
                    if (!File.Exists(tabItemData.ItemPath)) continue;
                    if (!string.IsNullOrEmpty(tabItemData.ItemIconPath) &&
                        File.Exists(tabItemData.ItemIconPath))
                    {
                        continue;
                    }

                    //比较耗时
                    SetupTargetIconWithData(tabItemData);
                }
            }
        }

        return mainConfigData;
    }

    public static void SetupTargetIconWithData(TabListItemData tabItemData)
    {
        if (!string.IsNullOrEmpty(tabItemData.ItemPath))
        {
            var iconBitmap = IconManager.GetLargeIcon(tabItemData.ItemPath);
            var currentDirectory = Directory.GetCurrentDirectory();
            var imagesDir = Path.Combine(currentDirectory, "icons");
            // 如果目录不存在，则创建
            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);
            }

            tabItemData.ItemIconPath = Path.Combine(imagesDir, tabItemData.ItemName + ".ico");
            iconBitmap?.Save(tabItemData.ItemIconPath, ImageFormat.Icon);
            iconBitmap?.Dispose();
        }
    }

    public static void SetupTargetIconSource(TabListItemData tabItemData)
    {
        if (!string.IsNullOrEmpty(tabItemData.ItemIconPath))
        {
            BitmapImage bitmapImage = new BitmapImage();

            // 创建Uri对象指向图片路径
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(tabItemData.ItemIconPath);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            tabItemData.ItemIconSource = bitmapImage;
        }
    }

    private async Task LoadListConfigAsync()
    {
        // 异步操作
        var localConfigData = await LoadDataAsync();
        // 在 UI 线程上更新 ListView 的数据源
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            foreach (var tabPageData in localConfigData.TabRootData!)
            {
                var newTabItem = new TabItem
                {
                    Header = tabPageData.TabName
                };
                var tabItemView = new TabPageListView();
                var tabListView = tabItemView.TabListView;
                newTabItem.Content = tabListView;

                _configItemModel = new ConfigItemModel();
                tabListView.DataContext = _configItemModel;

                // 设置拖放逻辑
                GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(tabListView,
                    new FileDropHandler(MainTabControl, _configItemModel)
                );


                if (tabPageData.TabItemDataList != null)
                {
                    foreach (var tabItemData in tabPageData.TabItemDataList)
                    {
                    
                        BitmapImage bitmapImage = new BitmapImage();

                        // 创建Uri对象指向图片路径
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(tabItemData.ItemIconPath);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        tabItemData.ItemIconSource = bitmapImage;

                        _configItemModel.ListViewItems.Add(tabItemData);
                    }

                    tabListView.ItemsSource = _configItemModel.ListViewItems;
                }

                //添加到TabControl里去
                MainTabControl.Items.Add(newTabItem);
            }
        });
    }
    
    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        
        ConfigManager.GetInstance().SaveConfig();
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
        ShowInTaskbar = false;//取消窗体在任务栏的显示
        NotifyIconManager.LoadIconToSystemTray();
    }
}