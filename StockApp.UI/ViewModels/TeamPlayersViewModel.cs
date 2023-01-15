using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class TeamPlayersViewModel : ViewModelBase
{

    private TeamPlayerViewModel _selectedPlayer;
    private readonly ITeam _team;
    ICommand _addPlayerCommand;
    ICommand _removePlayerCommand;


    public ObservableCollection<TeamPlayerViewModel> TeamPlayers { get; }

    public TeamPlayerViewModel SelectedPlayer
    {
        get => _selectedPlayer;
        set => SetProperty(ref _selectedPlayer, value);
    }

    public ICommand AddPlayerCommand => _addPlayerCommand ??= new RelayCommand((p) => _team.AddPlayer(), (p) => _team.Players?.Count() < 6);
    public ICommand RemovePlayerCommand => _removePlayerCommand ??= new RelayCommand((p) => _team.RemovePlayer(SelectedPlayer.Player), (p) => SelectedPlayer != null && _team.Players?.Count() > 1);


    public TeamPlayersViewModel(ITeam team)
    {
        _team = team;

        TeamPlayers = new ObservableCollection<TeamPlayerViewModel>(_team.Players.Select(p => new TeamPlayerViewModel(p)));
        _team.PlayersChanged += TeamPlayersChanged;
    }

    private void TeamPlayersChanged(object sender, EventArgs e)
    {
        TeamPlayers.DisposeAndClear();

        foreach (var player in _team.Players) TeamPlayers.Add(new TeamPlayerViewModel(player));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _team.PlayersChanged -= TeamPlayersChanged;
                TeamPlayers?.DisposeAndClear();
                SelectedPlayer = null;
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
