using System.Windows.Input;
using NanoSearch.Launchers;
using NanoSearch.Services;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch.Navigation;

public class ListboxNavigationEnterStrategy : INavigationStrategy
{
    public Key ShortcutKey { get; } = Key.Enter;
    private readonly IAppLauncher _appLauncher;
    
    public ListboxNavigationEnterStrategy(IAppLauncher appLauncher)
    {
        _appLauncher = appLauncher;
    }

    public void Execute(ListBox listBox)
    {
        if (listBox?.SelectedItem == null)
            return;

        if (listBox.SelectedItem is AppSearchResult item)
            _appLauncher.Launch(item.FullPath);
    }
}