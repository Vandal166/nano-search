namespace NanoSearch;

public interface IListBoxNavigationStrategyFactory
{
    IEnumerable<INavigationStrategy> CreateStrategies(IAppLauncher appLauncher);
}