using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using GongSolutions.Wpf.DragDrop;

namespace MicroStarter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ListViewConfigItemModel _listViewConfigItemModel;

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

    private static void SetupTargetIconWithData(TabListItemData tabItemData)
    {
        //比较耗时
        var iconBitmap = IconManager.GetLargeIcon(tabItemData.ItemPath);
        string currentDirectory = Directory.GetCurrentDirectory();
        string imagesDir = Path.Combine(currentDirectory, "icons");
        // 如果目录不存在，则创建
        if (!Directory.Exists(imagesDir))
        {
            Directory.CreateDirectory(imagesDir);
        }

        tabItemData.ItemIconPath = Path.Combine(imagesDir, tabItemData.ItemName + ".ico");
        iconBitmap?.Save(tabItemData.ItemIconPath, ImageFormat.Icon);
        iconBitmap?.Dispose();
    }

    private static void SetupTargetIconSource(TabListItemData tabItemData)
    {
        BitmapImage bitmapImage = new BitmapImage();

        // 创建Uri对象指向图片路径
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(tabItemData.ItemIconPath);
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        tabItemData.ItemIconSource = bitmapImage;
    }

    private async Task LoadListConfigAsync()
    {
        // 执行异步操作，例如从网络或数据库加载数据
        var localConfigData = await LoadDataAsync();

        // 在 UI 线程上更新 ListView 的数据源
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            //SetupTabPages();
            // 清空现有数据
            //ListViewItems.Clear();

            foreach (var tabPageData in localConfigData.TabRootData!)
            {
                var newTabItem = new TabItem
                {
                    Header = tabPageData.TabName
                };
                var tabItemView = new TabPageListView();
                var tabListView = tabItemView.TabListView;
                newTabItem.Content = tabListView;


                /*
                tabItemView.DragEnter += MyListView_DragEnter;
                tabItemView.DragOver += MyListView_DragOver;
                tabItemView.Drop += listView1_DragDrop;
                */

                _listViewConfigItemModel = new ListViewConfigItemModel();
                tabListView.DataContext = _listViewConfigItemModel;

                // 设置拖放逻辑
                GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(tabListView,
                    new FileDropHandler(MainTabControl, _listViewConfigItemModel)
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

                        /*
                        tabItemData.ItemIconSource = Imaging.CreateBitmapSourceFromHBitmap(
                            tabItemData.ItemBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                            */
                        //}

                        _listViewConfigItemModel.ListViewItems.Add(tabItemData);
                    }

                    tabListView.ItemsSource = _listViewConfigItemModel.ListViewItems;
                }

                //添加到TabControl里去
                MainTabControl.Items.Add(newTabItem);
            }
        });
    }

    public class FileDropHandler(
        TabControl mainTabControl,
        ListViewConfigItemModel listViewConfigItemModel)
        : IDropTarget
    {
        private readonly DefaultDropHandler _defaultDropHandler = new();

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }

        private static readonly Guid ClsidWshShell = new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8");

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject dataObject && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var dropFiles = dataObject.GetData(DataFormats.FileDrop) as string[];
                if (dropFiles != null && dropFiles.Length > 0)
                {
                    // 对拖放的文件进行处理
                    foreach (var filePath in dropFiles)
                    {
                        var tabItemData = new TabListItemData();
                        if (Path.GetExtension(filePath) == ".lnk")
                        {
                            dynamic objWshShell = Activator.CreateInstance(Type.GetTypeFromCLSID(ClsidWshShell));
                            var objShortcut = objWshShell?.CreateShortcut(filePath);
                            tabItemData.ItemPath = objShortcut?.TargetPath;
                            string fileName = Path.GetFileName(objShortcut?.TargetPath);
                            tabItemData.ItemName = fileName;
                        }
                        else
                        {
                            var fileName = Path.GetFileName(filePath);
                            tabItemData.ItemName = fileName;
                            tabItemData.ItemPath = filePath;
                        }

                        if (ConfigManager.GetInstance().AddTabItemData(mainTabControl.SelectedIndex, tabItemData))
                        {
                            //添加到列表里
                            SetupTargetIconWithData(tabItemData);
                            SetupTargetIconSource(tabItemData);
                            listViewConfigItemModel.ListViewItems.Add(tabItemData);
                        }
                    }

                    ConfigManager.GetInstance().SaveConfig();
                }
            }
            else
            {
                _defaultDropHandler.Drop(dropInfo);
            }
        }
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        ConfigManager.GetInstance().SaveConfig();
    }
}