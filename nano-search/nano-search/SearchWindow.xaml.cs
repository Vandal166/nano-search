using System.Collections.Immutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using NanoSearch.Algorithms.Indexer;
using NanoSearch.Algorithms.RadixTrie;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

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
            Top = SystemParameters.WorkArea.Height / 2 - Height / 2;
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
    
    // Occurs each time the user types in the search box
    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        var query = SearchBox.Text;
        ImmutableHashSet<string>? results = _fileIndexer.RadixTree.SearchExact(query);
        
        if (results == null || results.Count == 0)
        {
            SearchResultsPanel.ItemsSource = new[]
            {
                new AppSearchResult(string.Empty)
                {
                    FileName = "No results found",
                    Icon = null
                }
            };
            return;
        }

        SearchResultsPanel.ItemsSource = results.Select(p => new AppSearchResult(p)).ToList();
    }
    
    // Occurs when the user clicks on a search result
    private void SearchResult_Click(object sender, RoutedEventArgs e) {
        // Handle search result click
        // Open file or perform action
        //Close(); // Close the search window after action
    }
}