using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Services;
using StockApp.UI.Stores;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public interface INavigationViewModel : IDisposable
{
    ViewModelBase CurrentNavigationViewModel { get; set; }
    void NavigationReset();
    bool IsTeamBewerb { get; }
    bool IsZielBewerb { get; }
    ICommand NavigateContestCommand { get; }
    ICommand NavigateStockTVsCommand { get; }
    ICommand NavigateTurnierCommand { get; }
}

public class NavigationViewModel : ViewModelBase, INavigationViewModel
{
    private readonly ITurnierStore _turnierStore;
    private ViewModelBase _currentNavigationViewModel;

    private readonly INavigationService<TeamBewerbContainerViewModel> _teamBewerbContainerNavigationService;
    private readonly INavigationService<TeamsViewModel> _teamsNavigationService;
    private readonly INavigationService<GamesViewModel> _gamesNavigationService;
    private readonly INavigationService<ResultsViewModel> _resultsNavigationService;
    
    private readonly INavigationService<ZielBewerbViewModel> _zielTeilnehmerNavigationService;
    private readonly INavigationService<ZielBewerbDruckViewModel> _zielDruckNavigationService;
    private readonly INavigationService<OptionsViewModel> _optionsNavigationService;
    private readonly IDialogService<LiveResultsZielViewModel> _liveResultZielDialogService;

    public ICommand NavigateTurnierCommand { get; }
    public ICommand NavigateContestCommand { get; }
    public ICommand NavigateStockTVsCommand { get; }
    public bool IsTeamBewerb => _turnierStore.Turnier.Wettbewerb is ITeamBewerb;
    public bool IsZielBewerb => _turnierStore.Turnier.Wettbewerb is IZielBewerb;

    public NavigationViewModel(ITurnierStore turnierStore,
                               INavigationService<TeamBewerbContainerViewModel> teamBewerbContainerNavigationService,
                               INavigationService<WettbewerbsartViewModel> contestNavigationService,
                               INavigationService<TurnierViewModel> turnierNavigationService,
                               INavigationService<TeamsViewModel> teamsNavigationService,
                               INavigationService<GamesViewModel> gamesNavigationService,
                               INavigationService<ResultsViewModel> resultsNavigationService,
                               INavigationService<StockTVCollectionViewModel> stockTVsNavigationService,
                               INavigationService<ZielBewerbViewModel> zielTeilnehmerNavigationService,
                               INavigationService<ZielBewerbDruckViewModel> zielDruckNavigationService,
                               INavigationService<OptionsViewModel> outputNavigationService,
                               IDialogService<LiveResultsZielViewModel> liveResultZielDialogService)
    {
        _turnierStore = turnierStore;
        _teamBewerbContainerNavigationService = teamBewerbContainerNavigationService;
        _teamsNavigationService = teamsNavigationService;
        _gamesNavigationService = gamesNavigationService;
        _resultsNavigationService = resultsNavigationService;
        _zielTeilnehmerNavigationService = zielTeilnehmerNavigationService;
        _zielDruckNavigationService = zielDruckNavigationService;
        _optionsNavigationService = outputNavigationService;
        _liveResultZielDialogService = liveResultZielDialogService;
        NavigateTurnierCommand = new NavigateCommand<TurnierViewModel>(turnierNavigationService);
        NavigateContestCommand = new NavigateCommand<WettbewerbsartViewModel>(contestNavigationService);
        NavigateStockTVsCommand = new NavigateCommand<StockTVCollectionViewModel>(stockTVsNavigationService);

        _turnierStore.Turnier.WettbewerbChanging += CurrentTurnier_WettbewerbChanging;
        _turnierStore.Turnier.WettbewerbChanged += CurrentTurnier_WettbewerbChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _currentNavigationViewModel?.Dispose();
                _turnierStore.Turnier.WettbewerbChanging -= CurrentTurnier_WettbewerbChanging;
                _turnierStore.Turnier.WettbewerbChanged -= CurrentTurnier_WettbewerbChanged;
            }
            _disposed = true;
        }
    }

    public void NavigationReset() => NavigateTurnierCommand.Execute(null);

    private void CurrentTurnier_WettbewerbChanging(object sender, CancelEventArgs e) => CurrentNavigationViewModel = null;

    private void CurrentTurnier_WettbewerbChanged()
    {
        if (_turnierStore.Turnier.Wettbewerb == null)
            CurrentNavigationViewModel = null;
        else
            CurrentNavigationViewModel =
                _turnierStore.Turnier.Wettbewerb is IContainerTeamBewerbe
                    ? new NavigationTeamViewModel(turnierStore: _turnierStore,
                                                  teamBewerbContainerNaviagationService: _teamBewerbContainerNavigationService,
                                                  teamsNavigationService: _teamsNavigationService,
                                                  gamesNavigationService: _gamesNavigationService,
                                                  resultsNavigationService: _resultsNavigationService,
                                                  optionsNavigationService: _optionsNavigationService)
                    : new NavigtaionZielViewModel(_zielTeilnehmerNavigationService,
                                                  _liveResultZielDialogService,
                                                  _zielDruckNavigationService);

        RaisePropertyChanged(nameof(IsTeamBewerb));
        RaisePropertyChanged(nameof(IsZielBewerb));
    }


    public ViewModelBase CurrentNavigationViewModel
    {
        get => _currentNavigationViewModel;
        set
        {
            _currentNavigationViewModel?.Dispose();
            SetProperty(ref _currentNavigationViewModel, value);
        }
    }
}
