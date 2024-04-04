using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

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
        SetupTabPages();
    }

    private void SetupTabPages()
    {
        var mainConfigData = ConfigManager.GetInstance().LoadConfig();
        Console.WriteLine(mainConfigData);

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

            /*
            tabListView.DragEnter += new DragEventHandler(listView1_DragEnter);
            tabListView.DragOver += new DragEventHandler(listView1_DragOver);
            tabListView.DragDrop += new DragEventHandler(listView1_DragDrop);

            tabListView.ItemDrag += ListView1_ItemDrag;
            */
            if (!(tabPageData.TabItemDataList is null))
            {
                /*
                foreach (var tabItemData in tabPageData.TabItemDatas)
                {
                    var item = new ListViewItem();
                    item.Name = tabItemData.ItemName;
                    item.Tag = tabItemData;
                    tabListView.Items.Add(item);
                }*/
                tabListView.ItemsSource = tabPageData.TabItemDataList;
            }

            MainTabControl.Items.Add(newTabItem);
        }

        MainTabControl.SelectedIndex = 0;
    }

}