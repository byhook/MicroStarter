using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MicroStarter.Config;

namespace MicroStarter;

public partial class TabPageListView : UserControl
{

    private readonly TabPageViewModel _tabPageViewModel;
    
    public TabPageListView(TabPageViewModel tabPageViewModel)
    {
        _tabPageViewModel = tabPageViewModel;
        InitializeComponent();
    }

    private void TabListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var tabPageListView = sender as ListView;
        var tabListItemData = tabPageListView?.SelectedValue as TabItemViewModel;

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
        var contextMenuItem = sender as MenuItem;
        var tabPageListView = contextMenuItem?.DataContext as ListView;
        var tabListItemData = tabPageListView?.SelectedValue as TabItemViewModel;

        if (File.Exists(tabListItemData?.ItemPath))
        {
            // 使用DirectoryInfo获取文件的父目录
            var directoryInfo = Directory.GetParent(tabListItemData.ItemPath);
            var startInfo = new ProcessStartInfo
            {
                Arguments = directoryInfo?.FullName,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
    }

    private void MenuItem_OnClickCopyPath(object sender, RoutedEventArgs e)
    {
        var contextMenuItem = sender as MenuItem;
        var tabPageListView = contextMenuItem?.DataContext as ListView;
        var tabListItemData = tabPageListView?.SelectedValue as TabItemViewModel;

        if (File.Exists(tabListItemData?.ItemPath))
        {
            // 将文件路径复制到剪贴板
            Clipboard.SetText(tabListItemData.ItemPath);
        }
    }

    private void MenuItem_OnClickEdit(object sender, RoutedEventArgs e)
    {
        var contextMenuItem = sender as MenuItem;
        var tabPageListView = contextMenuItem?.DataContext as ListView;
        var tabItemData = tabPageListView?.SelectedValue as TabItemViewModel;
        if (tabItemData == null) return;

        var editWindow = new EditWindow(tabItemData);
        editWindow.ShowDialog();
    }

    private void MenuItem_OnClickDelete(object sender, RoutedEventArgs e)
    {
        var contextMenuItem = sender as MenuItem;
        var tabPageListView = contextMenuItem?.DataContext as ListView;
        if (tabPageListView != null)
        {
            var selectedIndex = tabPageListView.SelectedIndex;
            _tabPageViewModel.RemoveItem(selectedIndex);
            //保存配置到本地
            ConfigManager.GetInstance().SaveConfig();
        }
    }

}