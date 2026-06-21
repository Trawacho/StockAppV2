using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Template;
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

	private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
    private ICommand _showLiveResultCommand;
    private ICommand _printTeamResultsCommand;

    public ICommand PrintTeamResultsCommand => _printTeamResultsCommand ??= new AsyncRelayCommand(
        async (p) =>
        {
            var _printPreview = new PrintPreview(await TeamTemplateFactory.Create(_turnierStore.Turnier));
            PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "TeamResult");
            _printPreview.ShowDialog();
        },
        (p) => { return true; });

    public ICommand ShowLiveResultCommand
    {
        get => _showLiveResultCommand;
        set
        {
            (_showLiveResultCommand as IDisposable)?.Dispose();
            _showLiveResultCommand = value;
        }
    }


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

    private bool _isParagraph610Applicable;

    public bool IsParagraph610Applicable
    {
        get => _isParagraph610Applicable;
        private set => SetProperty(ref _isParagraph610Applicable, value);
    }

    public bool UseParagraph610
    {
        get => TeamBewerb.UseParagraph610;
        set
        {
            if (TeamBewerb.UseParagraph610 == value) return;
            TeamBewerb.UseParagraph610 = value;
            RaisePropertyChanged();
        }
    }

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
        SubscribeToTeamBewerbEvents();

        _showLiveResultCommand = new DialogCommand<LiveResultsTeamViewModel>(
                new DialogService<LiveResultsTeamViewModel>(
                    _dialogStore,
                    () => new LiveResultsTeamViewModel(TeamBewerb), false));

        InputMethod = PreferencesManager.TeamBewerbSettings.TeamBewerbInputMethod;
    }

    private void CurrentTeamBewerbChangend(object sender, EventArgs e)
    {
        UnsubscribeFromTeamBewerbEvents(_teamBewerb);
        TeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
        SubscribeToTeamBewerbEvents();

        _showLiveResultCommand = new DialogCommand<LiveResultsTeamViewModel>(
                new DialogService<LiveResultsTeamViewModel>(
                    _dialogStore,
                    () => new LiveResultsTeamViewModel(TeamBewerb), false));

        SetResultsEntryViewModel(InputMethod);
    }

    private void SubscribeToTeamBewerbEvents()
    {
        TeamBewerb.GamesChanged += TeamBewerb_GamesChanged;
        foreach (var game in TeamBewerb.GetAllGames())
        {
            game.Spielstand.SpielStandChanged += Spielstand_SpielStandChanged;
        }
        UpdateParagraph610Applicable();
    }

    private void UnsubscribeFromTeamBewerbEvents(ITeamBewerb teamBewerb)
    {
        if (teamBewerb == null) return;
        teamBewerb.GamesChanged -= TeamBewerb_GamesChanged;
        foreach (var game in teamBewerb.GetAllGames())
        {
            game.Spielstand.SpielStandChanged -= Spielstand_SpielStandChanged;
        }
    }

    private void TeamBewerb_GamesChanged(object sender, EventArgs e)
    {
        UnsubscribeFromTeamBewerbEvents(TeamBewerb);
        SubscribeToTeamBewerbEvents();
        UpdateParagraph610Applicable();
    }

    private void Spielstand_SpielStandChanged(object sender, EventArgs e)
    {
        UpdateParagraph610Applicable();
    }

    private void UpdateParagraph610Applicable()
    {
        bool isApplicable = StockApp.Core.Wettbewerb.Teambewerb.Paragraph610Evaluator.IsApplicable(TeamBewerb, live: false);
        IsParagraph610Applicable = isApplicable;

        if (!isApplicable && UseParagraph610)
        {
            UseParagraph610 = false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged -= CurrentTeamBewerbChangend;
                UnsubscribeFromTeamBewerbEvents(TeamBewerb);
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
                ResultsEntryViewModel = new ResultInputPerTeamViewModel(TeamBewerb.Teams, TeamBewerb.WertungskarteAsCupCard);
                break;
            case TeamBewerbInputMethod.PerTeamWithTurns:
                ResultsEntryViewModel = new ResultInputPerTeamAndKehreViewModel(TeamBewerb.Teams, TeamBewerb.Is8TurnsGame, TeamBewerb.WertungskarteAsCupCard);
                break;
        }
    }
}
