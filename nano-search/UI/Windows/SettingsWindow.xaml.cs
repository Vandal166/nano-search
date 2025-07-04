using System.Windows;
using NanoSearch.UI.ViewModels;

namespace NanoSearch.UI.Windows;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        
        vm.Indexing.snackDialogService.SetDialogHost(RootContentDialog);
        vm.Indexing.snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        
        Deactivated += (s, e) =>
        {
            Hide();
        };
    }
}