using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NanoSearch.Launchers;
using NanoSearch.Navigation;
using NanoSearch.Services;
using Wpf.Ui.Controls;

// ReSharper disable once ConvertClosureToMethodGroup

namespace NanoSearch.UI.Windows;

public partial class SearchWindow : FluentWindow
{
    private readonly ISearchService _searchService;
    private readonly IAppLauncher _appLauncher;
    private readonly INavigationService _navigation;

    public SearchWindow(ISearchService searchService, IAppLauncher appLauncher, INavigationService navigation)
    {
        InitializeComponent();
        
        _searchService = searchService;
        _appLauncher = appLauncher;
        _navigation = navigation;

        // to center
        Loaded += (s, e) =>
        {
            ShowAndFocus();
            Top = SystemParameters.WorkArea.Height / 2 - Height * 2;
            Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
        };
        
        Activated += (_, _) => {
            ShowAndFocus();
            _navigation.Attach(SearchResultsPanel);
        };
        
        Deactivated += (_, _) => {
            // Close the window when it loses focus
            if(!IsVisible) 
                return;
            Hide();
            _navigation.Detach();
        };
        
        KeyDown +=(s, e) =>
        {
            // Close the window on Escape key press
            if (e.Key == Key.Escape) 
                Hide();
        };
    }
    

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
        SearchResultsPanel.ItemsSource = _searchService.Search(query).ToList();
        SearchResultsPanel.SelectedIndex = 0; // select first item by default
        SearchResultsPanel.ScrollIntoView(SearchResultsPanel.Items[0]!);
    }
    
    private void SearchResultsPanel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _appLauncher.Launch((SearchResultsPanel.SelectedItem as AppSearchResult)?.FullPath!);
    }
    public void ShowAndFocus()
    {
        if (!IsVisible)
            Show();

        Activate();
        Topmost = true;                   // top
        Topmost = false;                  // reset
    
        // delaying the actual keyboard focus so WPF has processed the activation
        Dispatcher.InvokeAsync(() =>
        {
            SearchBox.Focus();
            Keyboard.Focus(SearchBox);
        }, System.Windows.Threading.DispatcherPriority.Input);
    }
}