namespace StockApp.UI.ViewModels;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Stores;
using System.Linq;
using System.Windows.Input;

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

    public bool Has8Turns
    {
        get => _teamBewerb.Is8TurnsGame;
        set
        {
            if (_teamBewerb.Is8TurnsGame == value) return;
            _teamBewerb.Is8TurnsGame = value;
            RaisePropertyChanged();
            if (value)
                HasOpponentOnScoreCard = false;
        }
    }

    public bool HasSummarizedScoreCards
    {
        get => _hasSummarizedScoreCards;
        set => SetProperty(ref _hasSummarizedScoreCards, value);
    }

    public bool HasNamesOnScoreCard
    {
        get => _hasNamesOnScoreCard;
        set => SetProperty(ref _hasNamesOnScoreCard, value);
    }

    public bool HasScoreCardsOptimizedForStockTV
    {
        get => _hasScoreCardsOptimizedForStockTV;
        set
        {
            SetProperty(ref _hasScoreCardsOptimizedForStockTV, value);
            if (value)
                HasOpponentOnScoreCard = false;
        }
    }

    public bool HasOpponentOnScoreCard
    {
        get => _hasOpponentOnScoreCard;
        set
        {
            SetProperty(ref _hasOpponentOnScoreCard, value);
            if (value)
            {
                Has8Turns = false;
                HasScoreCardsOptimizedForStockTV = false;
            }
        }
    }

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

    public ICommand PrintSpielPlanCommand => _printSpielPlanCommand ??= new RelayCommand(
        (p) =>
        {
            _ = Prints.Spielplan.SpielPlanFactory.CreateSpielPlan(Prints.PageSizes.A4Size, _teamBewerb).ShowAsDialog();
        },
        (p) => _teamBewerb.GetCountOfGames() > 0);

    public GamesPrintsViewModel(ITeamBewerb teamBewerb, ITurnierStore turnierStore)
    {
        _teamBewerb = teamBewerb;
        _turnierStore = turnierStore;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {

            }
            _disposed = true;
        }
    }


}

