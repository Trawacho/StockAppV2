using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
using StockApp.UI.Stores;
using System;
using System.Threading.Tasks;

namespace StockApp.UI.Services;

public interface ITurnierNetworkManager : IDisposable
{
    bool AcceptNetworkResult { get; set; }
}

public class TurnierNetworkManager : ITurnierNetworkManager
{
	private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	private readonly ITurnierStore _turnierStore;
    private readonly IStockTVService _stockTVService;
    private readonly IBroadcastService _broadCastService;
    private bool _disposed;
    private bool _acceptNetworkResult;

    public TurnierNetworkManager(ITurnierStore turnierStore, IStockTVService stockTVService, IBroadcastService broadCastService)
    {
        _turnierStore = turnierStore;
        _stockTVService = stockTVService;
        _broadCastService = broadCastService;
        _stockTVService.StockTVResultChanged += StockTVResultChanged;
        _broadCastService.BroadCastReceived += BroadCastReceived;
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stockTVService.StockTVResultChanged -= StockTVResultChanged;
                _broadCastService.BroadCastReceived -= BroadCastReceived;
                _stockTVService?.Dispose();
                _broadCastService?.Dispose();
            }
            _disposed = true;
        }
    }

    #endregion

    private void StockTVResultChanged(object sender, StockTVResultChangedEventArgs e)
    {
        if (AcceptNetworkResult)
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                _turnierStore.Turnier.Wettbewerb?.SetStockTVResult(e.TVResult));
    }

    private void BroadCastReceived(BroadCastReceivedEventArgs obj)
    {
        if (AcceptNetworkResult)
            System.Windows.Application.Current.Dispatcher?.Invoke(() =>
                _turnierStore.Turnier.Wettbewerb?.SetBroadcastData(obj.NetworkTelegram));
    }

    public bool AcceptNetworkResult
    {
        get => _acceptNetworkResult;
        set
        {
            _acceptNetworkResult = value;

            _logger.Info($"Accept Network Result is set: {value}");


			if (value)
                Task.Factory.StartNew(() => _broadCastService.Start());
            else
                Task.Factory.StartNew(() => _broadCastService.Stop());
        }
    }
}

