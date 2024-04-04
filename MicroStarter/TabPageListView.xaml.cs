using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MicroStarter;

public partial class TabPageListView : UserControl
{
    public TabPageListView()
    {
        InitializeComponent();
    }

    private void TabListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var tabPageListView = sender as ListView;
        var tabListItemData = tabPageListView?.SelectedValue as TabItemData;

        if (File.Exists(tabListItemData?.ItemPath))
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(tabListItemData.ItemPath, tabListItemData.ItemRunCommand);
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = Path.GetDirectoryName(tabListItemData.ItemPath);

            process.StartInfo = startInfo;
            process.Start();
        }
    }

    private void MenuItem_OnClickOpenPath(object sender, RoutedEventArgs e)
    {
    }

    private void MenuItem_OnClickCopyPath(object sender, RoutedEventArgs e)
    {
    }

    private void MenuItem_OnClickEdit(object sender, RoutedEventArgs e)
    {
        var editWindow = new EditWindow();
        editWindow.ShowDialog();
    }
}