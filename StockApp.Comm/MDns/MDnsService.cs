using Zeroconf;

namespace StockApp.Comm.MDns;

internal interface IMdnsService : IDisposable
{
    void Discover();
    void Stop();

    event EventHandler<IMDnsHost> StockTVDiscovered;
}

internal class MDnsService : IMdnsService
{
    private IDisposable _listenSubscription;
    private bool _disposedValue;
    private readonly string _protocol = "_stockTV._tcp.local.";

    public event EventHandler<IMDnsHost> StockTVDiscovered;
    protected virtual void RaiseStockTVDiscoverd(MDnsHost host)
    {
        var handler = StockTVDiscovered;
        handler?.Invoke(this, host);
    }

    internal MDnsService()
    {

    }

    public void Discover()
    {
        if (_listenSubscription != null)
        {
            _listenSubscription.Dispose();
            _listenSubscription = null;
        }

        var sub = ZeroconfResolver.Resolve(_protocol, TimeSpan.FromSeconds(5), 5, 500);
        _listenSubscription = sub.Subscribe(resp => RaiseStockTVDiscoverd(new MDnsHost(resp)));

    }

    public void Stop()
    {
        if (_listenSubscription != null)
        {
            _listenSubscription.Dispose();
            _listenSubscription = null;
        }
    }



    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _listenSubscription?.Dispose();
                _listenSubscription = null;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MDnsService()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}



