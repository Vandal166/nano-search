using System.Globalization;
using System.Windows.Data;

namespace NanoSearch.UI.ViewModels;

public class KeyToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Keys key)
            return key.ToString();
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}