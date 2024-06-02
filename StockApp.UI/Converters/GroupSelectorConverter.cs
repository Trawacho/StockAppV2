using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StockApp.UI.Converters;

public class GroupSelectorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int groups = System.Convert.ToInt32(value);
        int parm = System.Convert.ToInt32(parameter);
        return groups >= parm
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
