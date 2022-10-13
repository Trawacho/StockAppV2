using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Services;
using StockApp.UI.Stores;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private readonly ITeamBewerb _teamBewerb;
    private readonly ITurnierNetworkManager _turnierNetworkManager;

    private ICommand _printTeamResultsCommand;
    public ICommand PrintTeamResultsCommand => _printTeamResultsCommand ??= new RelayCommand(
        (p) =>
        {
            _ = Prints.Teamresult.TeamResultsFactory.CreateTeamResult(Prints.PageSizes.A4Size, _turnierStore.Turnier).ShowAsDialog();
        },
        (p) => { return true; });

    public int NumberOfTeamsWithNamedPlayerOnResult
    {
        get => _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult;
        set
        {
            if (_teamBewerb.NumberOfTeamsWithNamedPlayerOnResult != value)
            {
                _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult = value;
                RaisePropertyChanged();
            }
        }
    }

    private bool? _inputAfterGame;
    public bool? InputAfterGame
    {
        get => _inputAfterGame;
        set => SetProperty(
            ref _inputAfterGame, 
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputAfterGameViewModel(_teamBewerb.GetAllGames());
            });
    }

    private bool? _inputPerTeam;
    public bool? InputPerTeam
    {
        get => _inputPerTeam;
        set => SetProperty(
            ref _inputPerTeam,
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputPerTeamViewModel(_teamBewerb.Teams);
            });
    }

    private bool? _inputPerTeamAndKehre;

    public bool? InputPerTeamAndKehre
    {
        get => _inputPerTeamAndKehre;
        set => SetProperty(
            ref _inputPerTeamAndKehre,
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputPerTeamAndKehreViewModel(_teamBewerb.Teams, _teamBewerb.Is8TurnsGame);
            });
    }


    private bool? _inputAfterGameAndKehre;

    public bool? InputAfterGameAndKehre
    {
        get => _inputAfterGameAndKehre;
        set => SetProperty(
            ref _inputAfterGameAndKehre,
            value,
            () =>
            {
                if (value == true)
                    ResultsEntryViewModel = new ResultInputAfterGameWithKehreViewModel(_teamBewerb.GetAllGames(), _teamBewerb.Is8TurnsGame);
            });
    }

    public bool RankingNewIERVersion
    {
        get => _teamBewerb.IERVersion == IERVersion.v2022;
        set => _teamBewerb.IERVersion = value ? IERVersion.v2022 : IERVersion.v2018;
    }

    public bool AcceptNetworkResults { get => _turnierNetworkManager.AcceptNetworkResult; set => _turnierNetworkManager.AcceptNetworkResult = value; }

    private ViewModelBase _resultsEntryViewModel;
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

    public ResultsViewModel(ITurnierStore turnierStore, ITurnierNetworkManager turnierNetworkManager)
    {
        _turnierStore = turnierStore;
        _turnierNetworkManager = turnierNetworkManager;
        if (_turnierStore.Turnier.Wettbewerb is ITeamBewerb teamBewerb)
            _teamBewerb = teamBewerb;

        InputAfterGame = false;
        InputAfterGameAndKehre = false;
        InputPerTeamAndKehre = false;
        InputPerTeam = true;



    }
}