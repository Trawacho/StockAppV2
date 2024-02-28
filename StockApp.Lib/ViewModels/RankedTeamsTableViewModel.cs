using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Lib.ViewModels;

public class RankedTeamsTableViewModel : ViewModelBase
{
    private readonly ITeamBewerb _teamBewerb;
    private readonly bool _isLive;
    private readonly string _groupName;

    public RankedTeamsTableViewModel()
    {
        IERVersion2022 = true;
    }

    public RankedTeamsTableViewModel(ITeamBewerb teamBewerb, bool isLive, bool showGroupName = false)
    {
        _teamBewerb = teamBewerb;
        ShowStockPunkte = true;
        _isLive = isLive;
        _groupName = showGroupName ? $"{_teamBewerb.Gruppenname}" : null;
        int rank = 1;
        foreach (var t in _teamBewerb.GetTeamsRanked())
        {
            RankedTeams.Add(new RankedTeamModel(rank: rank,
                                                team: t,
                                                printNameOfPlayer: rank <= _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult,
                                                live: false,
                                                aufAbSteiger: AufAbSteigerZeichen(rank)));
            rank++;
        }
        IERVersion2022 = _teamBewerb.IERVersion == Core.Wettbewerb.Teambewerb.IERVersion.v2022;

    }

    public RankedTeamsTableViewModel(ITeamBewerb teamBewerb, bool isLive, bool isSplitGroupOne, bool showStockPunkte)
    {
        _teamBewerb = teamBewerb;
        _isLive = isLive;
        ShowStockPunkte = showStockPunkte;
        _groupName = isSplitGroupOne ? "Gruppe A" : "Gruppe B";

        int rank = 1;
        foreach (var t in _teamBewerb.GetSplitTeamsRanked(isSplitGroupOne, isLive))
        {
            RankedTeams.Add(new RankedTeamModel(rank: rank,
                                                team: t,
                                                printNameOfPlayer: rank <= _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult,
                                                live: _isLive,
                                                aufAbSteiger: AufAbSteigerZeichen(rank)));
            rank++;
        }
        IERVersion2022 = _teamBewerb.IERVersion == Core.Wettbewerb.Teambewerb.IERVersion.v2022;

    }

    public bool IERVersion2022 { get; init; }
    public IList<RankedTeamModel> RankedTeams { get; } = new List<RankedTeamModel>();
    public string GroupName => _groupName;

    public bool ShowStockPunkte { get; init; }

    private string AufAbSteigerZeichen(int rank)
    {
        return rank <= _teamBewerb.AnzahlAufsteiger ? "↑"
                        : (_teamBewerb.Teams.Count() - rank) < _teamBewerb.AnzahlAbsteiger ? "↓"
                        : string.Empty;
    }
}
