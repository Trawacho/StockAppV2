using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Lib.ViewModels;

public class RankedClubTableViewModel : ViewModelBase
{
    private readonly ITeamBewerb _teamBewerb;
    private readonly bool _isLive;

    public RankedClubTableViewModel()
    {

    }

    public RankedClubTableViewModel(ITeamBewerb teamBewerb, bool isLive)
    {
        _teamBewerb = teamBewerb;
        _isLive = isLive;

        AsDataGrid = false;
        IsIERVersion2022 = _teamBewerb?.IERVersion == IERVersion.v2022;

        var clubA = new RankedClubModel(_teamBewerb.Teams.Take(_teamBewerb.Teams.Count() / 2), live: _isLive);
        var clubB = new RankedClubModel(_teamBewerb.Teams.Skip(_teamBewerb.Teams.Count() / 2), live: _isLive);
        var compared = clubA.CompareTo(clubB);
        switch (compared)
        {
            case 0:
                clubA.Rank = 1;
                clubB.Rank = 1;
                break;
            case 1:
                clubA.Rank = 1;
                clubB.Rank = 2;
                break;
            case -1:
            default:
                clubA.Rank = 2;
                clubB.Rank = 1;
                break;
        }

        RankedClubs = new[] { clubA, clubB }.OrderBy(o => o.Rank).ToList();
    }

    public IList<RankedClubModel> RankedClubs { get; init; }
    
    public bool IsIERVersion2022 { get; init; }
    
    public bool AsDataGrid { get; set; }
}
