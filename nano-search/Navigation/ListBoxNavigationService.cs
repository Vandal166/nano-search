using System.Windows;
using System.Windows.Input;
using NanoSearch.Configuration;
using NanoSearch.Configuration.Keybindings;
using NanoSearch.Launchers;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch.Navigation;

public class ListBoxNavigationService : INavigationService
{
    private readonly IListBoxNavigationStrategyFactory _factory;
    private readonly IAppLauncher _appLauncher;
    private Dictionary<Key, INavigationStrategy> _keyStrategiesMap;
    private ListBox? _listBox;
    private Window? _window;
    
    public ListBoxNavigationService(IListBoxNavigationStrategyFactory factory, IAppLauncher appLauncher, IConfigService<KeybindingsOptions> kbConfig)
    {
        _factory = factory;
        _appLauncher = appLauncher;
        kbConfig.OptionsChanged += (_,__) => RebuildMap();
        RebuildMap(); // initial map build
    }

    public void RebuildMap()
    {
        var strategies = _factory.CreateStrategies(_appLauncher);
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