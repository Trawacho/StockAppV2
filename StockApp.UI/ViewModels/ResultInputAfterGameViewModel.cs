using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StockApp.UI.ViewModels;

public class ResultInputAfterGameViewModel : ViewModelBase
{
    private IGame _selectedGame;
    private readonly IEnumerable<IGame> _allGames;

    public IGame SelectedGame
    {
        get => _selectedGame;
        set => SetProperty(
            ref _selectedGame,
            value,
            () =>
            {
                RaisePropertyChanged(nameof(SelectedGameName));
                SetPointsPerGame();
            });
    }

    private void SetPointsPerGame()
    {
        foreach (var game in PointsPerGame)
            game.Dispose();

        PointsPerGame.Clear();
        if (SelectedGame == null) return;

        foreach (var game in _allGames
                                    .Where(g => g.RoundOfGame == SelectedGame.RoundOfGame
                                             && g.GameNumber == SelectedGame.GameNumber
                                             && !g.IsPauseGame())
                                    .OrderBy(o => o.CourtNumber))
        {
            PointsPerGame.Add(new PointsPerGameViewModel(game));
        }
    }

    public string SelectedGameName => $"Runde: {SelectedGame?.RoundOfGame} | Spiel: {SelectedGame?.GameNumber}";

    public ObservableCollection<IGame> Games { get; } = new();

    public ObservableCollection<PointsPerGameViewModel> PointsPerGame { get; } = new();

    public ResultInputAfterGameViewModel(IEnumerable<IGame> allGames)
    {
        _allGames = allGames;

        foreach (var game in allGames.Where(g => !g.IsPauseGame())
                                                .OrderBy(o => o.GameNumberOverAll)
                                                .GroupBy(x => x.GameNumberOverAll)
                                                .Select(group => group.First()))
        {
            Games.Add(game);
        }
        SelectedGame = Games?.FirstOrDefault();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                SelectedGame = null;
                Games?.Clear();
                PointsPerGame?.DisposeAndClear();
            }
            _disposed = true;
            base.Dispose(disposing);
        }
    }

    public class PointsPerGameViewModel : ViewModelBase
    {
        private void SpielstandChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(StockPunkte1));
            RaisePropertyChanged(nameof(StockPunkte2));
        }

        private readonly IGame _game;
        public int Bahn => _game.CourtNumber;

        public string TeamName1 => _game.IsTeamA_Starting ? _game.TeamA.TeamName : _game.TeamB.TeamName;

        public string TeamName2 => !_game.IsTeamA_Starting ? _game.TeamA.TeamName : _game.TeamB.TeamName;

        public int StockPunkte1
        {
            get => _game.IsTeamA_Starting ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;
            set
            {
                if (_game.IsTeamA_Starting)
                {
                    _game.Spielstand.SetMasterTeamAValue(value);
                }
                else
                {
                    _game.Spielstand.SetMasterTeamBValue(value);
                }
                RaisePropertyChanged();
            }
        }

        public int StockPunkte2
        {
            get => !_game.IsTeamA_Starting ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;
            set
            {
                if (!_game.IsTeamA_Starting)
                {
                    _game.Spielstand.SetMasterTeamAValue(value);
                }
                else
                {
                    _game.Spielstand.SetMasterTeamBValue(value);
                }
                RaisePropertyChanged();
            }
        }

        public PointsPerGameViewModel(IGame game)
        {
            _game = game;
            _game.SpielstandChanged += SpielstandChanged;
        }



        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _game.SpielstandChanged -= SpielstandChanged;
                }
                _disposed = true;
            }
        }
    }
}