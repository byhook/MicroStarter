using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WinStarter
{
    public sealed class ConfigManager
    {
        private const String CONFIG_DATA_NAME = "MicroConfig.json";

        private static readonly Lazy<ConfigManager> _lazy =
            new Lazy<ConfigManager>(() => new ConfigManager());

        //配置数据
        private ConfigData mainConfigData { get; set; }

        private ConfigManager() {

        }

        public static ConfigManager GetInstance()
        {
            return _lazy.Value;
        }

        public void AddTabPage(String tabName)
        {

        }

        public bool AddTabItemData(int tabIndex, TabItemData tabItemData)
        {
            TabData? tabData = null;
            if (mainConfigData?.TabRootData == null)
            {
                mainConfigData.TabRootData = new List<TabData>();
                tabData = new TabData();
                mainConfigData.TabRootData.Add(tabData);
            }
            tabData = mainConfigData.TabRootData[tabIndex];
            if (tabData.TabItemDatas == null)
            {
                tabData.TabItemDatas = new List<TabItemData>();
            }
            foreach (var item in tabData.TabItemDatas)
            {
                if (item.ItemPath == tabItemData.ItemPath)
                {
                    return false;
                }
            }
            tabData.TabItemDatas?.Add(tabItemData);
            return true;
        }

        public ConfigData LoadConfig()
        {
            ConfigData? tempConfigData = null;
            if (File.Exists(CONFIG_DATA_NAME))
            {
                String? configContent = File.ReadAllText(CONFIG_DATA_NAME);
                if (!string.IsNullOrEmpty(configContent))
                {
                    tempConfigData = JsonSerializer.Deserialize<ConfigData>(configContent);
                }
            }
            if (tempConfigData != null)
            {
                mainConfigData = tempConfigData;
            } else if (tempConfigData == null)
            {
                mainConfigData = new ConfigData();
            }
            if (mainConfigData.TabRootData == null)
            {
                //本地没有配置或者解析失败-默认给个常用工具配置
                var tabDataList = new List<TabData>();
                var tabData = new TabData();
                tabData.TabName = "常用工具";
                tabDataList.Add(tabData);
                mainConfigData.TabRootData = tabDataList;
            }
            return mainConfigData;
        }

        public void SaveConfig()
        {
            if (mainConfigData != null)
            {
                string content = JsonSerializer.Serialize(mainConfigData);
                File.WriteAllText(CONFIG_DATA_NAME, content);
            }
        }

    }

}
