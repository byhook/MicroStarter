using System.Drawing;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MicroStarter;

public class ConfigData
{
    public List<TabData>? TabRootData { get; set; }
}

public class TabData
{
    public string? TabName { get; set; }

    public List<TabItemData>? TabItemDataList { get; set; }
}

public class TabItemData
{
    public string? ItemName { get; set; }

    [JsonIgnore]
    public ImageSource? ItemIconSource
    {
        get
        {
            if (ItemPath != null)
            {
                var bitmap = IconManager.GetLargeIcon(ItemPath);
                if (bitmap != null) 
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
            return null;
        }
    }
    
    public string? ItemPath { get; set; }
    

    public string? ItemIconPath { get; set; }

    public string? ItemRunCommand { get; set; }

    public bool RunWithAdmin { get; set; }
    
}