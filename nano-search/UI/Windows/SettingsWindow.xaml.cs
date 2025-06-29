using System.Windows;
using NanoSearch.UI.ViewModels;

namespace NanoSearch.UI.Windows;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        
        vm.snackDialogService.SetDialogHost(RootContentDialog);
        vm.snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        
        Deactivated += (s, e) =>
        {
            Hide();
        };
    }
}