using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinStarter
{
    public sealed class IconManager
    {

        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            public char szDisplayName;
            public char szTypeName;
        }

        private static readonly Lazy<IconManager> _lazy =
            new Lazy<IconManager>(() => new IconManager());

        private IconManager() { }

        public static IconManager GetInstance()
        {
            return _lazy.Value;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public Icon? getTargetIcon(String execPath)
        {
            /*
            const uint SHGFI_ICON = 0x000000100;     // Get icon
            const uint SHGFI_LARGEICON = 0x000000000;  // Get large icon
            const uint SHGFI_SMALLICON = 0x000000001;  // Get small icon   

            SHFILEINFO fi = new SHFILEINFO();
            Icon ic = null;
            //SHGFI_ICON + SHGFI_USEFILEATTRIBUTES + SmallIcon   
            int iTotal = (int)SHGetFileInfo(execPath, 0, ref fi, (uint)Marshal.SizeOf(fi),
                SHGFI_ICON | SHGFI_LARGEICON);
            if (iTotal > 0)
            {
                ic = Icon.FromHandle(fi.hIcon);
            }
            */
;
            Icon ic = Icon.ExtractAssociatedIcon(execPath);
            
            return ic;
        }

    }
}
