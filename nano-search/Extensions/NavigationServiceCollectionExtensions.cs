using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Configuration.Indexing;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Launchers;
using NanoSearch.Navigation;
using NanoSearch.Navigation.Hotkey;
using NanoSearch.UI.Windows;

namespace NanoSearch.Configuration.Services;

public static class NavigationServiceCollectionExtensions
{
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<KeybindingsOptions>();
        
        services.AddSingleton<IHotKeyAction, ShowWindowHotKey>();
        services.AddSingleton<IHotKeyService>(sp =>
            new HotKeyService(
                sp.GetRequiredService<SearchWindow>(), sp.GetRequiredService<IConfigService<KeybindingsOptions>>(),
                sp.GetRequiredService<Func<HotkeyAction, IHotKeyAction>>())
        );
        
        services.AddSingleton<Func<HotkeyAction, IHotKeyAction>>(sp => action =>
        {
            var window = sp.GetRequiredService<SearchWindow>();

            return new ShowWindowHotKey(window);
        });
        
        services.AddSingleton<IListBoxNavigationStrategyFactory, ListBoxNavigationStrategyFactory>();
        services.AddSingleton<INavigationService>(provider =>
        {
            var factory = provider.GetRequiredService<IListBoxNavigationStrategyFactory>();
            var appLauncher = provider.GetRequiredService<IAppLauncher>();
            var cfg = provider.GetRequiredService<IConfigService<KeybindingsOptions>>();
            
            return new ListBoxNavigationService(factory, appLauncher, cfg);
        });
        return services;
    }
}