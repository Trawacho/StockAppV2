using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Lib.Models;

public class RankedTeamModel
{
    private readonly ITeam _team;
    private readonly bool _printNameOfPlayer;
    private readonly bool _live;
    private readonly string _aufAbSteiger;
    private readonly bool _teamNameWithStartnumber;
    private readonly int _rank;

    public int Rank => _rank;
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
    public string TeamName => _teamNameWithStartnumber 
        ? "(" + _team.StartNumber + ") " + _team.TeamNamePublic.Trim()
        : _team.TeamNamePublic.Trim();
    public string SpielPunkte => $"{_team.GetSpielPunkte(_live).positiv}:{_team.GetSpielPunkte(_live).negativ}";
    public string StockPunkte => $"{_team.GetStockPunkte(_live).positiv}:{_team.GetStockPunkte(_live).negativ}";
    public string StockNote => _team.GetStockNote(_live).ToString("F3");
    public string StockPunkteDifferenz => $"{_team.GetStockPunkteDifferenz(_live)}";
    public bool HasPlayerNames => _printNameOfPlayer && !string.IsNullOrWhiteSpace(PlayerNames);
    public string AufAbSteiger => _aufAbSteiger;

    public RankedTeamModel(int rank, ITeam team, bool printNameOfPlayer, bool live, string aufAbSteiger = "", bool teamNameWithStartnumber = false)
    {
        _rank = rank;
        _team = team;
        _printNameOfPlayer = printNameOfPlayer;
        _live = live;
        _aufAbSteiger = aufAbSteiger;
        _teamNameWithStartnumber = teamNameWithStartnumber;
    }

}
