using System.Text.RegularExpressions;

namespace StockApp.Comm.NetMqStockTV
{
    public class StockTVBegegnung
    {
        public StockTVBegegnung()
        {

        }
        public StockTVBegegnung(int spielNummer, string teamNameA, string teamNameB, bool isAnspielTeamA) : this()
        {
            SpielNummer = spielNummer;
            TeamNameA = teamNameA;
            TeamNameB = teamNameB;
            IsAnspielTeamA = isAnspielTeamA;
        }
        public int SpielNummer { get; set; }
        public string TeamNameA { get; set; }
        public string TeamNameB { get; set; }
        public bool IsAnspielTeamA { get; set; }

        /// <summary>
        /// Bereitet einen Namen für das StockTV-Format "{Nr}:{NameA}:{NameB};" vor:
        /// Die Trennzeichen ':' und ';' sowie Steuerzeichen (Zeilenumbrüche, Tabs) werden durch Leerzeichen ersetzt,
        /// mehrfache Leerzeichen zusammengefasst und der Name getrimmt. null ergibt einen leeren String.
        /// </summary>
        public static string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            var replaced = Regex.Replace(name, @"[:;\p{Cc}]", " ");
            return Regex.Replace(replaced, @"\s{2,}", " ").Trim();
        }

        public string GetStockTVString(bool nextCourtLeft)
        {
            var nameA = SanitizeName(TeamNameA);
            var nameB = SanitizeName(TeamNameB);

            if (IsAnspielTeamA && nextCourtLeft)
                return $"{SpielNummer}:{nameA} »:{nameB};";
            else if (!IsAnspielTeamA && nextCourtLeft)
                return $"{SpielNummer}:{nameA}:« {nameB};";


            else if (IsAnspielTeamA && !nextCourtLeft)
                return $"{SpielNummer}:« {nameA}:{nameB};";
            else if(!IsAnspielTeamA && !nextCourtLeft)
                return $"{SpielNummer}:{nameA}:{nameB} »;";

            else
                return $"{SpielNummer}:{nameA}:{nameB};";

        }
    }
}
