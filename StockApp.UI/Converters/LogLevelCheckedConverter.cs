﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace StockApp.UI.Converters;

internal class LogLevelCheckedConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString().ToLower().Equals(parameter.ToString().ToLower());
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
