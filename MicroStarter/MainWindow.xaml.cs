using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    
    public class DataItem
    {
        public string Icon { get; set; }
        public string Name { get; set; }
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

            var image = new Image();
            
            if (!(tabPageData.TabItemDataList is null))
            {
                // 创建数据集合
                ObservableCollection<DataItem> items = new ObservableCollection<DataItem>
                {
                    new DataItem { Name = "John Doe", Icon = "WinLogo.png" },
                    new DataItem { Name = "哈哈哈哈", Icon = "WinLogo.png" },
                };
                
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