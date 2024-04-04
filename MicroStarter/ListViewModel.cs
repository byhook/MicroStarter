using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MicroStarter;

public class ListViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<TabItemData> ListViewItems { get; set; }

    public ListViewModel()
    {
        ListViewItems = new ObservableCollection<TabItemData>();
        ListViewItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListViewItems));
    }

    public void RemoveItem(TabItemData item)
    {
        if (ListViewItems.Contains(item))
        {
            ListViewItems.Remove(item);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}