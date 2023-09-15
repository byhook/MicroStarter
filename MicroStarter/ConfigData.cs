using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinStarter
{
    public class ConfigData
    {

        public List<TabData>? TabRootData { get; set; }

    }

    public class TabData
    {

        public String? TabName { get; set; }

        public List<TabItemData>? TabItemDatas { get; set; }

    }

    public class TabItemData
    {

        public String? ItemName { get; set; }

        public String? ItemPath { get; set; }

    }

}
