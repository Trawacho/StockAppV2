namespace StockApp.Core.Wettbewerb.Teambewerb;

public partial class TeamRankingComparer : IComparer<ITeam>
{

    public TeamRankingComparer(bool isLive, IERVersion version)
    {
        _isLive = isLive;
        _version = version;
    }

    private bool _isLive;
    private IERVersion _version;

    public int Compare(ITeam x, ITeam y)
    {

        switch (_version)
        {
            case IERVersion.v2018:
                return CompareVersionUpFrom2018(x, y);
            case IERVersion.v2022:
            default:
                return CompareVersionUpFrom2022(x, y);
        }
    }

    /// <summary>
    /// führt den direkten Vergleich zweier Mannschaften durch
    /// </summary>
    /// <param name="gamesTeamX">Alle Spiele von Mannschaft X</param>
    /// <param name="startNumberX">Startnummer von Mannschaft X</param>
    /// <param name="startNumberY">Startnummer von Mannschaft Y</param>
    /// <param name="isLive">Live oder Master - Werte bewerten</param>
    /// <returns>-1 wenn X vor Y, 1 wenn Y vor X. 0 bei Gleichstand.</returns>
    internal int CompareDirekterVergleich(IEnumerable<IGame> gamesTeamX, int startNumberX, int startNumberY, bool isLive)
    {

        int spielpunkteX = 0;
        int spielpunkteY = 0;
        int stockpunkteX = 0;
        int stockpunkteY = 0;

        foreach (var game in gamesTeamX.Where(g => g.TeamA.StartNumber == startNumberY || g.TeamB.StartNumber == startNumberY))  //alle Spiele von gamesTeamX reduziert auf die Spiele mit Gegner Y
        {
            stockpunkteX += game.TeamA.StartNumber == startNumberX
                                ? game.Spielstand.GetStockPunkteTeamA(isLive)
                                : game.Spielstand.GetStockPunkteTeamB(isLive);

            stockpunkteY += game.TeamA.StartNumber == startNumberX
                                ? game.Spielstand.GetStockPunkteTeamB(isLive)
                                : game.Spielstand.GetStockPunkteTeamA(isLive);

            spielpunkteX += game.TeamA.StartNumber == startNumberX
                                ? game.Spielstand.GetSpielPunkteTeamA(isLive)
                                : game.Spielstand.GetSpielPunkteTeamB(isLive);

            spielpunkteY += game.TeamA.StartNumber == startNumberX
                                ? game.Spielstand.GetSpielPunkteTeamB(isLive)
                                : game.Spielstand.GetSpielPunkteTeamA(isLive);
        }

        return spielpunkteX > spielpunkteY
                 ? -1
                 : spielpunkteX < spielpunkteY
                     ? 1
                     : stockpunkteX > stockpunkteY
                         ? -1
                         : stockpunkteX < stockpunkteY
                             ? 1
                             : 0;

    }

    private int GetRandom()
    {
        return (new Random().Next(int.MinValue, int.MaxValue) <= 0) ? -1 : 1;
    }

    private int CompareVersionUpFrom2022(ITeam x, ITeam y)
    {
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
                    var direkterVergleich = CompareDirekterVergleich(x.Games, x.StartNumber, y.StartNumber, _isLive);
                    return direkterVergleich != 0 
                        ? direkterVergleich 
                        : GetRandom();
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
                    return 0;  //Wenn alles gleich, dann werden beide Mannschaften auf den selben Rang gesetzt
            }
        }
    }
}

