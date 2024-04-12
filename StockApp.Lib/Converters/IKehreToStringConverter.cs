using StockApp.Lib.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace StockApp.Lib.Converters;

public class IKehreToStringConverter : IValueConverter
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is MasterKehrenPerGameModel vmMaster)
        {
            try
            {
                //Split converter in Team and Turn
                var team1 = ((string)parameter).Substring(0, 1) == "A";
                var parmLength = parameter.ToString().Length;
                var turn = int.Parse(parameter.ToString().Substring(1, parmLength - 1));

                //Get Master Values
                var kehre = vmMaster.GetMasterKehre(turn);

                if (kehre == null) return "x";

                //If result is 0:0 return the 0;
                if (kehre.PunkteTeamA == 0 && kehre.PunkteTeamB == 0) return "0";

                //If value of team is equal to 0 reutrn "-"
                if (team1)
                    return kehre.PunkteTeamA == 0 ? $"-" : kehre.PunkteTeamA.ToString();
                else
                    return kehre.PunkteTeamB == 0 ? $"-" : kehre.PunkteTeamB.ToString();

            }
            catch
            {
                return new ArgumentException("Parameter needs at least two chars. First has to be A or B for team. Second has to be 1 ... 99 for turn");
            }
        }
        else if(value is LiveKehrenPerGameModel vmLive)
        {
            try
            {
                //Split converter in Team and Turn
                var team1 = ((string)parameter).Substring(0, 1) == "A";
                var parmLength = parameter.ToString().Length;
                var turn = int.Parse(parameter.ToString().Substring(1, parmLength - 1));

                //Get Master Values
                var kehre = vmLive.GetLiveKehre(turn);

                if (kehre == null) return "x";

                //If result is 0:0 return the 0;
                if (kehre.PunkteTeamA == 0 && kehre.PunkteTeamB == 0) return "0";

                //If value of team is equal to 0 reutrn "-"
                if (team1)
                    return kehre.PunkteTeamA == 0 ? $"-" : kehre.PunkteTeamA.ToString();
                else
                    return kehre.PunkteTeamB == 0 ? $"-" : kehre.PunkteTeamB.ToString();

            }
            catch
            {
                return new ArgumentException("Parameter needs at least two chars. First has to be A or B for team. Second has to be 1 ... 99 for turn");
            }
        }

        return new ArgumentException($"Value must be Type of {typeof(MasterKehrenPerGameModel)}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
