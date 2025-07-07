using System.ComponentModel;
using System.Windows.Input;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.UI.Services;
// ReSharper disable MemberCanBePrivate.Global
namespace NanoSearch.UI.ViewModels;

public class KeybindingsSettingsViewModel : INotifyPropertyChanged
{
    private readonly IConfigService<KeybindingsOptions> _keybindingsConfig;
    private readonly IDialogService _dialogService;
    public ICommand OpenKeybindingsCommand        { get; }
    private readonly Action _onOptionsChanged;
    public ICommand ExitCommand               { get; }
    
    public KeybindingsSettingsViewModel(IDialogService dialogService, IConfigService<KeybindingsOptions> keybindingConfig, Action onOptionsChanged)
    {
        _dialogService = dialogService;
        _keybindingsConfig = keybindingConfig;
        _onOptionsChanged = onOptionsChanged;

        OpenKeybindingsCommand = new RelayCommand(OpenKeybindingsOptions);
        ExitCommand = new RelayCommand(OnExit);
    }
    
    private void OpenKeybindingsOptions()
    {
        _keybindingsConfig.Load();
        var clonedOpts = new KeybindingsOptions();
        clonedOpts.CopyFrom(_keybindingsConfig.Options);
        var dlg = new KeybindingsOptionsViewModel(clonedOpts);
        if (_dialogService.ShowDialog(dlg))
        {
            _keybindingsConfig.Options.CopyFrom(clonedOpts);
            _keybindingsConfig.Save();
            _onOptionsChanged.Invoke();
        }
    }
    
    public void OnExit() => _keybindingsConfig.Save();
    
    public event PropertyChangedEventHandler? PropertyChanged;
}