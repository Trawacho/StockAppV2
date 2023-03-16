using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Models;
using System.Collections.Generic;

namespace StockApp.Lib.ViewModels;

public class RankedTeamsTableViewModel : ViewModelBase
{
    private readonly ITeamBewerb _teamBewerb;
    private readonly bool _isLive;
    private readonly bool _showGroupName;

    public RankedTeamsTableViewModel()
    {
        IERVersion2022 = true;
    }

    public RankedTeamsTableViewModel(ITeamBewerb teamBewerb, bool isLive, bool showGroupName = false)
    {
        _teamBewerb = teamBewerb;
        _isLive = isLive;
        _showGroupName = showGroupName;
        int rank = 1;
        foreach (var t in _teamBewerb.GetTeamsRanked())
        {
            RankedTeams.Add(new RankedTeamModel(rank: rank, team: t, printNameOfPlayer: rank <= _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult, live: false));
            rank++;
        }
        IERVersion2022 = _teamBewerb.IERVersion == Core.Wettbewerb.Teambewerb.IERVersion.v2022;

    }
    public bool IERVersion2022 { get; init; }
    public IList<RankedTeamModel> RankedTeams { get; } = new List<RankedTeamModel>();
    public string GroupName => _showGroupName ? $"{_teamBewerb.Gruppenname}" : null;

}
