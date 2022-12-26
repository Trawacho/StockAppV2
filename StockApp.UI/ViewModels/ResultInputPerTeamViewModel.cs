using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StockApp.UI.ViewModels;

public class ResultInputPerTeamViewModel : ViewModelBase
{
    private ITeam _selectedTeam;
    public ITeam SelectedTeam
    {
        get => _selectedTeam;
        set => SetProperty(
                ref _selectedTeam,
                value,
                () => SetPointsPerTeam());
    }

    private void SetPointsPerTeam()
    {
        PointsPerTeam.Clear();

        if (SelectedTeam == null) return;

        foreach (var game in SelectedTeam?.Games?.OrderBy(g => g.GameNumberOverAll))
        {
            PointsPerTeam.Add(new PointsPerTeamAndGameViewModel(game, SelectedTeam));
        }

    }

    public ObservableCollection<ITeam> Teams { get; } = new();

    public ObservableCollection<PointsPerTeamAndGameViewModel> PointsPerTeam { get; } = new();


    public ResultInputPerTeamViewModel(IEnumerable<ITeam> teams)
    {
        foreach (var team in teams)
        {
            Teams.Add(team);
        }

        SelectedTeam = Teams?.FirstOrDefault();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                SelectedTeam = null;
                Teams?.Clear();
                PointsPerTeam?.DisposeAndClear();
            }
            _disposed = true;
        }
    }


    public class PointsPerTeamAndGameViewModel : ViewModelBase
    {
        private void SpielstandChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(StockPunkte));
            RaisePropertyChanged(nameof(StockPunkteGegner));
        }

        private readonly IGame _game;
        private readonly ITeam _team;

        public int GameNumber => _game.GameNumberOverAll;
        public string Gegner
        {
            get
            {
                if (IsBreakGame)
                    return "Setzt aus";

                return _team.StartNumber == _game.TeamA.StartNumber
                    ? _game.TeamB.TeamName
                    : _game.TeamA.TeamName;
            }
        }

        public bool IsBreakGame => _game.IsPauseGame();

        public int StockPunkte
        {
            get => _team.StartNumber == _game.TeamA.StartNumber ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;
            set
            {
                if (_game.IsPauseGame()) return;
                if (_team.StartNumber == _game.TeamA.StartNumber)
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

        public int StockPunkteGegner
        {
            get => _team.StartNumber != _game.TeamA.StartNumber ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;
            set
            {
                if (_game.IsPauseGame()) return;
                if (_team.StartNumber != _game.TeamA.StartNumber)
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

        public PointsPerTeamAndGameViewModel(IGame game, ITeam team)
        {
            _game = game;
            _team = team;
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