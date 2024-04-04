using System.Windows;

namespace MicroStarter;

public partial class EditWindow : Window
{
    private TabItemData _tabItemData;

    public EditWindow(TabItemData tabItemData)
    {
        this._tabItemData = tabItemData;
        InitializeComponent();
    }


    private void EditWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        TextBoxItemName.Text = _tabItemData.ItemName ?? string.Empty;
        TextBoxItemPath.Text = _tabItemData.ItemPath ?? string.Empty;
        TextBoxIconPath.Text = _tabItemData.ItemIconPath ?? string.Empty;
        TextBoxRunCommand.Text = _tabItemData.ItemRunCommand;
        CheckBoxRunWithAdmin.IsChecked = _tabItemData.RunWithAdmin;
    }

    private void EditWindow_OnSaveClick(object sender, RoutedEventArgs e)
    {
        _tabItemData.ItemName = TextBoxItemName.Text;
        _tabItemData.ItemPath = TextBoxItemPath.Text;
        _tabItemData.ItemIconPath = TextBoxIconPath.Text;
        _tabItemData.ItemRunCommand = TextBoxRunCommand.Text;
        _tabItemData.RunWithAdmin = CheckBoxRunWithAdmin.IsChecked.GetValueOrDefault(false);
        
        ConfigManager.GetInstance().SaveConfig();
        //关闭当前对话框
        Close();
    }

    private void EditWindow_OnCancelClick(object sender, RoutedEventArgs e)
    {
        //关闭
        Close();
    }
}