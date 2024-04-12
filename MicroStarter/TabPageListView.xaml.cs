using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MicroStarter.Config;
using Clipboard = System.Windows.Clipboard;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

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

        if (File.Exists(tabListItemData?.ItemPath) || Directory.Exists(tabListItemData?.ItemPath))
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo(tabListItemData.ItemPath,
                tabListItemData.ItemRunCommand ?? string.Empty
            )
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(tabListItemData.ItemPath)
            };
            process.StartInfo = startInfo;
            process.Start();
        }
        else
        {
            var result = MessageBox.Show(
                "当前文件不存在，是否需要移除？",
                "错误",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _tabPageViewModel.RemoveItem(tabListItemData);
            }
        }
    }

    private void MenuItem_OnClickOpenPath(object sender, RoutedEventArgs e)
    {
        var contextMenuItem = sender as MenuItem;
        var tabPageListView = contextMenuItem?.DataContext as ListView;
        var tabListItemData = tabPageListView?.SelectedValue as TabItemViewModel;

        if (Directory.Exists(tabListItemData?.ItemPath))
        {
            // 使用DirectoryInfo获取文件的父目录
            var startInfo = new ProcessStartInfo
            {
                Arguments = tabListItemData.ItemPath,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
        else if (File.Exists(tabListItemData?.ItemPath))
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