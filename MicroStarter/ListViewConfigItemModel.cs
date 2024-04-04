using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

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
    
    private async Task<List<TabListItemData>> LoadDataAsync()
    {
        // 模拟异步数据加载
        await Task.Delay(1000); // 等待1秒
        return new List<TabListItemData>
        {
            new TabListItemData { ItemName = "Item1" },
            new TabListItemData { ItemName = "Item2" },
            // ... 添加更多项
        };
    }
    
    public async Task RefreshListViewAsync()
    {
        // 执行异步操作，例如从网络或数据库加载数据
        var newData = await LoadDataAsync();
        // 在 UI 线程上更新 ListView 的数据源
        Application.Current.Dispatcher.Invoke(() =>
        {
            // 清空现有数据
            ListViewItems.Clear();

            // 添加新数据
            foreach (var item in newData)
            {
                ListViewItems.Add(item);
            }
        });
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