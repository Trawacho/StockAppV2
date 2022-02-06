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
    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.Wettbewerb as ITeamBewerb;
    public NavigationTeamViewModel(ITurnierStore turnierStore,
                                    INavigationService<TeamsViewModel> teamsNavigationService,
                                    INavigationService<GamesViewModel> gamesNavigationService,
                                    INavigationService<ResultsViewModel> resultsNavigationService,
                                    IDialogService<LiveResultsTeamViewModel> liveResultTeamDialogCommand)
    {
        _turnierStore = turnierStore;
        TeamBewerb.TeamsChanged += TeamBewerb_TeamsChanged;
        TeamBewerb.GamesChanged += TeamBewerb_GamesChanged;

        NavigateTeamsCommand = new NavigateCommand<TeamsViewModel>(teamsNavigationService);
        NavigateGamesCommand = new NavigateCommand<GamesViewModel>(gamesNavigationService);
        NavigateResultsCommand = new NavigateCommand<ResultsViewModel>(resultsNavigationService);
        NavigateLiveResultsCommand = new DialogCommand<LiveResultsTeamViewModel>(liveResultTeamDialogCommand);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (TeamBewerb != null)
                {
                    TeamBewerb.TeamsChanged -= TeamBewerb_TeamsChanged;
                    TeamBewerb.GamesChanged -= TeamBewerb_GamesChanged;
                }
            }
            _disposed = true;
        }
    }

    private void TeamBewerb_TeamsChanged(object sender, EventArgs e) => RaisePropertyChanged(nameof(HasTeams));
    private void TeamBewerb_GamesChanged(object sender, EventArgs e) => RaisePropertyChanged(nameof(HasGames));

    /// <summary>
    /// True, if count of Teams more than 1
    /// </summary>
    public bool HasTeams => (TeamBewerb?.Teams?.Count() > 1);

    /// <summary>
    /// True, if games existing
    /// </summary>
    public bool HasGames => (TeamBewerb?.Games?.Any() ?? false);

    public ICommand NavigateTeamsCommand { get; }
    public ICommand NavigateGamesCommand { get; }
    public ICommand NavigateResultsCommand { get; }
    public ICommand NavigateLiveResultsCommand { get; }
}
