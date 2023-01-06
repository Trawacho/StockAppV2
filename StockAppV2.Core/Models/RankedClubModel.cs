using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Core.Models;

public class RankedClubModel : IComparable<RankedClubModel>
{
    private readonly string _clubname;
    internal readonly (int positiv, int negativ) _spielpunkte;
    internal readonly (int positiv, int negativ) _stockpunkte;
    internal readonly int _stockpunkteDifferenz;


    public int Rank { get; set; }
    public string ClubName => _clubname;
    public string SpielPunkte => $"{_spielpunkte.positiv}:{_spielpunkte.negativ}";
    public string StockPunkte => $"{_stockpunkte.positiv}:{_stockpunkte.negativ}";
    public string StockPunkteDifferenz => $"{_stockpunkteDifferenz}";
    public string StockNote => (_stockpunkte.negativ == 0
                                    ? _stockpunkte.positiv
                                    : Math.Round((double)_stockpunkte.positiv / _stockpunkte.negativ, 3)
                                ).ToString("F3");


    public RankedClubModel(IEnumerable<ITeam> teams, bool live = false)
    {
        _clubname = teams.First().TeamName;
        foreach (var team in teams)
        {
            _spielpunkte.positiv += team.GetSpielPunkte(live).positiv;
            _spielpunkte.negativ += team.GetSpielPunkte(live).negativ;

            _stockpunkte.positiv += team.GetStockPunkte(live).positiv;
            _stockpunkte.negativ += team.GetStockPunkte(live).negativ;
            _stockpunkteDifferenz += team.GetStockPunkteDifferenz(live);
        }
    }

    public int CompareTo(RankedClubModel other)
    {
        //Spielpunkte
        if (this._spielpunkte.positiv > other._spielpunkte.positiv)
            return 1;
        else if (this._spielpunkte.positiv < other._spielpunkte.positiv)
            return -1;
        else
        {
            //Differenz der Stockpunkte
            if (this._stockpunkteDifferenz > other._stockpunkteDifferenz)
                return 1;
            else if (this._stockpunkteDifferenz < other._stockpunkteDifferenz)
                return -1;
            else
            {
                //die höhere Anzahl an selber erreichten Stockpunkten
                if (this._stockpunkte.positiv > other._stockpunkte.positiv)
                    return 1;
                else if (this._stockpunkte.positiv < other._stockpunkte.positiv)
                    return -1;
                else
                {
                    return 0;
                }
            }
        }
    }
}
