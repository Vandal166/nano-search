using System.Windows.Input;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch.Navigation;

public class ListBoxNavigateDownStrategy : INavigationStrategy
{
    public Key ShortcutKey { get; } = Key.Down;
    
    public void Execute(ListBox listBox)
    {
        listBox.SelectedIndex = Math.Min(listBox.Items.Count - 1,
            listBox.SelectedIndex + 1);
        listBox.ScrollIntoView(listBox.SelectedItem);
    }
}