using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace MicroStarter.Config;

public class TabItemViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string? _itemName = null;

    public string? ItemName
    {
        get => _itemName;
        set
        {
            if (_itemName == value) return;
            _itemName = value;
            OnPropertyChanged(nameof(ItemName));
        }
    }

    [JsonIgnore] public ImageSource? ItemIconSource { get; set; }

    public string? ItemPath { get; set; }

    public string? ItemIconPath { get; set; }

    public string? ItemRunCommand { get; set; }

    public bool RunWithAdmin { get; set; } = false;
}