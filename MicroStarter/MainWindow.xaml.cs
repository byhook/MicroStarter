using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
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
                    if (File.Exists(tabItemData.ItemPath))
                    {
                        //比较耗时
                        var bitmap = IconManager.GetLargeIcon(tabItemData.ItemPath);
                        if (bitmap != null)
                        {
                            tabItemData.ItemBitmap = bitmap;
                        }
                    }
                }
            }
        }
        return mainConfigData;
    }

    private async Task LoadListConfigAsync()
    {
        // 执行异步操作，例如从网络或数据库加载数据
        var localConfigData = await LoadDataAsync();

        // 在 UI 线程上更新 ListView 的数据源
        this.Dispatcher.Invoke(() =>
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
                        if (tabItemData.ItemBitmap != null)
                        {
                            tabItemData.ItemIconSource = Imaging.CreateBitmapSourceFromHBitmap(
                                tabItemData.ItemBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                        }

                        _listViewConfigItemModel.ListViewItems.Add(tabItemData);
                    }

                    tabListView.ItemsSource = _listViewConfigItemModel.ListViewItems;
                }

                //添加到TabControl里去
                MainTabControl.Items.Add(newTabItem);
            }
            //设置默认的下标
            //MainTabControl.SelectedIndex = 0;
        });
    }

    private void SetupTabPages()
    {
        var mainConfigData = ConfigManager.GetInstance().LoadConfig();

        foreach (var tabPageData in mainConfigData.TabRootData)
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
                    if (File.Exists(tabItemData.ItemPath))
                    {
                        var bitmap = IconManager.GetLargeIcon(tabItemData.ItemPath);
                        if (bitmap != null)
                        {
                            //更新本地的图片资源
                            tabItemData.ItemIconSource = Imaging.CreateBitmapSourceFromHBitmap(
                                bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                        }
                    }

                    _listViewConfigItemModel.ListViewItems.Add(tabItemData);
                }

                tabListView.ItemsSource = _listViewConfigItemModel.ListViewItems;
            }

            //添加到TabControl里去
            MainTabControl.Items.Add(newTabItem);
        }

        MainTabControl.SelectedIndex = 0;
    }
}