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
        var newTabItem = new TabItem
        {
            Header = "常用工具"
        };
        var textBlock = new TextBlock
        {
            Text = "动态内容"
        };
        newTabItem.Content = textBlock;
        MainTabControl.Items.Add(newTabItem);
        MainTabControl.SelectedIndex = 0;
    }
}