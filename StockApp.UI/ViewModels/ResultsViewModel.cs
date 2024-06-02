using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Teamresult;
using StockApp.UI.Commands;
using StockApp.UI.Components;
using StockApp.UI.Enumerations;
using StockApp.UI.Services;
using StockApp.UI.Settings;
using StockApp.UI.Stores;
using System;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class ResultsViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private readonly IDialogStore _dialogStore;
    private ITeamBewerb _teamBewerb;

    private ITeamBewerb TeamBewerb
    {
        get => _teamBewerb;
        set
        {
            SetProperty(ref _teamBewerb, value);  
        }
    }
    private readonly ITurnierNetworkManager _turnierNetworkManager;
    private TeamBewerbInputMethod _inputMethod;
    private ViewModelBase _resultsEntryViewModel;
    private ICommand _printTeamResultsCommand;

    public ICommand PrintTeamResultsCommand => _printTeamResultsCommand ??= new AsyncRelayCommand(
        async (p) =>
        {
            var _printPreview = new PrintPreview(await TeamTemplateFactory.Create(_turnierStore.Turnier));
            PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "TeamResult");
            _printPreview.ShowDialog();
        },
        (p) => { return true; });

    public ICommand ShowLiveResultCommand { get; set; }


    public TeamBewerbInputMethod InputMethod
    {
        get => _inputMethod;
        set
        {
            _inputMethod = value;
            PreferencesManager.TeamBewerbSettings.TeamBewerbInputMethod = value;
            SetResultsEntryViewModel(value);
        }
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

        _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged += CurrentTeamBewerbChangend;

        TeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;


        ShowLiveResultCommand = new DialogCommand<LiveResultsTeamViewModel>(
                new DialogService<LiveResultsTeamViewModel>(
                    _dialogStore,
                    () => new LiveResultsTeamViewModel(TeamBewerb), false));

        InputMethod = PreferencesManager.TeamBewerbSettings.TeamBewerbInputMethod;
    }

    private void CurrentTeamBewerbChangend(object sender, EventArgs e)
    {
        TeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;


        ShowLiveResultCommand = new DialogCommand<LiveResultsTeamViewModel>(
                new DialogService<LiveResultsTeamViewModel>(
                    _dialogStore,
                    () => new LiveResultsTeamViewModel(TeamBewerb), false));

        SetResultsEntryViewModel(InputMethod);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged -= CurrentTeamBewerbChangend;
                ResultsEntryViewModel?.Dispose();
            }
            _disposed = true;
        }
    }

    private void SetResultsEntryViewModel(TeamBewerbInputMethod inputMethod)
    {
        switch (inputMethod)
        {
            case TeamBewerbInputMethod.AfterGame:
                ResultsEntryViewModel = new ResultInputAfterGameViewModel(TeamBewerb.GetAllGames());
                break;
            case TeamBewerbInputMethod.AfterGameWithTurns:
                ResultsEntryViewModel = new ResultInputAfterGameWithKehreViewModel(TeamBewerb.GetAllGames(), TeamBewerb.Is8TurnsGame);
                break;
            case TeamBewerbInputMethod.PerTeam:
                ResultsEntryViewModel = new ResultInputPerTeamViewModel(TeamBewerb.Teams);
                break;
            case TeamBewerbInputMethod.PerTeamWithTurns:
                ResultsEntryViewModel = new ResultInputPerTeamAndKehreViewModel(TeamBewerb.Teams, TeamBewerb.Is8TurnsGame);
                break;
        }
    }
}