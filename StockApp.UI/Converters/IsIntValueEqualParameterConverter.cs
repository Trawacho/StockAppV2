using System;
using System.Globalization;
using System.Windows.Data;

namespace StockApp.UI.Converters;

public class IsIntValueEqualParameterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return int.TryParse( value.ToString(), out int v) && int.TryParse(parameter.ToString(), out int p) && v == p;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
