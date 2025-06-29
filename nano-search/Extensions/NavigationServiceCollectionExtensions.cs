using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Launchers;
using NanoSearch.Navigation;

namespace NanoSearch.Configuration.Services;

public static class NavigationServiceCollectionExtensions
{
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<ListBoxNavigateDownStrategy>();
        services.AddSingleton<ListboxNavigationUpStrategy>();
        services.AddSingleton<ListboxNavigationEnterStrategy>();
        services.AddSingleton<IListBoxNavigationStrategyFactory, ListBoxNavigationStrategyFactory>();
        services.AddSingleton<INavigationService>(provider =>
        {
            var factory = provider.GetRequiredService<IListBoxNavigationStrategyFactory>();
            var appLauncher = provider.GetRequiredService<IAppLauncher>();
            
            var strategies = factory.CreateStrategies(appLauncher);
            return new ListBoxNavigationService(strategies);
        });
        return services;
    }
}