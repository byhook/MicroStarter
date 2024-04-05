using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MicroStarter;

public class ConfigItem
{
    public List<TabListData>? TabRootData { get; set; }
}

public class TabListData
{
    public string? TabName { get; set; }

    public List<TabListItemData>? TabItemDataList { get; set; }
}

public class TabListItemData : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string? _itemName = null;

    public string? ItemName
    {
        get { return _itemName; }
        set
        {
            if (_itemName == value) return;
            _itemName = value;
            OnPropertyChanged(nameof(ItemName));
        }
    }

    [JsonIgnore] public Bitmap? ItemBitmap { get; set; }
    
    [JsonIgnore] public ImageSource? ItemIconSource { get; set; }

    public string? ItemPath { get; set; }

    public string? ItemIconPath { get; set; }

    public string? ItemRunCommand { get; set; }

    public bool RunWithAdmin { get; set; } = false;
}