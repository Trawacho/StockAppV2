using System.ComponentModel;
using StockApp.Comm.MDns;

namespace StockApp.Comm.NetMqStockTV;

public interface IStockTVService : IDisposable
{
    /// <summary>
    /// Discovers for StockTV
    /// </summary>
    void Discover();
    void AddManual(string hostname, string ipAddress);

    IEnumerable<IStockTV> StockTVCollection { get; }

    event EventHandler<StockTVCollectionChangedEventArgs> StockTVCollectionChanged;
    event EventHandler<StockTVResultChangedEventArgs> StockTVResultChanged;
}


public class StockTVService : IStockTVService
{
    private readonly List<IStockTV> _stockTvList;
    private readonly IMdnsService _mdnsService;
    private readonly object _lock = new();
    private bool _disposed;

    public IEnumerable<IStockTV> StockTVCollection
    {
        get
        {

            if (_stockTvList.Any() && _stockTvList
                                        .OrderBy(t => t.TVSettings.Spielgruppe)
                                        .ThenBy(t => t.TVSettings.Bahn)
                                        .First().TVSettings.NextBahnModus == NextCourtMode.Right)
            {


                return _stockTvList.OrderBy(t => t.TVSettings.Spielgruppe)
                                        .ThenBy(t => t.TVSettings.Bahn)
                                        .ThenBy(t => t.HostName)
                                        .ThenBy(t => t.IPAddress)
                                        .ToList().AsReadOnly();
            }
            else
            {
                return _stockTvList.OrderBy(t => t.TVSettings.Spielgruppe)
                                       .ThenByDescending(t => t.TVSettings.Bahn)
                                       .ThenBy(t => t.HostName)
                                       .ThenBy(t => t.IPAddress)
                                       .ToList().AsReadOnly();
            }
        }
    }

    public event EventHandler<StockTVCollectionChangedEventArgs> StockTVCollectionChanged;
    protected virtual void RaiseStockTVCollectionChanged(bool added)
    {

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"StockTvService changed {nameof(StockTVCollection)}. {(added ? "Added" : "Removed")} item");
#endif

        var handler = StockTVCollectionChanged;
        handler?.Invoke(this, new StockTVCollectionChangedEventArgs(added));
    }


    public event EventHandler<StockTVResultChangedEventArgs> StockTVResultChanged;
    protected virtual void RaiseStockTVResultChanged(IStockTV stockTV, StockTVResultChangedEventArgs args)
    {
        var handler = StockTVResultChanged;
        handler?.Invoke(stockTV, args);
    }

    public StockTVService()
    {
        _stockTvList = new List<IStockTV>();
        
        _mdnsService = new MDnsService();
        
        _mdnsService.StockTVDiscovered += MdnsService_StockTVDiscovered;
    }

   

    #region mDNS Implementation

    public void AddManual(string hostname, string ipAddress)
    {
        var host = MDnsHost.Create(hostname, ipAddress, 4747, 4748, "n.a.");
        AddNewStockTV(host);

    }

    public void Discover() => _mdnsService.Discover();
    
    private void MdnsService_StockTVDiscovered(object sender, IMDnsHost e)
    {
        AddNewStockTV(e);
    }
       
    private void AddNewStockTV(IMDnsHost mDns) 
    {
        lock (_lock)
        {
            if (!_stockTvList.Any(s => s.IPAddress == mDns.IPAddress))
            {
                IStockTV newTV = new StockTV(mDns);

                EventHandler RemoveFromCollectionRequestedHandler = null;
                EventHandler<StockTVResultChangedEventArgs> StockTVResultChangedHandler = null;
                PropertyChangedEventHandler StockTVSettingsChangedHandler = null;
                EventHandler<bool> StockTVDirectorChangedHandler = null;

                StockTVResultChangedHandler = (s, e) => RaiseStockTVResultChanged(s as IStockTV, e);
                StockTVDirectorChangedHandler = (s, e) =>
                {
                    if (s is IStockTV stockTV && e == true)
                    {
                        foreach (IStockTV tv in _stockTvList.Where(t => t != stockTV))
                        {
                            tv.Director = false;
                        }
                    }
                };

                StockTVSettingsChangedHandler = (sender, e) =>
                {
                    if (sender is IStockTV s)
                    {
                        if (s.Director
                                && !e.PropertyName.Equals(nameof(IStockTVSettings.Bahn)))
                        {
                            object newValue = typeof(IStockTVSettings).GetProperty(e.PropertyName).GetValue(s.TVSettings);
                            foreach (IStockTV stockTV in _stockTvList.Where(t => !t.Director))
                            {
                                typeof(IStockTVSettings).GetProperty(e.PropertyName).SetValue(stockTV.TVSettings, newValue);
                            }
                        }
                    }

                };

                RemoveFromCollectionRequestedHandler = (s, e) =>
                {
                    var stockTV = s as IStockTV;
                    stockTV.RemoveFromCollectionRequested -= RemoveFromCollectionRequestedHandler;
                    stockTV.StockTVResultChanged -= StockTVResultChangedHandler;
                    stockTV.StockTVSettingsChanged -= StockTVSettingsChangedHandler;
                    stockTV.StockTVDirectorChanged -= StockTVDirectorChangedHandler;
                    lock (_lock)
                    {
                        if (_stockTvList.Remove(stockTV))
                            RaiseStockTVCollectionChanged(false);
                    }
                    stockTV.Dispose();
                };

                newTV.RemoveFromCollectionRequested += RemoveFromCollectionRequestedHandler;
                newTV.StockTVResultChanged += StockTVResultChangedHandler;
                newTV.StockTVSettingsChanged += StockTVSettingsChangedHandler;
                newTV.StockTVDirectorChanged += StockTVDirectorChangedHandler;

                _stockTvList.Add(newTV);
                RaiseStockTVCollectionChanged(true);
            }
        }
    }

    #endregion

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
                foreach (var item in _stockTvList)
                {
                    item.Dispose();
                }
                lock (_lock)
                    _stockTvList.Clear();

                StockTVResultChanged = null;
                StockTVCollectionChanged = null;
                _mdnsService?.Dispose();
            }
            _disposed = true;
        }
    }

    #endregion
}


