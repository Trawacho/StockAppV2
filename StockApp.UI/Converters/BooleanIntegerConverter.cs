using System;
using System.Globalization;
using System.Windows.Data;

namespace StockApp.UI.Converters;

/// <summary>
/// compares the value with the parameter and returns the equality
/// </summary>
internal class BooleanIntegerConverter : IValueConverter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">integer</param>
    /// <param name="targetType">Boolean</param>
    /// <param name="parameter">number as string</param>
    /// <param name="culture">--</param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int i && int.TryParse(parameter?.ToString(), out int p))
        {
            return i == p;
        }
        else
        {
            return false;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
