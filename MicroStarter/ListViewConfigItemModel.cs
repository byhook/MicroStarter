using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MicroStarter;

public class ListViewConfigItemModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<TabListItemData> ListViewItems { get; set; }

    public ListViewConfigItemModel()
    {
        ListViewItems = new ObservableCollection<TabListItemData>();
        ListViewItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ListViewItems));
    }

    public void RemoveItem(TabListItemData listItem)
    {
        if (ListViewItems.Contains(listItem))
        {
            ListViewItems.Remove(listItem);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}