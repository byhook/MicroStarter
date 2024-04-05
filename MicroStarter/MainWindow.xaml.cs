using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
                

                // 设置拖放逻辑
                GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(tabListView, new FileDropHandler(tabListView));
                
                /*
                tabItemView.DragEnter += MyListView_DragEnter;
                tabItemView.DragOver += MyListView_DragOver;
                tabItemView.Drop += listView1_DragDrop;
                */

                _listViewConfigItemModel = new ListViewConfigItemModel();
                tabListView.DataContext = _listViewConfigItemModel;

                
                // 设置拖拽事件
                //DragDrop.AddDropHandler(tabListView, listView1_DragDrop);
                /*
                tabListView.DragEnter += ListView_DragEnter;
                tabListView.DragOver += ListView_DragOver;
                tabListView.Drop += ListView_Drop;
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
    
    private void MyListView_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Move;
        }
    }

    public class FileDropHandler : IDropTarget
    {
        private readonly ListView _listBox;

        private DefaultDropHandler defaultDropHandler;
        
        public FileDropHandler(ListView listBox)
        {
            defaultDropHandler = new DefaultDropHandler();
            _listBox = listBox;
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }
 
        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject dataObject && dataObject.GetDataPresent(DataFormats.StringFormat))
            {
                // 从拖拽的数据对象中获取数据
                string draggedItem = (string)dataObject.GetData(DataFormats.StringFormat);

                // 将拖拽的项添加到 ListView 的数据源中
                //ItemsSource.Add(draggedItem);

                // 通知库已处理拖拽
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.NotHandled = false;
            }
            else
            {
                defaultDropHandler.Drop(dropInfo);
            }
        }
    }
    
    private void MyListView_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Move;
        }
    }
    
    private void listView1_DragDrop(object sender, DragEventArgs e)
    {
        var tabListView = sender as ListView;

        string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (dropFiles != null && dropFiles.Length > 0)
        {
            // 对拖放的文件进行处理
            foreach (string filePath in dropFiles)
            {
                /*
                var tabItemData = new TabItemData();
                if (Path.GetExtension(filePath) == ".lnk")
                {
                    dynamic objWshShell = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WSH_SHELL));
                    var objShortcut = objWshShell.CreateShortcut(filePath);
                    tabItemData.ItemPath = objShortcut.TargetPath;
                    string fileName = Path.GetFileName(objShortcut.TargetPath);
                    tabItemData.ItemName = fileName;
                }
                else
                {
                    string fileName = Path.GetFileName(filePath);
                    tabItemData.ItemName = fileName;
                    tabItemData.ItemPath = filePath;
                }

                if (ConfigManager.GetInstance().AddTabItemData(mainTabControl.SelectedIndex, tabItemData))
                {
                    //添加到列表里
                    addTabListItem(tabItemData);
                }*/

            }
            ConfigManager.GetInstance().SaveConfig();

        }
    }
    
    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        ConfigManager.GetInstance().SaveConfig();
    }
}