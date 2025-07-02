using NanoSearch.Configuration;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Launchers;
using NanoSearch.Services;

namespace NanoSearch.Navigation;

public class ListBoxNavigationStrategyFactory : IListBoxNavigationStrategyFactory
{
    private readonly IConfigService<KeybindingsOptions> _kbOptions;

    public ListBoxNavigationStrategyFactory(IConfigService<KeybindingsOptions> kbOptions)
    {
        _kbOptions  = kbOptions;
    }
    
    public IEnumerable<INavigationStrategy> CreateStrategies(IAppLauncher appLauncher)
    {
        yield return new NavigationStrategy(
            _kbOptions.Options.Bindings[HotkeyAction.NavigateDown].ToWpfKey(),
            box => { 
                box.SelectedIndex = Math.Min(box.Items.Count - 1,
                    box.SelectedIndex + 1);
                box.ScrollIntoView(box.SelectedItem);
            });

        yield return new NavigationStrategy(
            _kbOptions.Options.Bindings[HotkeyAction.NavigateUp].ToWpfKey(),
            box => {
                box.SelectedIndex = Math.Max(0,
                    box.SelectedIndex - 1);
                box.ScrollIntoView(box.SelectedItem);
            });

        yield return new NavigationStrategy(
            _kbOptions.Options.Bindings[HotkeyAction.LaunchSelection].ToWpfKey(),
            box => {
                if (box?.SelectedItem == null)
                    return;

                if (box.SelectedItem is AppSearchResult item)
                    appLauncher.Launch(item.FullPath);
            });
    }
}