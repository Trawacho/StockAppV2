using StockApp.UI.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
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

internal class TestConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //System.Diagnostics.Debug.WriteLine(value.GetType());
        //System.Diagnostics.Debug.WriteLine(parameter?.GetType());
        //GroupSelectorViewModel vm = (GroupSelectorViewModel)((UserControl)value).DataContext;
        if (value is ListViewItem lvi)
            return lvi.IsSelected;

        return value;
        // System.Diagnostics.Debug.WriteLine(value.GetType());
        // System.Diagnostics.Debug.WriteLine(value.GetType());
        // return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //throw new NotImplementedException();
        return value;
    }
}

internal class TestMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is TeamBewerbViewModel a && values[1] is TeamBewerbViewModel b)
            return a.ID == b.ID;
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return new object[] { Binding.DoNothing, Binding.DoNothing };
    }
}
