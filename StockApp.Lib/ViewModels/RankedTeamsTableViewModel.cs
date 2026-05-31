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
    private readonly IReadOnlyDictionary<ITeam, IList<IGame>> _adjustedGames;

    public RankedTeamsTableViewModel()
    {
        IERVersion2022 = true;
        ShowStockPunkte = true;
    }

    public RankedTeamsTableViewModel(ITeamBewerb teamBewerb, bool isLive, bool showGroupName = false, bool useParagraph610 = false)
    {
        _teamBewerb = teamBewerb;
        ShowStockPunkte = true;
        _isLive = isLive;
        _groupName = showGroupName ? $"{_teamBewerb.Gruppenname}" : null;
        _adjustedGames = useParagraph610 ? Paragraph610Evaluator.GetAdjustedGames(teamBewerb, isLive) : null;

        int rank = 1;
        foreach (var t in _teamBewerb.GetTeamsRanked())
        {
            RankedTeams.Add(new RankedTeamModel(rank: rank,
                                                team: t,
                                                printNameOfPlayer: rank <= _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult,
                                                live: false,
                                                aufAbSteiger: AufAbSteigerZeichen(rank),
                                                teamNameWithStartnumber: _teamBewerb.TeamNameWithStartnumber,
                                                filteredGames: _adjustedGames?.ContainsKey(t) == true ? ((List<IGame>)_adjustedGames[t]).AsReadOnly() : null));
            rank++;
        }
        IERVersion2022 = _teamBewerb.IERVersion == Core.Wettbewerb.Teambewerb.IERVersion.v2022;
    }

    public RankedTeamsTableViewModel(ITeamBewerb teamBewerb, bool isLive, bool isSplitGroupOne, bool showStockPunkte, bool useParagraph610 = false)
    {
        _teamBewerb = teamBewerb;
        _isLive = isLive;
        ShowStockPunkte = showStockPunkte;
        _groupName = isSplitGroupOne ? "Gruppe A" : "Gruppe B";
        _adjustedGames = useParagraph610 ? Paragraph610Evaluator.GetAdjustedGames(teamBewerb, isLive) : null;

        int rank = 1;
        foreach (var t in _teamBewerb.GetSplitTeamsRanked(isSplitGroupOne, isLive))
        {
            RankedTeams.Add(new RankedTeamModel(rank: rank,
                                                team: t,
                                                printNameOfPlayer: rank <= _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult,
                                                live: _isLive,
                                                aufAbSteiger: AufAbSteigerZeichen(rank),
                                                teamNameWithStartnumber: _teamBewerb.TeamNameWithStartnumber,
                                                filteredGames: _adjustedGames?.ContainsKey(t) == true ? ((List<IGame>)_adjustedGames[t]).AsReadOnly() : null));
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
        return rank <= _teamBewerb.AnzahlAufsteiger 
                        ? "↑"
                        : _teamBewerb.IsSplitGruppe 
                            ? (_teamBewerb.Teams.Count() / 2 ) - rank  < _teamBewerb.AnzahlAbsteiger 
                                ? "↓"
                                :string.Empty
                            : (_teamBewerb.Teams.Count() - rank) < _teamBewerb.AnzahlAbsteiger 
                                    ? "↓"
                                    : string.Empty;
    }
}
