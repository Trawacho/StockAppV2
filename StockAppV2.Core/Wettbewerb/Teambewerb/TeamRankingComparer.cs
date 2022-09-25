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

    private int CompareVersionUpFrom2022(ITeam x, ITeam y)
    {
        if (x.GetSpielPunkte(_isLive).positiv > y.GetSpielPunkte(_isLive).positiv)
            return -1;
        else if (x.GetSpielPunkte(_isLive).positiv < y.GetSpielPunkte(_isLive).positiv)
            return 1;
        else
        {
            if (x.GetStockPunkteDifferenz(_isLive) > y.GetStockPunkteDifferenz(_isLive))
                return -1;
            else if (x.GetStockPunkteDifferenz(_isLive) < y.GetStockPunkteDifferenz(_isLive))
                return 1;
            else
            {
                if (x.GetStockPunkte(_isLive).positiv > y.GetStockPunkte(_isLive).positiv)
                    return -1;
                else if (x.GetStockPunkte(_isLive).positiv < y.GetStockPunkte(_isLive).positiv)
                    return 1;
                else
                {
                    //direkter Vergleich
                    var gameXvsY = x.Games.First(g => g.TeamA.StartNumber == y.StartNumber || g.TeamB.StartNumber == y.StartNumber);
                    if (x.StartNumber == gameXvsY.TeamA.StartNumber)
                    {
                        if (gameXvsY.TeamA.GetStockPunkte().positiv > gameXvsY.TeamA.GetStockPunkte().negativ) return -1;
                        return 1;
                    }
                    else if (x.StartNumber == gameXvsY.TeamB.StartNumber)
                    {
                        if (gameXvsY.TeamA.GetStockPunkte().positiv < gameXvsY.TeamA.GetStockPunkte().negativ) return 1;
                        return -1;
                    }
                    else
                    {
                        //Wenn alles gleich, entscheidet das Los. 
                        var random = new Random().Next(int.MinValue, int.MaxValue);
                        if (random <= 0)
                            return -1;
                        else
                            return 1;
                    }
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

