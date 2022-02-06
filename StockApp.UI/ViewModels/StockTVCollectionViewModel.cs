using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;
public class StockTVCollectionViewModel : ViewModelBase
{
    private readonly IStockTVService _stockTVService;
    private readonly IStockTVCommandStore _stockTVCommandStore;
    private readonly ITurnierStore _turnierStore;

    public StockTVCollectionViewModel(IStockTVService stockTVService, IStockTVCommandStore stockTVCommandStore, ITurnierStore turnierStore)
    {
        _stockTVService = stockTVService;
        _stockTVCommandStore = stockTVCommandStore;
        _turnierStore = turnierStore;
        StockTvViewModels = new ObservableCollection<StockTVViewModel>();

        _stockTVService.StockTVCollectionChanged += StockTVService_StockTVCollectionChanged;
        FillStockTvViewModelsCollection();

        OnLoadedCommand = _stockTVCommandStore.StockTvSerivceDiscoverCommand;
        DiscoverCommand = _stockTVCommandStore.StockTvSerivceDiscoverCommand;
        RecreateViewsCommand = new RelayCommand((p) => FillStockTvViewModelsCollection(), (p) => _stockTVService.StockTVCollection.Any());
        SendTeamNamesCommand = new RelayCommand((p) => SendTeamNames(), (p) => _turnierStore.Turnier.Wettbewerb is ITeamBewerb);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stockTVService.StockTVCollectionChanged -= StockTVService_StockTVCollectionChanged;
                foreach (var item in StockTvViewModels)
                {
                    item.Dispose();
                }
                StockTvViewModels.Clear();
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public bool DirectorMode { get; set; }

    public ICommand OnLoadedCommand { get; }
    public ICommand DiscoverCommand { get; }
    public ICommand RecreateViewsCommand { get; }
    public ICommand SendTeamNamesCommand { get; init; }


    private void StockTVService_StockTVCollectionChanged(object sender, StockTVCollectionChangedEventArgs e)
    {
        App.Current.Dispatcher.Invoke(
            () => FillStockTvViewModelsCollection()
            );
    }

    private void FillStockTvViewModelsCollection()
    {
        StockTvViewModels.Clear();
        foreach (var stockTv in _stockTVService.StockTVCollection)
        {
            StockTvViewModels.Add(new StockTVViewModel(stockTv, _stockTVCommandStore));
        }
    }

    public ObservableCollection<StockTVViewModel> StockTvViewModels { get; }

    private void SendTeamNames()
    {
        var allGames = (_turnierStore.Turnier.Wettbewerb as ITeamBewerb).GetAllGames(false);
        foreach (var game in allGames.GroupBy(g => g.CourtNumber))
        {
            var tv = _stockTVService.StockTVCollection.FirstOrDefault(c => c.TVSettings.Bahn == game.Key);
            var begegnungen = new List<StockTVBegegnung>();
            foreach (var item in game)
            {
                begegnungen.Add(new StockTVBegegnung()
                {
                    SpielNummer = item.GameNumberOverAll,
                    TeamNameA = item.TeamA.TeamName,
                    TeamNameB = item.TeamB.TeamName
                });
            }
            tv?.SendTeamNames(begegnungen);
        }
    }


}
