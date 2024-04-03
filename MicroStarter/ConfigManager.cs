﻿using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicroStarter;

public sealed class ConfigManager
{
    private const string ConfigDataName = "MicroConfig.json";

    private static readonly Lazy<ConfigManager> _lazy =
        new Lazy<ConfigManager>(() => new ConfigManager());

    //配置数据
    private ConfigData MainConfigData { get; set; }

    private ConfigManager()
    {
    }

    public static ConfigManager GetInstance()
    {
        return _lazy.Value;
    }

    public void AddTabPage(String tabName)
    {
    }

    public bool RemoveTabItemData(int tabIndex, TabItemData tabItemData)
    {
        var tabData = MainConfigData.TabRootData[tabIndex];
        if (tabData.TabItemDataList != null)
        {
            return tabData.TabItemDataList.Remove(tabItemData);
        }

        return false;
    }

    public bool AddTabItemData(int tabIndex, TabItemData tabItemData)
    {
        TabData? tabData = null;
        if (MainConfigData?.TabRootData == null)
        {
            MainConfigData.TabRootData = new List<TabData>();
            tabData = new TabData();
            MainConfigData.TabRootData.Add(tabData);
        }

        tabData = MainConfigData.TabRootData[tabIndex];
        if (tabData.TabItemDataList == null)
        {
            tabData.TabItemDataList = new List<TabItemData>();
        }

        foreach (var item in tabData.TabItemDataList)
        {
            if (item.ItemPath == tabItemData.ItemPath)
            {
                return false;
            }
        }

        tabData.TabItemDataList?.Add(tabItemData);
        return true;
    }


    public void SwapTabItemData(int tabIndex, int dragIndex, int dropIndex)
    {
        var tabData = MainConfigData.TabRootData?[tabIndex];
        if (tabData?.TabItemDataList != null)
        {
            var dragItemData = tabData.TabItemDataList[dragIndex];
            tabData.TabItemDataList.Remove(dragItemData);
            tabData.TabItemDataList.Insert(dropIndex, dragItemData);
            //保存变更记录
            SaveConfig();
        }
    }

    public ConfigData LoadConfig()
    {
        ConfigData? tempConfigData = null;
        if (File.Exists(ConfigDataName))
        {
            String? configContent = File.ReadAllText(ConfigDataName);
            if (!string.IsNullOrEmpty(configContent))
            {
                tempConfigData = JsonSerializer.Deserialize<ConfigData>(configContent);
            }
        }

        if (tempConfigData != null)
        {
            MainConfigData = tempConfigData;
        }
        else if (tempConfigData == null)
        {
            MainConfigData = new ConfigData();
        }

        if (MainConfigData.TabRootData == null)
        {
            //本地没有配置或者解析失败-默认给个常用工具配置
            var tabDataList = new List<TabData>();
            var tabData = new TabData();
            tabData.TabName = "常用工具";
            tabDataList.Add(tabData);
            MainConfigData.TabRootData = tabDataList;
        }

        return MainConfigData;
    }

    public void SaveConfig()
    {
        if (MainConfigData != null)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            string content = JsonSerializer.Serialize(MainConfigData, options);
            File.WriteAllText(ConfigDataName, content);
        }
    }
}