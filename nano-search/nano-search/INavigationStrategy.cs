using System.Windows.Input;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch;

public interface INavigationStrategy
{
    Key ShortcutKey { get; }
    void Execute(ListBox listBox);
}