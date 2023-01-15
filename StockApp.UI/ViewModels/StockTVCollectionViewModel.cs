using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;
public class StockTVCollectionViewModel : ViewModelBase
{
    private readonly IStockTVService _stockTVService;
    private readonly IStockTVCommandStore _stockTVCommandStore;
    private readonly ITurnierStore _turnierStore;
    private string _ipAddress;

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
        SendTeamNamesCommand = new RelayCommand((p) => SendTeamNames(), (p) => _turnierStore.Turnier.Wettbewerb is IContainerTeamBewerbe);
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
        if (_turnierStore.Turnier.Wettbewerb is IContainerTeamBewerbe containerTeamBewerbe)
        {
            if (containerTeamBewerbe.CurrentTeamBewerb is ITeamBewerb teamBewerb)
            {
                var allGames = teamBewerb.GetAllGames(false);
                foreach (var game in allGames.GroupBy(g => g.CourtNumber))
                {
                    var tv = _stockTVService.StockTVCollection.FirstOrDefault(c => c.TVSettings.Bahn == game.Key && c.TVSettings.Spielgruppe == teamBewerb.SpielGruppe);
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

    }

    public string IpAddress { get => _ipAddress; set => SetProperty(ref _ipAddress, value); }

    private ICommand _addManualCommand;
    public ICommand AddManualCommand => _addManualCommand ??= new RelayCommand(
        (p) =>
        {
            if (IPAddress.TryParse(IpAddress, out var ipA))
            {
                IpAddress = ipA.ToString();
                var hostname = ipA.GetAddressBytes().Last().ToString();
                _stockTVService.AddManual(hostname, ipA.ToString());
            }
        },
        (p) =>
        {
            return IPAddress.TryParse(IpAddress, out IPAddress ipAddress);
        });

}
