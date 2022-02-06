using System;
using System.Globalization;
using System.Windows.Data;


namespace StockApp.UI.Converters;

internal class GameGroupStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int i)
        {
            return Core.Converter.GameGroupStringConverter.Convert(i);
        }
        else
        {
            throw new ArgumentException("Value must be an integer");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            return Core.Converter.GameGroupStringConverter.Convert(s);
        }
        else
        {
            return new ArgumentException("value must be a string");
        }
    }
}
