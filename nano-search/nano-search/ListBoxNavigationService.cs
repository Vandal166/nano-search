using System.Windows;
using System.Windows.Input;
using ListBox = System.Windows.Controls.ListBox;

namespace NanoSearch;

public class ListBoxNavigationService : INavigationService
{
    private readonly IAppLauncher _appLauncher;
    private ListBox? _listBox;
    private Window? _window;
    
    public ListBoxNavigationService(IAppLauncher appLauncher)
    {
        _appLauncher = appLauncher;
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
        if (_window == null || !_window.IsVisible) 
            return;

        if (_listBox == null || _listBox.Items.Count == 0)
            return;

        switch (e.Key)
        {
            case Key.Down:
                _listBox.SelectedIndex = Math.Min(_listBox.Items.Count - 1,
                    _listBox.SelectedIndex + 1);
                _listBox.ScrollIntoView(_listBox.SelectedItem);
                e.Handled = true;
                break;

            case Key.Up:
                _listBox.SelectedIndex = Math.Max(0,
                    _listBox.SelectedIndex - 1);
                _listBox.ScrollIntoView(_listBox.SelectedItem);
                e.Handled = true;
                break;

            case Key.Enter:
                if (_listBox.SelectedItem is AppSearchResult item)
                    _appLauncher.Launch(item.FullPath);
                e.Handled = true;
                break;
        }
    }
}