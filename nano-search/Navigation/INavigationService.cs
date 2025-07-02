using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch.Navigation;

public interface INavigationService
{
    void Attach(ListBox listBox);
    void Detach();
    void RebuildMap();
}