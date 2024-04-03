namespace MicroStarter;

public class ConfigData
{
    public List<TabData>? TabRootData { get; set; }
}

public class TabData
{
    public string? TabName { get; set; }

    public List<TabItemData>? TabItemDataList { get; set; }
}

public class TabItemData
{
    public string? ItemName { get; set; }

    public string? ItemPath { get; set; }

    public string? ItemIconPath { get; set; }

    public string? ItemRunCommand { get; set; }

    public bool RunWithAdmin { get; set; }
}