﻿using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Core.Models;

public class RankedTeamModel
{
    private readonly ITeam _team;
    private readonly bool _printNameOfPlayer;
    private readonly bool _live;
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
    public string TeamName => _team.TeamName.Trim();
    public string SpielPunkte => $"{_team.GetSpielPunkte(_live).positiv}:{_team.GetSpielPunkte(_live).negativ}";
    public string StockPunkte => $"{_team.GetStockPunkte(_live).positiv}:{_team.GetStockPunkte(_live).negativ}";
    public string StockNote => _team.GetStockNote(_live).ToString("F3");
    public string StockPunkteDifferenz => $"{_team.GetStockPunkteDifferenz(_live)}";
    public bool HasPlayerNames => _printNameOfPlayer;
    public RankedTeamModel(int rank, ITeam team, bool printNameOfPlayer, bool live)
    {
        _rank = rank;
        _team = team;
        _printNameOfPlayer = printNameOfPlayer;
        _live = live;
    }

}