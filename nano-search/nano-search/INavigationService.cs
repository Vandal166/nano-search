using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch;

public interface INavigationService
{
    void Attach(ListBox listBox);
    void Detach();
}