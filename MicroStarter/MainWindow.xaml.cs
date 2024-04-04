using System.Windows;
using System.Windows.Controls;

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
        SetupTabPages();
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