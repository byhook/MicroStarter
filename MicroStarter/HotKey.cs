using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MicroStarter;

public static class HotKey
{
    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_ALT = 0x1;
    private const uint MOD_CONTROL = 0x2;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, uint id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, uint id);
    
    private const int HOTKEY_ID = 6300;
    
    private static IntPtr _windowHandle;
    private static HwndSource? _source;
    public static void RegisterHotKey(Window window)
    {
        _windowHandle = new WindowInteropHelper(window).Handle;
        _source = HwndSource.FromHwnd(_windowHandle);
        _source?.AddHook(OnHotKeyHook);
        RegisterHotKey(_windowHandle, HOTKEY_ID, 0, 0x73);
    }
    
    private static IntPtr OnHotKeyHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_HOTKEY:
                switch (wParam.ToInt32())
                {
                    case HOTKEY_ID:
                        int vkey = (((int)lParam >> 16) & 0xFFFF);
                        if (vkey == 0x73)
                        {
                            Application.Current.MainWindow.ShowInTaskbar = true;
                            Application.Current.MainWindow.Visibility = Visibility.Visible;
                        }
                        handled = true;
                        break;
                }
                break;
        }
        return IntPtr.Zero;
    }

    public static void UnRegisterHotKey()
    {
        _source?.RemoveHook(OnHotKeyHook);
        UnregisterHotKey(_windowHandle, HOTKEY_ID);
    }
    
}