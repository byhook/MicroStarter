using System.Drawing;

namespace MicroStarter;

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

public class NotifyIconManager
{

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern bool NotifyIcon(int dwMessage, ref NOTIFYICONDATA lpData);

    [StructLayout(LayoutKind.Sequential)]
    public struct NOTIFYICONDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uID;
        public int uFlags;
        public int uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
    }

    private const int NIM_ADD = 0x00000000;
    private const int NIM_MODIFY = 0x00000001;
    private const int NIM_DELETE = 0x00000002;
    private const int NIF_ICON = 0x00000002;
    private const int NIF_MESSAGE = 0x00000010;
    private const int NIF_TIP = 0x00000004;

    public static void AddIconToSystemTray(Window window, string iconText)
    {
        NOTIFYICONDATA notifyIconData = new NOTIFYICONDATA
        {
            cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA)),
            hWnd = new WindowInteropHelper(window).Handle,
            uID = 1,
            uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP,
            uCallbackMessage = 0x00000040, // WM_USER + 0x40
            szTip = iconText
        };

        // 获取应用程序图标
        var icon = Icon.ExtractAssociatedIcon(Application.ResourceAssembly.Location);
        notifyIconData.hIcon = icon.Handle;

        // 添加托盘图标
        NotifyIcon(NIM_ADD, ref notifyIconData);
        
        /*
        System.Windows.Interop.HwndSource source =
            PresentationSource.FromVisual(window) as System.Windows.Interop.HwndSource;
        if (source != null)
        {
            IntPtr icon = System.Drawing.Icon
                .ExtractAssociatedIcon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MicroStarter.ico"))
                .ToBitmap().GetHicon();
            notifyIconData.hIcon = icon;

            NotifyIcon(NIM_ADD, ref notifyIconData);
            DestroyIcon(icon);
        }
        */
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(IntPtr hIcon);
}