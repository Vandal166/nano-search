using System.Windows;
using System.Windows.Input;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch.Navigation;

public class ListBoxNavigationService : INavigationService
{
    private readonly Dictionary<Key, INavigationStrategy> _keyStrategiesMap;
    private ListBox? _listBox;
    private Window? _window;
    
    public ListBoxNavigationService(IEnumerable<INavigationStrategy> strategies)
    {
        _keyStrategiesMap = strategies.ToDictionary(s => s.ShortcutKey, s => s); // mapping <shortcut key, strategy>
    }
    public void Attach(ListBox listBox)
    {
        _listBox    = listBox;
        _window = Window.GetWindow(listBox);
        if (_window != null) 
            _window.PreviewKeyDown += OnKeyDown;
    }
    

    public void Detach()
    {
        if (_window != null)
            _window.PreviewKeyDown -= OnKeyDown;
        _listBox = null;
    }

    private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        // only when window is visible & the ListBox has items
        if (_window?.IsVisible == false || _listBox?.Items.Count == 0) 
            return;

        if (_keyStrategiesMap.TryGetValue(e.Key, out var strat))
        {
            strat.Execute(_listBox);
            e.Handled = true;
        }
    }
}