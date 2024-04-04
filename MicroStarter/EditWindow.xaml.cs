using System.Windows;
using System.Windows.Controls;

namespace MicroStarter;

public partial class EditWindow : Window
{
    private TabListItemData _tabListItemData;

    public EditWindow(TabListItemData tabListItemData)
    {
        _tabListItemData = tabListItemData;
        InitializeComponent();
    }

    private void EditWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        TextBoxItemName.Text = _tabListItemData.ItemName ?? string.Empty;
        TextBoxItemPath.Text = _tabListItemData.ItemPath ?? string.Empty;
        TextBoxIconPath.Text = _tabListItemData.ItemIconPath ?? string.Empty;
        TextBoxRunCommand.Text = _tabListItemData.ItemRunCommand ?? string.Empty;
        CheckBoxRunWithAdmin.IsChecked = _tabListItemData.RunWithAdmin;
    }

    private void EditWindow_OnSaveClick(object sender, RoutedEventArgs e)
    {
        _tabListItemData.ItemName = TextBoxItemName.Text;
        _tabListItemData.ItemPath = TextBoxItemPath.Text;
        _tabListItemData.ItemIconPath = TextBoxIconPath.Text;
        _tabListItemData.ItemRunCommand = TextBoxRunCommand.Text;
        _tabListItemData.RunWithAdmin = CheckBoxRunWithAdmin.IsChecked.GetValueOrDefault(false);
        
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