using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Spielplan;
using StockApp.Prints.Teamresult;
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

    public bool HasSummarizedScoreCards { get => _hasSummarizedScoreCards; set => SetProperty(ref _hasSummarizedScoreCards, value); }

    public bool HasNamesOnScoreCard { get => _hasNamesOnScoreCard; set => SetProperty(ref _hasNamesOnScoreCard, value); }

    public bool HasScoreCardsOptimizedForStockTV { get => _hasScoreCardsOptimizedForStockTV; set => SetProperty(ref _hasScoreCardsOptimizedForStockTV, value); }

    public bool HasOpponentOnScoreCard { get => _hasOpponentOnScoreCard; set => SetProperty(ref _hasOpponentOnScoreCard, value); }

    #region Commands

    public ICommand PrintScoreCardsCommand => _printScoreCardsCommand ??= new RelayCommand(
        (p) =>
        {
            _ = Prints.ScoreCards.ScoreCardsFactory.CreateScoreCards(Prints.PageSizes.A4Size, _teamBewerb, HasSummarizedScoreCards, HasNamesOnScoreCard, HasScoreCardsOptimizedForStockTV, HasOpponentOnScoreCard).ShowAsDialog();
        },
        (p) => _teamBewerb.GetCountOfGames() > 0);

    public ICommand PrintCourtCardsCommand => _printCourtCardsCommand ??= new RelayCommand(
        (p) =>
        {
            _ = Prints.CourtCards.CourtCardFactory.CreateCourtCard(Prints.PageSizes.A4Size, _teamBewerb).ShowAsDialog();
        },
        (p) => _teamBewerb.GetCountOfGames() > 0);

    public ICommand PrintReceiptsCommand => _printReceiptsCommand ??= new RelayCommand(
        (p) =>
        {
            _ = Prints.Receipts.ReceiptsFactory.CreateReceipts(Prints.PageSizes.A4Size, _turnierStore.Turnier).ShowAsDialog();
        },
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