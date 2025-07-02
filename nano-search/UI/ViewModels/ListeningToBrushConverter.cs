using System.Globalization;
using System.Windows.Data;

namespace NanoSearch.UI.ViewModels;

public class ListeningToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isListening = value is bool b && b;
        return isListening ? Brushes.Yellow : Brushes.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}