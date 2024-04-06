using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MicroStarter.Config;

namespace MicroStarter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
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
    
    private async Task<TabRootViewModel> LoadDataAsync()
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

    public static void SetupTargetIconWithData(TabItemViewModel tabItemViewModel)
    {
        if (!string.IsNullOrEmpty(tabItemViewModel.ItemPath))
        {
            var iconBitmap = IconManager.GetLargeIcon(tabItemViewModel.ItemPath);
            var currentDirectory = Directory.GetCurrentDirectory();
            var imagesDir = Path.Combine(currentDirectory, "icons");
            // 如果目录不存在，则创建
            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);
            }

            tabItemViewModel.ItemIconPath = Path.Combine(imagesDir, tabItemViewModel.ItemName + ".ico");
            iconBitmap?.Save(tabItemViewModel.ItemIconPath, ImageFormat.Icon);
            iconBitmap?.Dispose();
        }
    }

    public static void SetupTargetIconSource(TabItemViewModel tabItemViewModel)
    {
        if (!string.IsNullOrEmpty(tabItemViewModel.ItemIconPath))
        {
            BitmapImage bitmapImage = new BitmapImage();

            // 创建Uri对象指向图片路径
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(tabItemViewModel.ItemIconPath);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            tabItemViewModel.ItemIconSource = bitmapImage;
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
                
                var tabItemView = new TabPageListView(tabPageData);
                var tabListView = tabItemView.TabListView;
                newTabItem.Content = tabListView;

                tabListView.DataContext = tabPageData.TabItemDataList;

                // 设置拖放逻辑
                GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(tabListView,
                    new FileDropHandler(MainTabControl)
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
                    }

                    tabListView.ItemsSource = tabPageData.TabItemDataList;
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