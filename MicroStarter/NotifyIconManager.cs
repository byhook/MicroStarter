using System.Drawing;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace MicroStarter;

using System.Windows;

public class NotifyIconManager
{
    private static TaskbarIcon? _taskbarIcon = null;

    public static void LoadIconToSystemTray()
    {
        if (_taskbarIcon == null)
        {
            var targetIcon = Icon.ExtractAssociatedIcon(Application.ResourceAssembly.Location);
            _taskbarIcon = new TaskbarIcon();
            _taskbarIcon.Icon = targetIcon;
            _taskbarIcon.ToolTipText = "微启动器";
            _taskbarIcon.ContextMenu = new ContextMenu();

            var showMainMenuItem = new MenuItem
            {
                Header = "显示主界面"
            };
            showMainMenuItem.Click += MenuItem_OnShowMain_Click;
            _taskbarIcon.ContextMenu.Items.Add(showMainMenuItem);

            _taskbarIcon.ContextMenu.Items.Add(new Separator());

            var exitMenuItem = new MenuItem
            {
                Header = "退出"
            };
            exitMenuItem.Click += MenuItem_OnExit_Click;

            _taskbarIcon.TrayMouseDoubleClick += MenuItem_OnShowMain_DoubleClick;

            _taskbarIcon.ContextMenu.Items.Add(exitMenuItem);
        }
        else
        {
            _taskbarIcon.Visibility = Visibility.Visible;
        }
    }


    private static void MenuItem_OnShowMain_DoubleClick(object sender, RoutedEventArgs e)
    {
        MenuItem_OnShowMain_Click(sender, e);
    }

    private static void MenuItem_OnShowMain_Click(object sender, RoutedEventArgs e)
    {
        if (_taskbarIcon != null)
        {
            _taskbarIcon.Visibility = Visibility.Hidden;
        }
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.ShowInTaskbar = true;
            Application.Current.MainWindow.Visibility = Visibility.Visible;
        }
    }

    private static void MenuItem_OnExit_Click(object sender, RoutedEventArgs e)
    {
        _taskbarIcon?.Dispose();
        Application.Current.Shutdown();
    }

    public static void FreeIconToSystemTray()
    {
    }
}