using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using MicroStarter.Config;

namespace MicroStarter;

public sealed class ConfigManager
{
    private const string ConfigDataName = "MicroConfig.json";

    private static readonly Lazy<ConfigManager> Lazy =
        new(() => new ConfigManager());

    //配置数据
    private TabRootViewModel MainTabRootViewModel { get; set; } = new();

    public static ConfigManager GetInstance()
    {
        return Lazy.Value;
    }

    public void AddTabPage(String tabName)
    {
    }

    public bool RemoveTabItemData(int tabIndex, TabItemViewModel tabItemViewModel)
    {
        var tabData = MainTabRootViewModel.TabRootData?[tabIndex];
        if (tabData?.TabItemDataList == null) return false;
        return tabData.TabItemDataList.Remove(tabItemViewModel);
    }

    public bool AddTabItemData(int tabIndex, TabItemViewModel tabItemViewModel)
    {
        TabPageViewModel? tabData = null;
        if (MainTabRootViewModel.TabRootData == null)
        {
            MainTabRootViewModel.TabRootData = new ObservableCollection<TabPageViewModel>();
            tabData = new TabPageViewModel();
            MainTabRootViewModel.TabRootData.Add(tabData);
        }

        tabData = MainTabRootViewModel.TabRootData[tabIndex];
        tabData.TabItemDataList ??= new ObservableCollection<TabItemViewModel>();
        if (tabData.TabItemDataList.Any(item => item.ItemPath == tabItemViewModel.ItemPath))
        {
            return false;
        }

        tabData.TabItemDataList?.Add(tabItemViewModel);
        return true;
    }


    public void SwapTabItemData(int tabIndex, int dragIndex, int dropIndex)
    {
        var tabData = MainTabRootViewModel.TabRootData?[tabIndex];
        if (tabData?.TabItemDataList == null) return;
        var dragItemData = tabData.TabItemDataList[dragIndex];
        tabData.TabItemDataList.Remove(dragItemData);
        tabData.TabItemDataList.Insert(dropIndex, dragItemData);
        //保存变更记录
        SaveConfig();
    }

    public TabRootViewModel LoadConfig()
    {
        TabRootViewModel? tempConfigData = null;
        if (File.Exists(ConfigDataName))
        {
            var configContent = File.ReadAllText(ConfigDataName);
            if (!string.IsNullOrEmpty(configContent))
            {
                tempConfigData = JsonSerializer.Deserialize<TabRootViewModel>(configContent);
            }
        }

        if (tempConfigData != null)
        {
            MainTabRootViewModel = tempConfigData;
        }
        else if (tempConfigData == null)
        {
            MainTabRootViewModel = new TabRootViewModel();
        }

        if (MainTabRootViewModel.TabRootData != null) return MainTabRootViewModel;
        //本地没有配置或者解析失败-默认给个常用工具配置
        var tabDataList = new ObservableCollection<TabPageViewModel>();
        var tabData = new TabPageViewModel();
        tabData.TabName = "常用工具";
        tabDataList.Add(tabData);
        MainTabRootViewModel.TabRootData = tabDataList;

        return MainTabRootViewModel;
    }

    public void SaveConfig()
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        var content = JsonSerializer.Serialize(MainTabRootViewModel, options);
        File.WriteAllText(ConfigDataName, content);
    }
}