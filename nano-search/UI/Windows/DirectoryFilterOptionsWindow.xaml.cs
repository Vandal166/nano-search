using System.Windows;
using NanoSearch.UI.ViewModels;

namespace NanoSearch.UI.Windows;

public partial class DirectoryFilterOptionsWindow : Window
{
    public DirectoryFilterOptionsWindow(DirectoryFilterOptionsViewModel vm)
    {
        InitializeComponent();
        
        DataContext = vm;
        Loaded += (s, e) =>
        {
            //center
            Top = SystemParameters.WorkArea.Height / 2 - Height / 2;    
            Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
        };
    }
    
    private void OnOk(object s, RoutedEventArgs e) => DialogResult = true;
    private void OnCancel(object s, RoutedEventArgs e) => DialogResult = false;
}