using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class GamesViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private ICommand _createGamesCommand;
    private bool _isCreatingGames;

    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

    public ViewModelBase GamesPrintsViewModel { get; private set; }
    public bool IsCreatingGames { get => _isCreatingGames; set => SetProperty(ref _isCreatingGames, value); }

    public IEnumerable<IGameplan> Gameplans => _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == TeamBewerb.Teams.Count());

    #region TeamBewerb - Depended Properties

    public int CountOfGames => TeamBewerb.GetCountOfGames();
    public int CountOfGamesPerCourt => TeamBewerb.GetCountOfGamesPerCourt();
    public bool HasMoreGameRounds => SpielRunden > 1;
    public bool HasGames => CountOfGames > 0;
    public bool HasNoGames => !HasGames;

    public int SelectedGameplanId
    {
        get => TeamBewerb.GameplanId;
        set
        {
            if (TeamBewerb.GameplanId == value)
                return;

            TeamBewerb.GameplanId = value;
            RaisePropertyChanged();
        }
    }

    public bool HasChangeStart
    {
        get => TeamBewerb.StartingTeamChange;
        set
        {
            if (TeamBewerb.StartingTeamChange == value)
                return;

            TeamBewerb.StartingTeamChange = value;
            RaisePropertyChanged(nameof(HasChangeStart));
        }
    }

    public bool Has8Turns
    {
        get => TeamBewerb.Is8TurnsGame;
        set
        {
            if (TeamBewerb.Is8TurnsGame == value) return;
            TeamBewerb.Is8TurnsGame = value;
            RaisePropertyChanged();
        }
    }

    public int SpielRunden
    {
        get => TeamBewerb.NumberOfGameRounds;
        set
        {
            if (TeamBewerb.NumberOfGameRounds == value) return;
            TeamBewerb.NumberOfGameRounds = value;
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
            foreach (var t in TeamBewerb.Teams)
                t.ClearGames();

            TeamBewerb.IsSplitGruppe = Gameplans.FirstOrDefault(p => p.ID == SelectedGameplanId)?.IsSplit ?? false;

            GamePlanFactory.MatchTeamAndGames(Gameplans.First(g => g.ID == SelectedGameplanId), TeamBewerb.Teams, SpielRunden, HasChangeStart);

            IsCreatingGames = false;
        },
        (p) => !IsCreatingGames && SelectedGameplanId != 0
        );

    #region Constructor

    public GamesViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;

        GamesPrintsViewModel = new GamesPrintsViewModel(TeamBewerb, _turnierStore);

        TeamBewerb.GamesChanged += TeamBewerb_GamesChanged;
    }

    private void TeamBewerb_GamesChanged(object sender, System.EventArgs e)
    {
        RaisePropertyChanged(nameof(HasNoGames));
        RaisePropertyChanged(nameof(CountOfGames));
        RaisePropertyChanged(nameof(CountOfGamesPerCourt));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                TeamBewerb.GamesChanged -= TeamBewerb_GamesChanged;
                GamesPrintsViewModel?.Dispose();
                GamesPrintsViewModel = null;
            }
            _disposed = true;
        }
    }

    #endregion
}
