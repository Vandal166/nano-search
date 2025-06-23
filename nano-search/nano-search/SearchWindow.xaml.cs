using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Algorithms.RadixTrie;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace NanoSearch;

public partial class SearchWindow : FluentWindow
{
    private readonly FileIndexer _fileIndexer;
    public SearchWindow(FileIndexer fileIndexer)
    {
        InitializeComponent();
        
        _fileIndexer = fileIndexer;
        Console.WriteLine($"Indexer loaded and indexed - count :{_fileIndexer.RadixTree.Count}");
        ApplicationThemeManager.Apply(
            ApplicationTheme.Dark,                // Light or Dark theme
            WindowBackdropType.Mica,             // Mica or Acrylic
            updateAccent: true
        );
        
        SystemThemeWatcher.Watch(this);
        
        // to center
        Loaded += (s, e) =>
        {
            SearchBox.Focus();
            Top = SystemParameters.WorkArea.Height / 2 - Height * 2;
            Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
        };
        
        Activated += (_, _) => {
            this.Focus();
        };
        
        Deactivated += (_, _) => {
            // Close the window when it loses focus
            if(!IsVisible) 
                return;
            Hide();
        };
        
        KeyDown +=(s, e) =>
        {
            // Close the window on Escape key press
            if (e.Key == Key.Escape) 
                Hide();
        };
    }
    
    //TODO allow navigation results via arrow keys
    
    
    // Occurs each time the user types in the search box
    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var query = SearchBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            SearchResultsPanel.ItemsSource = null;
            return;
        }
       
        PerformSearch(query);
    }
    
    private void PerformSearch(string query)
    {
        var strategy = SearchStrategyFactory.GetFor(query);
        var results  = strategy.Search(_fileIndexer.RadixTree, query);
        
        if (results == null || results.Count == 0)
        {
            SearchResultsPanel.ItemsSource = new[]
            {
                new AppSearchResult(string.Empty)
                {
                    FileName = "No results found"
                }
            };
            return;
        }
        // get by closeness of length to the query
        var sortedResults = results
            .Select(path => new { Path = path, FileName = Path.GetFileName(path) })
            .OrderBy(x => Math.Abs(x.FileName.Length - query.Length))
            .ThenBy(x => x.FileName, StringComparer.OrdinalIgnoreCase)
            .Select(x => new AppSearchResult(x.Path))
            .ToList();

        SearchResultsPanel.ItemsSource = sortedResults;
    }

    // Occurs when the user clicks on a search result
    private void SearchResult_Click(object sender, RoutedEventArgs e) {
        // Handle search result click
        // Open file or perform action
        //Close(); // Close the search window after action
    }
}

public interface ISearchStrategy
{
    ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query);
}

public class ExactSearchStrategy : ISearchStrategy
{
    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
        => tree.SearchExact(query); //TODO also .Take(MAX_RESULTS) here or inside RadixTree to limit results CONFIGURABLE via conf opts
}

public class PrefixSearchStrategy : ISearchStrategy
{
    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
        => tree.SearchPrefix(query);
}

public interface ISearchFilter
{
    /// <summary>
    /// Takes the raw results (or null) and returns a new (or same) set.
    /// </summary>
    ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query);
}

public class RegexSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        // e.g. keep only the first N matches
        return rawResults?.Take(50)
            .ToImmutableHashSet();
    }
}

public class ExactSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        return rawResults?.Take(50).ToImmutableHashSet();
    }
}

public class PrefixSearchFilter : ISearchFilter
{
    public ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query)
    {
        return rawResults?.Take(50) // TODO configurable via options
            .ToImmutableHashSet();
    }
}
public class FilteringSearchStrategy : ISearchStrategy
{
    private readonly ISearchStrategy _inner;
    private readonly ISearchFilter   _filter;

    public FilteringSearchStrategy(ISearchStrategy inner, ISearchFilter filter)
    {
        _inner  = inner;
        _filter = filter;
    }

    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
    {
        var raw = _inner.Search(tree, query);

        // applying the filter, returning the filtered results
        return _filter.Apply(raw, query);
    }
}

public class RegexSearchStrategy : ISearchStrategy
{
    public ImmutableHashSet<string>? Search(RadixTree<ImmutableHashSet<string>> tree, string query)
    {
        try
        {
            string regexPattern = ConvertToRegex(query);
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            var results = tree.SearchRegex(regex)
                .SelectMany(pathSet => pathSet)
                .ToImmutableHashSet();
            
            return results;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static string ConvertToRegex(string pattern)
    {
        // Escape regex metacharacters, then replace wildcards
        string regexPattern = "^" + Regex.Escape(pattern)
            .Replace(@"\*", ".*")
            .Replace(@"\?", ".") + "$";
        return regexPattern;
    }
}

public static class SearchStrategyFactory
{
    private static readonly ISearchStrategy _filteredRegexSearchStrategy = 
        new FilteringSearchStrategy(new RegexSearchStrategy(), new RegexSearchFilter());
    
    private static readonly ISearchStrategy _filteredPrefixSearchStrategy = 
        new FilteringSearchStrategy(new PrefixSearchStrategy(), new PrefixSearchFilter());
    
    private static readonly ISearchStrategy _filteredExactSearchStrategy = 
        new FilteringSearchStrategy(new ExactSearchStrategy(), new ExactSearchFilter());
    
    public static ISearchStrategy GetFor(string query)
    {
        if (IsRegexPattern(query))
            return _filteredRegexSearchStrategy;


        if(EndsWithExtension(query)) //TODO get from Indexing config
            return _filteredExactSearchStrategy;


        return _filteredPrefixSearchStrategy;
    }
    
    private static bool IsRegexPattern(string input)
    {
        return input.Contains('*') || 
               input.Contains('?') || 
               input.Contains('[') || 
               input.Contains(']') || 
               input.Contains('^') || 
               input.Contains('$');
    }
    
    private static bool EndsWithExtension(string input)
    {
        return input.Contains('.') || 
               input.Length > 3 && char.IsDigit(input[^1]);
    }
}


