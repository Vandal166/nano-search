using System.Windows.Input;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch.Navigation;

public class ListboxNavigationUpStrategy : INavigationStrategy
{
    public Key ShortcutKey { get; } = Key.Up;
    
    public void Execute(ListBox listBox)
    {
        listBox.SelectedIndex = Math.Max(0,
            listBox.SelectedIndex - 1);
        listBox.ScrollIntoView(listBox.SelectedItem);
    }
}