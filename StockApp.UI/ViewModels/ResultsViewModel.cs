using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Services;
using StockApp.UI.Stores;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private readonly IDialogStore _dialogStore;
    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
    private readonly ITurnierNetworkManager _turnierNetworkManager;
    private bool? _inputAfterGame;
    private bool? _inputPerTeam;
    private bool? _inputPerTeamAndKehre;
    private bool? _inputAfterGameAndKehre;
    private ViewModelBase _resultsEntryViewModel;

    private ICommand _printTeamResultsCommand;

    public ICommand PrintTeamResultsCommand => _printTeamResultsCommand ??= new RelayCommand(
        (p) =>
        {
            _ = Prints.Teamresult.TeamResultsFactory.CreateTeamResult(Prints.PageSizes.A4Size, _turnierStore.Turnier).ShowAsDialog();
        },
        (p) => { return true; });
    public ICommand ShowLiveResultCommand { get; init; }

    

    public bool? InputAfterGame
    {
        get => _inputAfterGame;
        set => SetProperty(
            ref _inputAfterGame,
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputAfterGameViewModel(TeamBewerb.GetAllGames());
            });
    }

    public bool? InputPerTeam
    {
        get => _inputPerTeam;
        set => SetProperty(
            ref _inputPerTeam,
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputPerTeamViewModel(TeamBewerb.Teams);
            });
    }

    public bool? InputPerTeamAndKehre
    {
        get => _inputPerTeamAndKehre;
        set => SetProperty(
            ref _inputPerTeamAndKehre,
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputPerTeamAndKehreViewModel(TeamBewerb.Teams, TeamBewerb.Is8TurnsGame);
            });
    }

    public bool? InputAfterGameAndKehre
    {
        get => _inputAfterGameAndKehre;
        set => SetProperty(
            ref _inputAfterGameAndKehre,
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputAfterGameWithKehreViewModel(TeamBewerb.GetAllGames(), TeamBewerb.Is8TurnsGame);
            });
    }

    public bool RankingNewIERVersion
    {
        get => TeamBewerb.IERVersion == IERVersion.v2022;
        set => TeamBewerb.IERVersion = value ? IERVersion.v2022 : IERVersion.v2018;
    }

    public bool AcceptNetworkResults { get => _turnierNetworkManager.AcceptNetworkResult; set => _turnierNetworkManager.AcceptNetworkResult = value; }

    public ViewModelBase ResultsEntryViewModel
    {
        get => _resultsEntryViewModel;
        set
        {
            if (_resultsEntryViewModel == value) return;
            _resultsEntryViewModel?.Dispose();
            _resultsEntryViewModel = value;
            RaisePropertyChanged();
        }
    }

    public ResultsViewModel(ITurnierStore turnierStore, IDialogStore dialogStore, ITurnierNetworkManager turnierNetworkManager)
    {
        _turnierStore = turnierStore;
        _dialogStore = dialogStore;
        _turnierNetworkManager = turnierNetworkManager;

        InputAfterGame = false;
        InputAfterGameAndKehre = false;
        InputPerTeamAndKehre = false;
        InputPerTeam = true;


        ShowLiveResultCommand = new DialogCommand<LiveResultsTeamViewModel>(
                new DialogService<LiveResultsTeamViewModel>(
                    _dialogStore,
                    () => new LiveResultsTeamViewModel(TeamBewerb), false));

    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                ResultsEntryViewModel.Dispose();
            }
            _disposed = true;
        }
    }
}