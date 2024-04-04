using System.Windows.Media;

namespace MicroStarter;

using System;
using System.Drawing;
using System.Runtime.InteropServices;

public sealed class IconManager
{
    //详情: https://msdn.microsoft.com/en-us/library/windows/desktop/ms648075(v=vs.85).aspx
    [DllImport("User32.dll")]
    private static extern int PrivateExtractIcons(
        string lpszFile, //文件名可以是exe,dll,ico,cur,ani,bmp
        int nIconIndex, //从第几个图标开始获取
        int cxIcon, //获取图标的尺寸x
        int cyIcon, //获取图标的尺寸y
        IntPtr[] phicon, //获取到的图标指针数组
        int[] piconid, //图标对应的资源编号
        int nIcons, //指定获取的图标数量，仅当文件类型为.exe 和 .dll时候可用
        int flags //标志，默认0就可以，具体可以看LoadImage函数
    );

    //详情:https://msdn.microsoft.com/en-us/library/windows/desktop/ms648063(v=vs.85).aspx
    [DllImport("User32.dll")]
    private static extern bool DestroyIcon(
        IntPtr hIcon
    );

    public static Bitmap? GetLargeIcon(String targetFile, int targetSize = 128)
    {
        //选中文件中的图标总数
        var iconTotalCount = PrivateExtractIcons(targetFile, 0, 0, 0, null, null, 0, 0);

        //用于接收获取到的图标指针
        IntPtr[] hIcons = new IntPtr[iconTotalCount];
        //对应的图标id
        int[] ids = new int[iconTotalCount];
        //成功获取到的图标个数
        var successCount = PrivateExtractIcons(targetFile, 0, targetSize, targetSize, hIcons, ids, iconTotalCount, 0);

        //遍历并保存图标
        Bitmap? targetBitmap = null;
        for (var i = (successCount - 1); i >= 0; i--)
        {
            //指针为空，跳过
            if (hIcons[i] == IntPtr.Zero) continue;
            using (var ico = Icon.FromHandle(hIcons[i]))
            {
                targetBitmap = ico.ToBitmap();
            }

            //内存回收
            DestroyIcon(hIcons[i]);
        }

        if (targetBitmap == null)
        {
            return GetSmallIcon(targetFile);
        }

        return targetBitmap;
    }

    //小图标 32 * 32
    public static Bitmap? GetSmallIcon(string targetFile)
    {
        Bitmap? targetBitmap = null;
        var icon = Icon.ExtractAssociatedIcon(targetFile);
        if (icon != null)
        {
            targetBitmap = icon.ToBitmap();
        }

        return targetBitmap;
    }
}