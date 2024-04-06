using System.Collections.ObjectModel;

namespace MicroStarter.Config;

public class TabRootViewModel
{
    public ObservableCollection<TabPageViewModel>? TabRootData { get; set; }
}