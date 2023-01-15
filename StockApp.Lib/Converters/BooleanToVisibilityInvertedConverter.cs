using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StockApp.Lib.Converters;

public class BooleanToVisibilityInvertedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Visibility.Collapsed : Visibility.Visible;
        return new ArgumentException($"Value of type {value.GetType()} not allowed.");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
