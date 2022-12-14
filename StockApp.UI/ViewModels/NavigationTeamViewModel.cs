namespace StockApp.UI.ViewModels;

using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Commands;
using StockApp.UI.Services;
using StockApp.UI.Stores;
using System;
using System.Linq;
using System.Windows.Input;

public class NavigationTeamViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;

    private IContainerTeamBewerbe TeamBewerbContainer => _turnierStore.Turnier.Wettbewerb as IContainerTeamBewerbe;
    public NavigationTeamViewModel(ITurnierStore turnierStore,
                                   INavigationService<TeamBewerbContainerViewModel> teamBewerbContainerNaviagationService,
                                   INavigationService<TeamsViewModel> teamsNavigationService,
                                   INavigationService<GamesViewModel> gamesNavigationService,
                                   INavigationService<ResultsViewModel> resultsNavigationService)
    {
        _turnierStore = turnierStore;

        TeamBewerbContainer.CurrentTeambewerb_TeamsChanged += TeamBewerb_TeamsChanged;
        TeamBewerbContainer.CurrentTeambewerb_GamesChanged += TeamBewerb_GamesChanged;
        TeamBewerbContainer.CurrentTeamBewerbChanged += TeamBewerbContainer_ActiveTeamBewerbChanged;

        NavigateGroupsCommand = new NavigateCommand<TeamBewerbContainerViewModel>(teamBewerbContainerNaviagationService);
        NavigateTeamsCommand = new NavigateCommand<TeamsViewModel>(teamsNavigationService);
        NavigateGamesCommand = new NavigateCommand<GamesViewModel>(gamesNavigationService);
        NavigateResultsCommand = new NavigateCommand<ResultsViewModel>(resultsNavigationService);
    }



    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (TeamBewerbContainer != null)
                {
                    TeamBewerbContainer.CurrentTeambewerb_TeamsChanged -= TeamBewerb_TeamsChanged;
                    TeamBewerbContainer.CurrentTeambewerb_GamesChanged -= TeamBewerb_GamesChanged;
                    TeamBewerbContainer.CurrentTeamBewerbChanged -= TeamBewerbContainer_ActiveTeamBewerbChanged;
                }
            }
            _disposed = true;
        }
    }

    private void TeamBewerb_TeamsChanged(object sender, EventArgs e) => RaisePropertyChanged(nameof(HasTeams));
    private void TeamBewerb_GamesChanged(object sender, EventArgs e) => RaisePropertyChanged(nameof(HasGames));
    private void TeamBewerbContainer_ActiveTeamBewerbChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(HasGames));
        RaisePropertyChanged(nameof(HasTeams));
    }

    /// <summary>
    /// True, if count of Teams more than 1
    /// </summary>
    public bool HasTeams => (_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb?.Teams?.Count() > 1);

    /// <summary>
    /// True, if games existing
    /// </summary>
    public bool HasGames => (_turnierStore?.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb?.Games?.Any() ?? false);

    public ICommand NavigateGroupsCommand { get; }
    public ICommand NavigateTeamsCommand { get; }
    public ICommand NavigateGamesCommand { get; }
    public ICommand NavigateResultsCommand { get; }
}
