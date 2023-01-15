using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Lib.ViewModels;

public class BestOfDetailViewModel : ViewModelBase
{
    private readonly ITeamBewerb _teamBewerb;
    private readonly bool _isLive;

    public BestOfDetailViewModel()
    {

    }


    public BestOfDetailViewModel(ITeamBewerb teamBewerb, bool isLive)
    {
        _teamBewerb = teamBewerb;
        _isLive = isLive;

        var bestOfDetails = new List<KehrenBaseModel>();

        foreach (var game in _teamBewerb.GetAllGames(false))
            bestOfDetails.Add(
                _isLive 
                ? new LiveKehrenPerGameModel(game) 
                : new MasterKehrenPerGameModel(game));

        BestOfDetails = bestOfDetails.AsEnumerable();


    }
    public string BestOfTeamA => _teamBewerb.Games.First().TeamA.TeamName;
    public string BestOfTeamB => _teamBewerb.Games.First().TeamB.TeamName;
    public bool Is8TurnsGame => _teamBewerb.Is8TurnsGame;
    public string AnspielWechselText => IsAnspielWechsel ? $"Das Anspiel wurde bei jedem Spiel gewechselt" : $"Das Anspiel wurde bei jedem Spiel NICHT gewechselt";
    public bool IsAnspielWechsel => _teamBewerb.StartingTeamChange;

    public IEnumerable<KehrenBaseModel> BestOfDetails { get; init; }

}
