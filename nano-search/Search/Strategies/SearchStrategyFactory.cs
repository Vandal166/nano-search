using NanoSearch.Search.Filters;

namespace NanoSearch.Search.Strategies;

public static class SearchStrategyFactory
{
    private static readonly List<ISearchStrategy> _listOfSearchStrategies = new List<ISearchStrategy>();
    
    static SearchStrategyFactory()
    {
        // registering default strategies
        RegisterStrategy(new FilteringSearchStrategy(new RegexSearchStrategy(), new RegexSearchFilter()));
        RegisterStrategy(new FilteringSearchStrategy(new ExactSearchStrategy(), new ExactSearchFilter()));
        RegisterStrategy(new FilteringSearchStrategy(new PrefixSearchStrategy(), new PrefixSearchFilter()));
        //TODO if the first strategy has the default implt of CanHandle then it will be checked first...
    }
    public static ISearchStrategy? GetStrategy(string query)
    {
        //TODO get from Indexing config
        
        return _listOfSearchStrategies.FirstOrDefault(x => x.CanHandle(query));
    }
    
    // used to register new strategies dynamically
    public static void RegisterStrategy(ISearchStrategy strategy)
    {
        _listOfSearchStrategies.Add(strategy);
    }
}