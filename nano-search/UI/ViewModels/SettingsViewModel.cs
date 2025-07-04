using System.Windows.Input;
using Application = System.Windows.Application;

// ReSharper disable MemberCanBePrivate.Global
namespace NanoSearch.UI.ViewModels;

public class SettingsViewModel
{
    public IndexingSettingsViewModel  Indexing { get; }
    public KeybindingsSettingsViewModel Keybindings { get; }
    public ICommand ExitCommand { get; }
   
    public SettingsViewModel(IndexingSettingsViewModel indexing, KeybindingsSettingsViewModel keybindings)
    {
        // delegating the DataContext of the window to the sub-viewmodels
        Indexing = indexing;
        Keybindings = keybindings;
        ExitCommand = new RelayCommand(() =>
        {
            Indexing.OnExit();
            Keybindings.OnExit();
            Application.Current.Shutdown();
        });
    }
}