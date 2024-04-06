using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MicroStarter.Config;

public class TabPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string? TabName { get; set; }

    public ObservableCollection<TabItemViewModel>? TabItemDataList { get; set; }

    public TabPageViewModel()
    {
        TabItemDataList = new ObservableCollection<TabItemViewModel>();
        TabItemDataList.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TabItemDataList));
    }

    public void RemoveItem(TabItemViewModel item)
    {
        if (TabItemDataList != null && TabItemDataList.Contains(item))
        {
            TabItemDataList.Remove(item);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}