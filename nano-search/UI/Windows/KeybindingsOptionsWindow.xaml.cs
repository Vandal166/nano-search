using System.Windows;
using System.Windows.Input;
using NanoSearch.UI.ViewModels;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;
namespace NanoSearch.UI.Windows;

public partial class KeybindingsOptionsWindow : Window
{
    public KeybindingsOptionsWindow(KeybindingsOptionsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        
        Loaded += (s, e) =>
        {
            Top = SystemParameters.WorkArea.Height / 2 - Height / 2;    
            Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
        };
    }
    
    private void OnOk(object s, RoutedEventArgs e) => DialogResult = true;
    private void OnCancel(object s, RoutedEventArgs e) => DialogResult = false;

    private void ModifiersBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is KeybindingItemViewModel vm && vm.IsListeningForModifiers)
        {
            tb.Focus();
            // Convert Key to KeyModifiers
            var key = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);
            //vm.Modifiers = key; // Assuming you have a helper method to convert
            vm.IsListeningForModifiers = false;
            e.Handled = true;
        }
    }

    private void ModifiersBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is KeybindingItemViewModel vm)
        {
            tb.Text = "Press a key";
            vm.IsListeningForModifiers = true;
            tb.Focus();
            e.Handled = true; // Prevents the button from being clicked
        }
    }

    private void KeyBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is KeybindingItemViewModel vm && vm.IsListeningForKey)
        {
            tb.Focus();
            vm.Key = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);
            // Optionally set Modifiers as well
            vm.IsListeningForKey = false;
            e.Handled = true;
        }
    }
    private void SuppressTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = true;
    }
    private void KeyBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBox tb && tb.DataContext is KeybindingItemViewModel vm)
        {
            tb.Text = "Press a key";
            vm.IsListeningForKey = true;
            tb.Focus();
            e.Handled = true; // Prevents the button from being clicked
        }
    }

}