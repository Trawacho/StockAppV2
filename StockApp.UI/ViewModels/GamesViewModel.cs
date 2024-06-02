using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class GamesViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private ICommand _createGamesCommand;
    private bool _isCreatingGames;
    private ITeamBewerb _currentTeamBewerb;
    private ViewModelBase _gamesPrintsViewModel;
    private IEnumerable<IGameplan> _gameplans;

    private ITeamBewerb CurrentTeamBewerb
    {
        get => _currentTeamBewerb;
        set
        {
            if (_currentTeamBewerb != null)
                _currentTeamBewerb.GamesChanged -= TeamBewerb_GamesChanged;

            SetProperty(ref _currentTeamBewerb, value);
            if (value != null)
                GamesPrintsViewModel = new GamesPrintsViewModel(CurrentTeamBewerb, _turnierStore);

            if (value != null)
                _currentTeamBewerb.GamesChanged += TeamBewerb_GamesChanged;
        }
    }

    public ViewModelBase GamesPrintsViewModel
    {
        get => _gamesPrintsViewModel;
        private set
        {
            SetProperty(ref _gamesPrintsViewModel, value);
        }
    }
    public bool IsCreatingGames { get => _isCreatingGames; set => SetProperty(ref _isCreatingGames, value); }

    public IEnumerable<IGameplan> Gameplans
    {
        get => _gameplans;
        private set
        {
            SetProperty(ref _gameplans, value);
        }
    }

    #region TeamBewerb - Depended Properties

    public int CountOfGames => CurrentTeamBewerb.GetCountOfGames();
    public int CountOfGamesPerCourt => CurrentTeamBewerb.GetCountOfGamesPerCourt();
    public bool HasMoreGameRounds => SpielRunden > 1;
    public bool HasGames => CountOfGames > 0;
    public bool HasNoGames => !HasGames;

    public int SelectedGameplanId
    {
        get
        {
            if (Gameplans.Any(g => g.ID == CurrentTeamBewerb.GameplanId))
                return CurrentTeamBewerb.GameplanId;
            else
                return 0;
        }
        set
        {
            if (CurrentTeamBewerb.GameplanId == value)
                return;

            CurrentTeamBewerb.GameplanId = value;
            RaisePropertyChanged();
        }
    }

    public bool HasChangeStart
    {
        get => CurrentTeamBewerb.StartingTeamChange;
        set
        {
            if (CurrentTeamBewerb.StartingTeamChange == value)
                return;

            CurrentTeamBewerb.StartingTeamChange = value;
            RaisePropertyChanged(nameof(HasChangeStart));
        }
    }

    public bool Has8Turns
    {
        get => CurrentTeamBewerb.Is8TurnsGame;
        set
        {
            if (CurrentTeamBewerb.Is8TurnsGame == value) return;
            CurrentTeamBewerb.Is8TurnsGame = value;
            RaisePropertyChanged();
        }
    }

    public int SpielRunden
    {
        get => CurrentTeamBewerb.NumberOfGameRounds;
        set
        {
            if (CurrentTeamBewerb.NumberOfGameRounds == value) return;
            CurrentTeamBewerb.NumberOfGameRounds = value;
            RaisePropertyChanged(nameof(SpielRunden));
            RaisePropertyChanged(nameof(HasMoreGameRounds));
        }
    }

    #endregion

    public ICommand CreateGamesCommand => _createGamesCommand ??= new RelayCommand
        ((p) =>
        {
            IsCreatingGames = true;

            //Entferne alle Spiele von allen Teams
            foreach (var t in CurrentTeamBewerb.Teams)
                t.ClearGames();

            CurrentTeamBewerb.IsSplitGruppe = Gameplans.FirstOrDefault(p => p.ID == SelectedGameplanId)?.IsSplit ?? false;

            GamePlanFactory.MatchTeamAndGames(Gameplans.FirstOrDefault(g => g.ID == SelectedGameplanId), CurrentTeamBewerb.Teams, SpielRunden, HasChangeStart);

            IsCreatingGames = false;
        },
        (p) => !IsCreatingGames && SelectedGameplanId != 0
        );

    #region Constructor

    public GamesViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged += CurrentTeamBewerbChangend;

        CurrentTeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

        Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Count() == 1
                          ? Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == CurrentTeamBewerb.Teams.Count())
                          : Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == CurrentTeamBewerb.Teams.Count() && !t.IsSplit);
    }

    private void CurrentTeamBewerbChangend(object sender, EventArgs e)
    {
        CurrentTeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
        Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Count() == 1
                         ? Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == CurrentTeamBewerb.Teams.Count())
                         : Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == CurrentTeamBewerb.Teams.Count() && !t.IsSplit);

        TeamBewerb_GamesChanged(sender, e);
    }

    private void TeamBewerb_GamesChanged(object sender, System.EventArgs e)
    {
        RaisePropertyChanged(nameof(HasNoGames));
        RaisePropertyChanged(nameof(CountOfGames));
        RaisePropertyChanged(nameof(CountOfGamesPerCourt));
        RaisePropertyChanged(nameof(SpielRunden));
        RaisePropertyChanged(nameof(HasMoreGameRounds));
        RaisePropertyChanged(nameof(HasGames));
        RaisePropertyChanged(nameof(SelectedGameplanId));
        RaisePropertyChanged(nameof(HasChangeStart));
        RaisePropertyChanged(nameof(Has8Turns));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged -= CurrentTeamBewerbChangend;
                CurrentTeamBewerb.GamesChanged -= TeamBewerb_GamesChanged;
                GamesPrintsViewModel?.Dispose();
                GamesPrintsViewModel = null;
            }
            _disposed = true;
        }
    }

    #endregion
}
