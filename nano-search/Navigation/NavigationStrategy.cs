using System.Windows.Input;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch.Navigation;

/// <summary>
/// Used for defining a navigation strat for search results in a ListBox.
/// </summary>
public class NavigationStrategy : INavigationStrategy
{
    public Key ShortcutKey { get; }

    private readonly Action<ListBox> _action;

    public NavigationStrategy(Key key, Action<ListBox> action)
    {
        ShortcutKey = key;
        _action = action;
    }

    public void Execute(ListBox listBox) => _action(listBox);
}