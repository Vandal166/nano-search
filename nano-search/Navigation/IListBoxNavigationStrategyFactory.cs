using NanoSearch.Launchers;

namespace NanoSearch.Navigation;

public interface IListBoxNavigationStrategyFactory
{
    IEnumerable<INavigationStrategy> CreateStrategies(IAppLauncher appLauncher);
}