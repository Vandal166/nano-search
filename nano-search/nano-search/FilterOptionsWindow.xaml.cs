using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch;

public partial class FilterOptionsWindow : Window
{
    private readonly FileFilterOptions _opts;
    public FilterOptionsWindow(FileFilterOptions options)
    {
        InitializeComponent();
        _opts = options;
        DataContext = _opts;
        RefreshLists();
        
        Loaded += (s, e) =>
        {
            //center
            Top = SystemParameters.WorkArea.Height / 2 - Height / 2;    
            Left = SystemParameters.WorkArea.Width / 2 - Width / 2;
        };
    }
    
    private void OnAddExcludeChar(object s, RoutedEventArgs e)
    {
        var c = NewCharBox.Text.FirstOrDefault();
        if (c != 0)
        {
            _opts.ExcludeBeginningWith.Add(c);
            RefreshLists();
        }
    }

    private void OnRemoveExcludeChar(object s, RoutedEventArgs e)
    {
        var selected = ExcludedCharsList.SelectedItem;
        if (selected != null)
        {
            _opts.ExcludeBeginningWith.Remove((char)selected);
            RefreshLists();
        }
    }

    private void OnAddExtension(object s, RoutedEventArgs e)
    {
        var ext = NewExtBox.Text;
        if (!string.IsNullOrWhiteSpace(ext) && !_opts.IncludedFileExtensions.Contains(ext))
            _opts.IncludedFileExtensions.Add(ext);
    }

    private void OnRemoveExtension(object s, RoutedEventArgs e)
    {
        var selected = (string)((System.Windows.Controls.ListBox)FindName("ExtensionsList")).SelectedItem;
        if (selected != null)
            _opts.IncludedFileExtensions.Remove(selected);
    }
    
    private void RefreshLists()
    {
        ExcludedCharsList.ItemsSource = null;
        ExcludedCharsList.ItemsSource = _opts.ExcludeBeginningWith.ToList();
        
        /*
        var extensionsList = (System.Windows.Controls.ListBox)FindName("ExtensionsList");
        extensionsList.ItemsSource = null;
        extensionsList.ItemsSource = _opts.IncludedFileExtensions.ToList();*/
    }

    private void OnOk(object s, RoutedEventArgs e) => DialogResult = true;
    private void OnCancel(object s, RoutedEventArgs e) => DialogResult = false;
}

public class EnumFlagMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is Enum enumValue && values[1] != null)
        {
            var paramValue = Enum.Parse(enumValue.GetType(), values[1].ToString());
            return enumValue.HasFlag((Enum)paramValue);
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        // Not implemented for brevity
        throw new NotImplementedException();
    }
}
