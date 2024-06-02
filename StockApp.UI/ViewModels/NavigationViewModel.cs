using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Services;
using StockApp.UI.Stores;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public interface INavigationViewModel : IDisposable
{
    ViewModelBase CurrentNavigationViewModel { get; set; }
    void NavigationReset();
    bool IsTeamBewerb { get; }
    bool IsZielBewerb { get; }
    int Groups { get; }
    ICommand NavigateContestCommand { get; }
    ICommand NavigateStockTVsCommand { get; }
    ICommand NavigateTurnierCommand { get; }
    ICommand SelectGroupCommand { get; }
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
   
    public bool IsTeamBewerb => _turnierStore.Turnier.Wettbewerb is IContainerTeamBewerbe;
    public bool IsZielBewerb => _turnierStore.Turnier.Wettbewerb is IZielBewerb;
    public int Groups => IsTeamBewerb 
        ? _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Count() 
        : 0;
    public int CurrentTeamBewerbId => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ID;

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
        SelectGroupCommand = new RelayCommand((p) => SetSelectedGroup(p), _ => true);

        _turnierStore.Turnier.WettbewerbChanging += CurrentTurnier_WettbewerbChanging;
        _turnierStore.Turnier.WettbewerbChanged += CurrentTurnier_WettbewerbChanged;
        _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbeChanged += ContainerTeamBewerbe_TeamBewerbeChanged;
        _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged += ContainerTeamBewerbe_CurrentTeamBewerbChanged;
    }

    private void ContainerTeamBewerbe_CurrentTeamBewerbChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(CurrentTeamBewerbId));
    }

    private void ContainerTeamBewerbe_TeamBewerbeChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(Groups));
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
                _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbeChanged -= ContainerTeamBewerbe_TeamBewerbeChanged;
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

    
    public ICommand SelectGroupCommand { get; } 
    private void SetSelectedGroup(object p)
    {
        if(int.TryParse(p.ToString(), out int groupId))
        {
            var t = _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Single(t => t.ID == groupId);
            _turnierStore.Turnier.ContainerTeamBewerbe.SetCurrentTeamBewerb(t);
        }
    }
}
