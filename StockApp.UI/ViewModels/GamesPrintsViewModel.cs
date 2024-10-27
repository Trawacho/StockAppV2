using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Receipts;
using StockApp.Prints.ScoreCards;
using StockApp.Prints.Spielplan;
using StockApp.UI.Commands;
using StockApp.UI.Components;
using StockApp.UI.Extensions;
using StockApp.UI.Settings;
using StockApp.UI.Stores;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class GamesPrintsViewModel : ViewModelBase
{

	private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



	private readonly ITeamBewerb _teamBewerb;
    private readonly ITurnierStore _turnierStore;
    private ICommand _printScoreCardsCommand;
    private ICommand _printCourtCardsCommand;
    private ICommand _printReceiptsCommand;
    private ICommand _printSpielPlanCommand;
    private bool _hasSummarizedScoreCards;
    private bool _hasNamesOnScoreCard;
    private bool _hasScoreCardsOptimizedForStockTV;
    private bool _hasOpponentOnScoreCard;

    public bool Has8Turns => _teamBewerb.Is8TurnsGame;

    public bool Has6Turns => !Has8Turns;

    public bool HasSummarizedScoreCards
    {
        get => _hasSummarizedScoreCards;
        set => SetProperty(
            ref _hasSummarizedScoreCards,
            value,
            () => PreferencesManager.TeamBewerbSettings.HasSummarizedScoreCards = value);
    }

    public bool HasNamesOnScoreCard
    {
        get => _hasNamesOnScoreCard;
        set => SetProperty(
            ref _hasNamesOnScoreCard,
            value,
            () => PreferencesManager.TeamBewerbSettings.HasNamesOnScoreCard = value);
    }

    public bool HasScoreCardsOptimizedForStockTV
    {
        get => _hasScoreCardsOptimizedForStockTV;
        set => SetProperty(ref _hasScoreCardsOptimizedForStockTV, value);
    }

    public bool HasOpponentOnScoreCard
    {
        get => _hasOpponentOnScoreCard;
        set => SetProperty(
            ref _hasOpponentOnScoreCard,
            value,
            () => PreferencesManager.TeamBewerbSettings.HasOpponentOnScoreCard = value);
    }

    #region Commands

    public ICommand PrintScoreCardsCommand => _printScoreCardsCommand ??= new RelayCommand(
        async (p) =>
        {
            var _printPreview = new PrintPreview(await ScoreCardsFactory.Create(_teamBewerb, HasNamesOnScoreCard, HasSummarizedScoreCards, HasOpponentOnScoreCard));
            PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "WertungsKarte");
            _printPreview.ShowDialog();
        },
        (p) => _teamBewerb.GetCountOfGames() > 0);

    public ICommand PrintCourtCardsCommand => _printCourtCardsCommand ??= new RelayCommand(
        (p) =>
        {
            _ = Prints.CourtCards.CourtCardFactory.CreateCourtCard(Prints.PageSizes.A4Size, _teamBewerb).ShowAsDialog();
        },
        (p) => _teamBewerb.GetCountOfGames() > 0);

    public ICommand PrintReceiptsCommand => _printReceiptsCommand ??= new AsyncRelayCommand(
        async (p) =>
        {
            var _printPreview = new PrintPreview(await ReceiptsFactory.Create(_turnierStore.Turnier));
            PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "Receipt");
            _printPreview.ShowDialog();
        }
        ,
        (p) => (_teamBewerb.Teams?.Count() ?? 0) > 0);

    public ICommand PrintSpielPlanCommand => _printSpielPlanCommand ??= new AsyncRelayCommand(
        async (p) =>
        {
            var _printPreview = new PrintPreview(await SpielPlanFactory.Create(_turnierStore.Turnier));
            PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "SpielPlan");
            _printPreview.ShowDialog();
        },
        (p) => _teamBewerb.GetCountOfGames() > 0);

    #endregion

    public GamesPrintsViewModel(ITeamBewerb teamBewerb, ITurnierStore turnierStore)
    {
        _teamBewerb = teamBewerb;
        _turnierStore = turnierStore;

        _teamBewerb.Is8TurnsGameChanged += TeamBewerb_Is8TurnsGameChanged;

        HasNamesOnScoreCard = PreferencesManager.TeamBewerbSettings.HasNamesOnScoreCard;
        HasOpponentOnScoreCard = PreferencesManager.TeamBewerbSettings.HasOpponentOnScoreCard;
        HasSummarizedScoreCards = PreferencesManager.TeamBewerbSettings.HasSummarizedScoreCards;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _teamBewerb.Is8TurnsGameChanged -= TeamBewerb_Is8TurnsGameChanged;
            }
            _disposed = true;
        }
    }

    private void TeamBewerb_Is8TurnsGameChanged(object sender, System.EventArgs e)
    {
        RaisePropertyChanged(nameof(Has8Turns));
        RaisePropertyChanged(nameof(Has6Turns));
        if (Has8Turns)
        {
            HasOpponentOnScoreCard = false;
            HasScoreCardsOptimizedForStockTV = false;
        }
    }
}