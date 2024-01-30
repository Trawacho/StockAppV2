namespace StockApp.Core.Wettbewerb.Teambewerb;

public class DirectCompareException : Exception
{
    private readonly string _message;
    public DirectCompareException(string message)
    {
        _message = message;
    }
    public override string Message => _message;
}

public partial class TeamRankingComparer : IComparer<ITeam>
{
    private readonly bool _isLive;
    private readonly IERVersion _version;

    public TeamRankingComparer(bool isLive, IERVersion version)
    {
        _isLive = isLive;
        _version = version;
    }

    public int Compare(ITeam x, ITeam y)
    {
        return _version switch
        {
            IERVersion.v2018 => CompareVersionUpFrom2018(x, y),
            _ => CompareVersionUpFrom2022(x, y),
        };
    }

    /// <summary>
    /// Direkter Vergleich zweier Mannschaften<br></br>
    /// Bei Mehrfachrunden wird das letzt Spiel bewertet
    /// </summary>
    /// <param name="gamesTeamX">Alle Spiele von Mannschaft X</param>
    /// <param name="startNumberX">Startnummer von Mannschaft X</param>
    /// <param name="startNumberY">Startnummer von Mannschaft Y</param>
    /// <param name="isLive">Live oder Master-Werte bewerten</param>
    /// <returns>-1 wenn X vor Y, 1 wenn Y vor X. 0 bei Gleichstand</returns>
    internal static int CompareLastGame(IEnumerable<IGame> gamesTeamX, int startNumberX, int startNumberY, bool isLive)
    {
        var game = gamesTeamX
                        .OrderByDescending(g => g.RoundOfGame)
                        .FirstOrDefault(g => g.TeamA.StartNumber == startNumberY || g.TeamB.StartNumber == startNumberY);

        if (game == null) return 0; //Bei Vergleichsturnieren, wird nicht jeder gegen jeden gespielt.

        if (game.TeamA.StartNumber == startNumberX)
        {
            return game.Spielstand.GetStockPunkteTeamA(isLive) > game.Spielstand.GetStockPunkteTeamB(isLive)
                ? -1
                : game.Spielstand.GetStockPunkteTeamA(isLive) < game.Spielstand.GetStockPunkteTeamB(isLive)
                    ? 1
                    : game.Spielstand.GetCountOfWinningTurnsTeamA(isLive) > game.Spielstand.GetCountOfWinningTurnsTeamB(isLive)
                        ? -1
                        : game.Spielstand.GetCountOfWinningTurnsTeamA(isLive) < game.Spielstand.GetCountOfWinningTurnsTeamB(isLive)
                            ? 1
                            : 0;

        }
        else
        {
            return game.Spielstand.GetStockPunkteTeamB(isLive) > game.Spielstand.GetStockPunkteTeamA(isLive)
                ? -1
                : game.Spielstand.GetStockPunkteTeamB(isLive) < game.Spielstand.GetStockPunkteTeamA(isLive)
                    ? 1
                    : game.Spielstand.GetCountOfWinningTurnsTeamB(isLive) > game.Spielstand.GetStockPunkteTeamA(isLive)
                        ? -1
                        : game.Spielstand.GetCountOfWinningTurnsTeamB(isLive) < game.Spielstand.GetStockPunkteTeamA(isLive)
                            ? 1
                            : 0;
        }
    }
    private int CompareVersionUpFrom2022(ITeam x, ITeam y)
    {
        //ausgeschiedene Mannschaften
        if (x.TeamStatus != TeamStatus.Normal && y.TeamStatus == TeamStatus.Normal)
            return 1;


        //Spielpunkte
        if (x.GetSpielPunkte(_isLive).positiv > y.GetSpielPunkte(_isLive).positiv)
            return -1;
        else if (x.GetSpielPunkte(_isLive).positiv < y.GetSpielPunkte(_isLive).positiv)
            return 1;
        else
        {
            //Differenz der Stockpunkte
            if (x.GetStockPunkteDifferenz(_isLive) > y.GetStockPunkteDifferenz(_isLive))
                return -1;
            else if (x.GetStockPunkteDifferenz(_isLive) < y.GetStockPunkteDifferenz(_isLive))
                return 1;
            else
            {
                //die höhere Anzahl an selber erreichten Stockpunkten
                if (x.GetStockPunkte(_isLive).positiv > y.GetStockPunkte(_isLive).positiv)
                    return -1;
                else if (x.GetStockPunkte(_isLive).positiv < y.GetStockPunkte(_isLive).positiv)
                    return 1;
                else
                {
                    //direkter Vergleich
                    var direkterVergleich = CompareLastGame(x.Games, x.StartNumber, y.StartNumber, _isLive);

                    if (direkterVergleich != 0) return direkterVergleich;
                    //kein Los-Entscheid!!
                    //die Startnummer soll entscheiden
                    return x.StartNumber < y.StartNumber ? -1 : 1;
                }
            }
        }
    }

    private int CompareVersionUpFrom2018(ITeam x, ITeam y)
    {
        if (x.GetSpielPunkte(_isLive).positiv > y.GetSpielPunkte(_isLive).positiv)
            return -1;
        else if (x.GetSpielPunkte(_isLive).positiv < y.GetSpielPunkte(_isLive).positiv)
            return 1;
        else
        {
            if (x.GetStockNote(_isLive) > y.GetStockNote(_isLive))
                return -1;
            else if (x.GetStockNote(_isLive) < y.GetStockNote(_isLive))
                return 1;
            else
            {
                if (x.GetStockPunkteDifferenz(_isLive) > y.GetStockPunkteDifferenz(_isLive))
                    return -1;
                else if (x.GetStockPunkteDifferenz(_isLive) < y.GetStockPunkteDifferenz(_isLive))
                    return 1;
                else
                {
                    var direkterVergleich = CompareLastGame(x.Games, x.StartNumber, y.StartNumber, _isLive);
                    if (direkterVergleich != 0) return direkterVergleich;

                    //die Startnummer soll entscheiden
                    return x.StartNumber < y.StartNumber ? -1 : 1;
                }
            }
        }
    }
}

