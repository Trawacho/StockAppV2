using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
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
    private IEnumerable<IFactoryGame> _factoryGames;
    private bool _isCreatingGames;

    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

    public ViewModelBase GamesPrintsViewModel { get; private set; }
    public bool IsCreatingGames { get => _isCreatingGames; set => SetProperty(ref _isCreatingGames, value); }


    #region TeamBewerb - Depended Properties

    public int CountOfCourts => TeamBewerb.NumberOfCourts;
    public int CountOfGames => TeamBewerb.GetCountOfGames();
    public int CountOfGamesPerCourt => TeamBewerb.GetCountOfGamesPerCourt();
    public bool CountOfNonVirtualTeamsIsEqual => TeamBewerb?.Teams.Where(t => !t.IsVirtual).Count() % 2 == 0;
    public bool HasMoreGameRounds => SpielRunden > 1;
    public bool HasGames => CountOfGames > 0;
    public bool HasNoGames => !HasGames;

    public int BreaksCount
    {
        get => TeamBewerb.BreaksCount;
        set
        {
            if (TeamBewerb.BreaksCount == value) return;
            TeamBewerb.BreaksCount = value;
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

            TeamBewerb.RemoveAllVirtualTeams();

            if (TeamBewerb.Teams.Count(t => !t.IsVirtual) % 2 == 0 && BreaksCount == 1) //Gerade Anzahl an Mannschaften (ohne virtuelle) und 1 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames2(TeamBewerb.Teams.Count(), SpielRunden);
            }
            else if (TeamBewerb.Teams.Count(t => !t.IsVirtual) % 2 == 0 && BreaksCount == 0)//Gerade Anzahl an Mannschaften (ohne virtuelle) und 0 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames(
                                           TeamBewerb.Teams.Count(),
                                           BreaksCount,
                                           SpielRunden,
                                           HasChangeStart)
                                     .OrderBy(g => g.GameNumberOverAll)
                                     .ThenBy(g => g.CourtNumber);
            }
            else if (TeamBewerb.Teams.Count(t => !t.IsVirtual) % 2 == 0 && BreaksCount == 2)//Gerade Anzahl an Mannschaften (ohne virtuelle) und 2 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames(
                                           TeamBewerb.Teams.Count(),
                                           BreaksCount,
                                           SpielRunden,
                                           HasChangeStart)
                                     .OrderBy(g => g.GameNumberOverAll)
                                     .ThenBy(g => g.CourtNumber);

                TeamBewerb.AddVirtualTeams(2);
            }
            else if (TeamBewerb.Teams.Count(t => !t.IsVirtual) % 2 != 0 && BreaksCount == 1) //Ungerade Anzahl an Mannschaften (ohne Virtuelle) und 1 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames(
                                            TeamBewerb.Teams.Count(),
                                            BreaksCount,
                                            SpielRunden,
                                            HasChangeStart)
                                      .OrderBy(g => g.GameNumberOverAll)
                                      .ThenBy(g => g.CourtNumber);

                TeamBewerb.AddVirtualTeams(1);


            }

            GameFactoryWrapper.MatchTeamAndGames(_factoryGames, TeamBewerb.Teams);


            IsCreatingGames = false;
        },
        (p) => !IsCreatingGames || TeamBewerb.Teams.Count() >= 2
        );

    #region Constructor

    public GamesViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;

        GamesPrintsViewModel = new GamesPrintsViewModel(TeamBewerb, _turnierStore);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                GamesPrintsViewModel?.Dispose();
                GamesPrintsViewModel = null;
            }
            _disposed = true;
        }
    }

    #endregion
}
