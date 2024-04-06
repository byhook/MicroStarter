using System.Windows;
using System.Windows.Controls;
using MicroStarter.Config;

namespace MicroStarter;

public partial class EditWindow : Window
{
    private TabItemViewModel _tabItemViewModel;

    public EditWindow(TabItemViewModel tabItemViewModel)
    {
        _tabItemViewModel = tabItemViewModel;
        InitializeComponent();
    }

    private void EditWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        TextBoxItemName.Text = _tabItemViewModel.ItemName ?? string.Empty;
        TextBoxItemPath.Text = _tabItemViewModel.ItemPath ?? string.Empty;
        TextBoxIconPath.Text = _tabItemViewModel.ItemIconPath ?? string.Empty;
        TextBoxRunCommand.Text = _tabItemViewModel.ItemRunCommand ?? string.Empty;
        CheckBoxRunWithAdmin.IsChecked = _tabItemViewModel.RunWithAdmin;
    }

    private void EditWindow_OnSaveClick(object sender, RoutedEventArgs e)
    {
        _tabItemViewModel.ItemName = TextBoxItemName.Text;
        _tabItemViewModel.ItemPath = TextBoxItemPath.Text;
        _tabItemViewModel.ItemIconPath = TextBoxIconPath.Text;
        _tabItemViewModel.ItemRunCommand = TextBoxRunCommand.Text;
        _tabItemViewModel.RunWithAdmin = CheckBoxRunWithAdmin.IsChecked.GetValueOrDefault(false);
        
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