using StockApp.Core.Wettbewerb.Teambewerb;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Lib.Models;

public class RankedTeamModel
{
    private readonly ITeam _team;
    private readonly bool _printNameOfPlayer;
    private readonly bool _live;
    private readonly string _aufAbSteiger;
    private readonly bool _teamNameWithStartnumber;
    private readonly int _rank;
    private readonly IReadOnlyCollection<IGame> _filteredGames;

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

    public string SpielPunkte
    {
        get
        {
            var (pos, neg) = GetSpielPunkteFiltered();
            return $"{pos}:{neg}";
        }
    }

    public string StockPunkte
    {
        get
        {
            var (pos, neg) = GetStockPunkteFiltered();
            return $"{pos}:{neg}";
        }
    }

    public string StockNote
    {
        get
        {
            var (pos, neg) = GetStockPunkteFiltered();
            double note = neg == 0 ? pos : System.Math.Round((double)pos / neg, 3);
            return note.ToString("F3");
        }
    }

    public string StockPunkteDifferenz
    {
        get
        {
            var (pos, neg) = GetStockPunkteFiltered();
            return $"{pos - neg}";
        }
    }

    public bool HasPlayerNames => _printNameOfPlayer && !string.IsNullOrWhiteSpace(PlayerNames);
    public string AufAbSteiger => _aufAbSteiger;
    public string TeamInfo(TeamInfo teamInfo)
    {
		return teamInfo switch
		{
			Core.Wettbewerb.Teambewerb.TeamInfo.Keine => string.Empty,
			Core.Wettbewerb.Teambewerb.TeamInfo.Kreis => _team.Kreis,
			Core.Wettbewerb.Teambewerb.TeamInfo.Bundesland => _team.Bundesland,
			Core.Wettbewerb.Teambewerb.TeamInfo.Region => _team.Region,
			Core.Wettbewerb.Teambewerb.TeamInfo.Nation => _team.Nation,
			_ => string.Empty,
		};
	}

    public RankedTeamModel(int rank, ITeam team, bool printNameOfPlayer, bool live, string aufAbSteiger = "", bool teamNameWithStartnumber = false, IReadOnlyCollection<IGame> filteredGames = null)
    {
        _rank = rank;
        _team = team;
        _printNameOfPlayer = printNameOfPlayer;
        _live = live;
        _aufAbSteiger = aufAbSteiger;
        _teamNameWithStartnumber = teamNameWithStartnumber;
        _filteredGames = filteredGames;
    }

    private (int positiv, int negativ) GetSpielPunkteFiltered()
    {
        var games = _filteredGames ?? _team.Games;
        int pos = games.Where(g => g.TeamA == _team && g.TeamB.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetSpielPunkteTeamA(_live)) +
                games.Where(g => g.TeamB == _team && g.TeamA.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetSpielPunkteTeamB(_live));

        int neg = games.Where(g => g.TeamA != _team && g.TeamA.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetSpielPunkteTeamA(_live)) +
                games.Where(g => g.TeamB != _team && g.TeamB.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetSpielPunkteTeamB(_live));

        return _team.TeamStatus == TeamStatus.Normal ? (pos - _team.StrafSpielpunkte, neg) : (0, 0);
    }

    private (int positiv, int negativ) GetStockPunkteFiltered()
    {
        var games = _filteredGames ?? _team.Games;
        int pos = games.Where(g => g.TeamA == _team && g.TeamB.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetStockPunkteTeamA(_live)) +
                games.Where(g => g.TeamB == _team && g.TeamA.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetStockPunkteTeamB(_live));

        int neg = games.Where(g => g.TeamA != _team && g.TeamA.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetStockPunkteTeamA(_live)) +
                games.Where(g => g.TeamB != _team && g.TeamB.TeamStatus == TeamStatus.Normal)
                .Sum(s => s.Spielstand.GetStockPunkteTeamB(_live));

        return _team.TeamStatus == TeamStatus.Normal ? (pos, neg) : (0, 0);
    }

}
