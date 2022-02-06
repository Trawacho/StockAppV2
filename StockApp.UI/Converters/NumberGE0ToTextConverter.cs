using System;
using System.Globalization;
using System.Windows.Data;


namespace StockApp.UI.Converters;

internal class NumberGE0ToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int i)
        {
            if (i >= 0)
                return i.ToString();
            else
                return "";
        }
        else
        {
            return "";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Int32.TryParse((string)value, out int i))
        {
            return i;
        }
        else
        {
            return -1;
        }
    }
}
