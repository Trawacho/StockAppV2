using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Models;
using StockApp.UI.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StockApp.UI.ViewModels;

internal class ResultInputAfterGameWithKehreViewModel : ViewModelBase
{
    private readonly IEnumerable<IGame> _allGames;
    private readonly bool _has8Turns;
    private IGame _selectedGame;

    public ResultInputAfterGameWithKehreViewModel(IEnumerable<IGame> allGames, bool has8Turns)
    {
        _allGames = allGames;
        _has8Turns = has8Turns;
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
                KehrenPerGame?.DisposeAndClear();
            }
            _disposed = true;
            base.Dispose(disposing);
        }
    }

    public bool Has8Turns => _has8Turns;
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
    public string SelectedGameName => $"Runde: {SelectedGame?.RoundOfGame} | Spiel: {SelectedGame?.GameNumber}";
    public ObservableCollection<IGame> Games { get; } = new();
    public ObservableCollection<KehrenPerGameViewModel> KehrenPerGame { get; } = new();
    private void SetPointsPerGame()
    {
        foreach (var game in KehrenPerGame)
            game.Dispose();

        KehrenPerGame.Clear();
        if (SelectedGame == null) return;

        foreach (var game in _allGames
                                    .Where(g => g.RoundOfGame == SelectedGame.RoundOfGame
                                             && g.GameNumber == SelectedGame.GameNumber
                                             && !g.IsPauseGame())
                                    .OrderBy(o => o.CourtNumber))
        {
            KehrenPerGame.Add(new KehrenPerGameViewModel(game));
        }
    }


    public class KehrenPerGameViewModel : KehrenBaseModel, INotifyPropertyChanged
    {
        public KehrenPerGameViewModel(IGame game) : base(game)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }
                _disposed = true;
            }
        }

        public int Bahn => _game.CourtNumber;

        public string TeamName1 => _game.IsTeamA_Starting ? _game.TeamA.TeamName : _game.TeamB.TeamName;

        public string TeamName2 => !_game.IsTeamA_Starting ? _game.TeamA.TeamName : _game.TeamB.TeamName;

        public override int StockPunkte1 => _game.IsTeamA_Starting ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;

        public override int StockPunkte2 => !_game.IsTeamA_Starting ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;

        protected override int GetKehre(int kehrenNummer, bool team1)
        {
            if (team1)
            {
                if (_game.IsTeamA_Starting)
                    return _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamA ?? 0;
                else
                    return _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamB ?? 0;
            }
            else
            {
                if (_game.IsTeamA_Starting)
                    return _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamB ?? 0;
                else
                    return _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamA ?? 0;
            }
        }

        protected override void SetKehre(int kehrenNummer, int value, bool team1, string propName1, [CallerMemberName] string propName2 = null)
        {
            if (value > 100) value = 0;

            if (team1)
            {
                if (_game.IsTeamA_Starting)
                    _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value, teamB: value != 0 ? 0 : int.MinValue);
                else
                    _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value != 0 ? 0 : int.MinValue, teamB: value);
            }
            else
            {
                if (_game.IsTeamA_Starting)
                    _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value != 0 ? 0 : int.MinValue, teamB: value);
                else
                    _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value, teamB: value != 0 ? 0 : int.MinValue);
            }

            RaisePropertyChanged(propName1);
            RaisePropertyChanged(propName2);
            RaisePropertyChanged(nameof(StockPunkte1));
            RaisePropertyChanged(nameof(StockPunkte2));
        }
    }
}