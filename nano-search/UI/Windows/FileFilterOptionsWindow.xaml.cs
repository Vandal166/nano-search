using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NanoSearch.UI.ViewModels;

namespace NanoSearch.UI.Windows;

public partial class FileFilterOptionsWindow : Window
{
    public FileFilterOptionsWindow(FileFilterOptionsViewModel vm)
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