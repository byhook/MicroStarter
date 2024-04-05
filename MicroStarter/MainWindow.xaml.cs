using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
            }
        }

        return mainConfigData;
    }

    // 假设这是一个WPF应用程序，使用BitmapImage作为ImageSource
    public async Task<List<BitmapImage>> LoadLocalImageSourcesAsync(List<string> filePaths)
    {
        var tasks = filePaths.Select(filePath => LoadImageAsync(filePath)).ToList();
        var images = await Task.WhenAll(tasks);
        return images.ToList();
    }

    private async Task<BitmapImage> LoadImageAsync(string filePath)
    {
        using (var stream = await Task.Run(() => System.IO.File.OpenRead(filePath)))
        {
            BitmapImage bitmapImage = null;
            this.Dispatcher.Invoke(() => { bitmapImage = new BitmapImage(); });
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }
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
                tabListView.AllowDrop = true;

                _listViewConfigItemModel = new ListViewConfigItemModel();
                tabListView.DataContext = _listViewConfigItemModel;

                /*
                tabListView.DragEnter += new DragEventHandler(listView1_DragEnter);
                tabListView.DragOver += new DragEventHandler(listView1_DragOver);
                tabListView.DragLeave += new DragEventHandler(listView1_DragDrop);
                */

                //tabListView.ItemDrag += ListView1_ItemDrag;
                if (tabPageData.TabItemDataList != null)
                {
                    foreach (var tabItemData in tabPageData.TabItemDataList)
                    {
                        //if (tabItemData.ItemBitmap != null)
                        //{
                        // 创建BitmapImage对象

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

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        ConfigManager.GetInstance().SaveConfig();
    }
}