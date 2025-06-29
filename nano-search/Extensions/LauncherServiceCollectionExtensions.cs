using Microsoft.Extensions.DependencyInjection;
using NanoSearch.Launchers;
using NanoSearch.Services;

namespace NanoSearch.Configuration.Services;

public static class LauncherServiceCollectionExtensions
{
    public static IServiceCollection AddLaunchers(this IServiceCollection services)
    {
        services.AddSingleton<IAppLauncher, AppLauncher>();
        services.AddSingleton<AppLauncher>();
        services.AddSingleton<IIconLoader, IconLoader>();
        services.AddSingleton<ExplorerLauncher>();
        services.AddSingleton<LinkLauncher>();
        return services;
    }
}