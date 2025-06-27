namespace NanoSearch;

public class ListBoxNavigationStrategyFactory : IListBoxNavigationStrategyFactory
{
    public IEnumerable<INavigationStrategy> CreateStrategies(IAppLauncher appLauncher)
    {
        return new INavigationStrategy[]
        {
            new ListBoxNavigateDownStrategy(),
            new ListboxNavigationUpStrategy(),
            new ListboxNavigationEnterStrategy(appLauncher)
        };
    }
}