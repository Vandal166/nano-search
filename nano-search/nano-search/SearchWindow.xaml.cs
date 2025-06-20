using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace NanoSearch;

public partial class SearchWindow : FluentWindow
{
    public SearchWindow()
    {
        InitializeComponent();
        ApplicationThemeManager.Apply(
            ApplicationTheme.Dark,                // Light or Dark theme
            WindowBackdropType.Mica,             // Mica or Acrylic
            updateAccent: true
        );
        
        SystemThemeWatcher.Watch(this);
        
        // to center
        Loaded += (s, e) => {
            SearchBox.Focus();
            Top = SystemParameters.WorkArea.Height / 2 - Height * 2;
            Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
        };
    }
    
    private void SearchBox_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Escape) {
            Close();
        } else if (e.Key == Key.Enter) {
            string query = SearchBox.Text;
            // Run search and show results
        }
    }
}