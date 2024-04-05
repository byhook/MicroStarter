using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicroStarter;

public sealed class ConfigManager
{
    private const string ConfigDataName = "MicroConfig.json";

    private static readonly Lazy<ConfigManager> Lazy =
        new(() => new ConfigManager());

    //配置数据
    private ConfigItem MainConfigItem { get; set; } = new();

    public static ConfigManager GetInstance()
    {
        return Lazy.Value;
    }

    public void AddTabPage(String tabName)
    {
    }

    public bool RemoveTabItemData(int tabIndex, TabListItemData tabListItemData)
    {
        var tabData = MainConfigItem.TabRootData?[tabIndex];
        if (tabData?.TabItemDataList == null) return false;
        return tabData.TabItemDataList.Remove(tabListItemData);
    }

    public bool AddTabItemData(int tabIndex, TabListItemData tabListItemData)
    {
        TabListData? tabData = null;
        if (MainConfigItem.TabRootData == null)
        {
            MainConfigItem.TabRootData = new List<TabListData>();
            tabData = new TabListData();
            MainConfigItem.TabRootData.Add(tabData);
        }

        tabData = MainConfigItem.TabRootData[tabIndex];
        tabData.TabItemDataList ??= new List<TabListItemData>();
        if (tabData.TabItemDataList.Any(item => item.ItemPath == tabListItemData.ItemPath))
        {
            return false;
        }

        tabData.TabItemDataList?.Add(tabListItemData);
        return true;
    }


    public void SwapTabItemData(int tabIndex, int dragIndex, int dropIndex)
    {
        var tabData = MainConfigItem.TabRootData?[tabIndex];
        if (tabData?.TabItemDataList == null) return;
        var dragItemData = tabData.TabItemDataList[dragIndex];
        tabData.TabItemDataList.Remove(dragItemData);
        tabData.TabItemDataList.Insert(dropIndex, dragItemData);
        //保存变更记录
        SaveConfig();
    }

    public ConfigItem LoadConfig()
    {
        ConfigItem? tempConfigData = null;
        if (File.Exists(ConfigDataName))
        {
            var configContent = File.ReadAllText(ConfigDataName);
            if (!string.IsNullOrEmpty(configContent))
            {
                tempConfigData = JsonSerializer.Deserialize<ConfigItem>(configContent);
            }
        }

        if (tempConfigData != null)
        {
            MainConfigItem = tempConfigData;
        }
        else if (tempConfigData == null)
        {
            MainConfigItem = new ConfigItem();
        }

        if (MainConfigItem.TabRootData != null) return MainConfigItem;
        //本地没有配置或者解析失败-默认给个常用工具配置
        var tabDataList = new List<TabListData>();
        var tabData = new TabListData();
        tabData.TabName = "常用工具";
        tabDataList.Add(tabData);
        MainConfigItem.TabRootData = tabDataList;

        return MainConfigItem;
    }

    public void SaveConfig()
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        var content = JsonSerializer.Serialize(MainConfigItem, options);
        File.WriteAllText(ConfigDataName, content);
    }
}