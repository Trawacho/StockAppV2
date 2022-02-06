namespace StockApp.UI.ViewModels;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System;
using System.Linq;
using System.Windows.Input;

public class GamesViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private readonly ITeamBewerb _teamBewerb;
    private ICommand _createGamesCommand;

    public ViewModelBase GamesPrintsViewModel { get; set; }


    public int CountOfCourts => _teamBewerb.NumberOfCourts;
    public int CountOfGames => _teamBewerb.GetCountOfGames();
    public int CountOfGamesPerCourt => _teamBewerb.GetCountOfGamesPerCourt();
    public bool CountOfNonVirtualTeamsIsEqual => _teamBewerb.Teams.Where(t => !t.IsVirtual).Count() % 2 == 0;
    public bool HasMoreGameRounds => SpielRunden > 1;
    public bool HasGames => CountOfGames > 0;
    public bool HasNoGames => !HasGames;
    public int GameGroup
    {
        get => _teamBewerb.SpielGruppe;
        set
        {
            if (_teamBewerb.SpielGruppe == value) return;
            _teamBewerb.SpielGruppe = value;
            RaisePropertyChanged();
        }
    }

    public bool HasTwoBreaks
    {
        get => _teamBewerb.TwoPauseGames;
        set
        {
            if (_teamBewerb.TwoPauseGames == value) return;
            _teamBewerb.TwoPauseGames = value;
            RaisePropertyChanged(nameof(HasTwoBreaks));
        }
    }

    public bool HasChangeStart
    {
        get => _teamBewerb.StartingTeamChange;
        set
        {
            if (_teamBewerb.StartingTeamChange == value) return;
            _teamBewerb.StartingTeamChange = value;
            RaisePropertyChanged(nameof(HasChangeStart));
        }
    }

    public int SpielRunden
    {
        get => _teamBewerb.NumberOfGameRounds;
        set
        {
            if (_teamBewerb.NumberOfGameRounds == value) return;
            _teamBewerb.NumberOfGameRounds = value;
            RaisePropertyChanged(nameof(SpielRunden));
            RaisePropertyChanged(nameof(HasMoreGameRounds));
        }
    }

    private bool _isCreatingGames;
    public bool IsCreatingGames
    {
        get => _isCreatingGames;
        set => SetProperty(ref _isCreatingGames, value);
    }

#pragma warning disable IDE0052 // Remove unread private members
    private IOrderedEnumerable<IFactoryGame> _factoryGames; //muss noch implementiert werden, aktuell werden die Spiele noch im TeamBewerb erstellt.
#pragma warning restore IDE0052 // Remove unread private members


    public ICommand CreateGamesCommand => _createGamesCommand ??= new RelayCommand
        ((p) =>
        {
            IsCreatingGames = true;
            _teamBewerb.CreateGames();
            _factoryGames = GameFactory.CreateGames(9, false, 1, false)
                                      .OrderBy(g => g.GameNumberOverAll)
                                      .ThenBy(g => g.CourtNumber);


            IsCreatingGames = false;
        },
        (p) => !IsCreatingGames || _teamBewerb.Teams.Count() >= 2
        );



    public GamesViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        _teamBewerb = turnierStore.Turnier.Wettbewerb as ITeamBewerb;
        _teamBewerb.GamesChanged += GamesChanged;
        _teamBewerb.TeamsChanged += TeamsChanged;

        GamesPrintsViewModel = new GamesPrintsViewModel(_teamBewerb, _turnierStore);
    }

    private void TeamsChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(CountOfNonVirtualTeamsIsEqual));
    }

    private void GamesChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(CountOfCourts));
        RaisePropertyChanged(nameof(CountOfGames));
        RaisePropertyChanged(nameof(HasGames));
        RaisePropertyChanged(nameof(HasNoGames));
        RaisePropertyChanged(nameof(CountOfGamesPerCourt));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _teamBewerb.GamesChanged -= GamesChanged;
                _teamBewerb.TeamsChanged -= TeamsChanged;

                GamesPrintsViewModel?.Dispose();
                GamesPrintsViewModel = null;
            }
            _disposed = true;
        }
    }
}
