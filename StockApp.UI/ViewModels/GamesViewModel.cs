using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
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

    public int BreaksCount
    {
        get => _teamBewerb.BreaksCount;
        set
        {
            if (_teamBewerb.BreaksCount == value) return;
            _teamBewerb.BreaksCount = value;
            RaisePropertyChanged();
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

    public bool Has8Turns
    {
        get => _teamBewerb.Is8TurnsGame;
        set
        {
            if (_teamBewerb.Is8TurnsGame == value) return;
            _teamBewerb.Is8TurnsGame = value;
            RaisePropertyChanged();
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

    private IEnumerable<IFactoryGame> _factoryGames;

    public ICommand CreateGamesCommand => _createGamesCommand ??= new RelayCommand
        ((p) =>
        {
            IsCreatingGames = true;

            _teamBewerb.RemoveAllVirtualTeams();

            if (_teamBewerb.Teams.Count(t => !t.IsVirtual) % 2 == 0 && BreaksCount == 1) //Gerade Anzahl an Mannschaften (ohne virtuelle) und 1 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames2(_teamBewerb.Teams.Count(), SpielRunden);
            }
            else if (_teamBewerb.Teams.Count(t => !t.IsVirtual) % 2 == 0 && BreaksCount == 0)//Gerade Anzahl an Mannschaften (ohne virtuelle) und 0 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames(
                                           _teamBewerb.Teams.Count(),
                                           BreaksCount,
                                           SpielRunden,
                                           HasChangeStart)
                                     .OrderBy(g => g.GameNumberOverAll)
                                     .ThenBy(g => g.CourtNumber);
            }
            else if (_teamBewerb.Teams.Count(t => !t.IsVirtual) % 2 == 0 && BreaksCount == 2)//Gerade Anzahl an Mannschaften (ohne virtuelle) und 2 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames(
                                           _teamBewerb.Teams.Count(),
                                           BreaksCount,
                                           SpielRunden,
                                           HasChangeStart)
                                     .OrderBy(g => g.GameNumberOverAll)
                                     .ThenBy(g => g.CourtNumber);

                _teamBewerb.AddVirtualTeams(2);
            }
            else if (_teamBewerb.Teams.Count(t => !t.IsVirtual) % 2 != 0 && BreaksCount == 1) //Ungerade Anzahl an Mannschaften (ohne Virtuelle) und 1 Aussetzer
            {
                _factoryGames = GameFactory.CreateGames(
                                            _teamBewerb.Teams.Count(),
                                            BreaksCount,
                                            SpielRunden,
                                            HasChangeStart)
                                      .OrderBy(g => g.GameNumberOverAll)
                                      .ThenBy(g => g.CourtNumber);

                _teamBewerb.AddVirtualTeams(1);
                

            }

            GameFactoryWrapper.MatchTeamAndGames(_factoryGames, _teamBewerb.Teams);


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
