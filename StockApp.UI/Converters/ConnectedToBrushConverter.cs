using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StockApp.UI.Converters;

internal class ConnectedToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Brushes.Green : Brushes.Red;
        else
            return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
