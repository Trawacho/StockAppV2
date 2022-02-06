using StockApp.Comm.NetMqStockTV;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;
public class StockTVCollectionViewModel : ViewModelBase
{
    private readonly IStockTVService _stockTVService;
    private readonly IStockTVCommandStore _stockTVCommandStore;

    public StockTVCollectionViewModel(IStockTVService stockTVService, IStockTVCommandStore stockTVCommandStore)
    {
        _stockTVService = stockTVService;
        _stockTVCommandStore = stockTVCommandStore;
        StockTvViewModels = new ObservableCollection<StockTVViewModel>();

        _stockTVService.StockTVCollectionChanged += StockTVService_StockTVCollectionChanged;
        FillStockTvViewModelsCollection();

        OnLoadedCommand = _stockTVCommandStore.StockTvSerivceDiscoverCommand;
        DiscoverCommand = _stockTVCommandStore.StockTvSerivceDiscoverCommand;
        RecreateViews = new RelayCommand((p) => FillStockTvViewModelsCollection(), (p) => _stockTVService.StockTVCollection.Any());
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
    public ICommand RecreateViews { get; }


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




}
