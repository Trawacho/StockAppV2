using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace StockApp.UI.Utilities;
/// <summary>
/// HelperKlasse für Enumerationen
/// </summary>
internal static class EnumUtil
{
    public static List<TEnum> GetEnumList<TEnum>() where TEnum : Enum
        => ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();

    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
        {
            return attributes.First().Description;
        }

        return value.ToString();
    }
}

