using System;
using System.Globalization;
using System.Windows.Controls;

namespace StockApp.UI.ValidationRules;
public class GamesRoundRange : ValidationRule
{
    public int Min { get; set; }
    public int Max { get; set; }
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var rounds = 0;

        try
        {
            if (((string)value).Length > 0)
                rounds = int.Parse((string)value);
        }
        catch (Exception e)
        {
            return new ValidationResult(false, "Illegal characters or " + e.Message);
        }

        if ((rounds < Min) || (rounds > Max))
        {
            return new ValidationResult(false,
                "Please enter a value in the range: " + Min + " - " + Max + ".");
        }
        return new ValidationResult(true, null);
    }
}
