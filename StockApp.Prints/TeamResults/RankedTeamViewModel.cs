using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Prints.TeamResults;

public class RankedTeamViewModel
{
    private readonly ITeam _team;
    private readonly bool _printNameOfPlayer;
    public int Rank { get; init; }
    public string PlayerNames
    {
        get
        {
            if (_printNameOfPlayer)
            {
                string t = string.Empty;
                foreach (var item in _team.Players)
                {
                    t += string.IsNullOrWhiteSpace(t)
                        ? $"{item.LastName} {item.FirstName}"
                        : $", {item.LastName} {item.FirstName}";
                }
                return t;
            }
            else
            {
                return null;
            }
        }
    }
    public string TeamName => _team.TeamName.Trim();
    public string SpielPunkte => $"{_team.GetSpielPunkte().positiv}:{_team.GetSpielPunkte().negativ}";
    public string StockPunkte => $"{_team.GetStockPunkte().positiv}:{_team.GetStockPunkte().negativ}";
    public string StockPunkteDifferenz => $"{_team.GetStockPunkteDifferenz()}";
    public string StockNote => _team.GetStockNote().ToString("F3");

    public RankedTeamViewModel(int rank, ITeam team, bool printNameOfPlayer)
    {
        Rank = rank;
        _team = team;
        _printNameOfPlayer = printNameOfPlayer;
    }

}
