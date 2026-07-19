using System.IO.Compression;
using System.Net;
using System.Net.Sockets;

namespace StockApp.Comm.Broadcasting;

public interface IBroadcastService : IDisposable
{
    void Start();
    void Stop();
    bool IsRunning { get; }
    event Action<BroadCastReceivedEventArgs> BroadCastReceived;
    event Action<bool> IsRunningChanged;
}

public class BroadcastService : IBroadcastService
{
    private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private bool _disposed;
    private UdpClient _udpClient;
    private UdpState _state;

    public event Action<BroadCastReceivedEventArgs> BroadCastReceived;
    public event Action<bool> IsRunningChanged;

    private protected void RaiseIsRunningChanged(bool isRunning)
    {
        _logger.Info($"BroadcastService is {(isRunning ? "running" : "stopped")}");
        var handler = IsRunningChanged;
        handler?.Invoke(isRunning);
    }

    private protected void RaiseBroadCastReceived(IPEndPoint sender, byte[] data)
    {
        _logger.Debug($"Broadcast received from {sender}: {data.Length} bytes");
        var handler = BroadCastReceived;
        handler?.Invoke(new BroadCastReceivedEventArgs(sender, data));
    }


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
                Stop();
                IsRunningChanged = null;
                BroadCastReceived = null;
            }
            _disposed = true;
        }
    }

    public BroadcastService()
    {

    }

    public bool IsRunning => _udpClient != null;

    public void Start()
    {
        if (_udpClient == null)
        {
            _udpClient = new UdpClient();
            _udpClient.Client.ReceiveTimeout = 500;
            _udpClient.EnableBroadcast = true;
            _udpClient.Client.Blocking = false;
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 4711));
        }

        _state ??= new UdpState()
            {
                udpClient = _udpClient,
                ipEndPoint = new IPEndPoint(0, 0),
            };

        _state.result = _state.udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), _state);
        RaiseIsRunningChanged(true);
    }

    public void Stop()
    {
        _udpClient?.Close();
        _udpClient?.Dispose();
        _udpClient = null;
        if (_state != null)
        {
            _state.udpClient = null;
            _state = null;
        }

        RaiseIsRunningChanged(false);
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            UdpClient u = ((UdpState)ar.AsyncState).udpClient;
            IPEndPoint e = ((UdpState)ar.AsyncState).ipEndPoint;
            IAsyncResult r = ((UdpState)ar.AsyncState).result;

            byte[] receiveBytes = u?.EndReceive(ar, ref e);
            if (receiveBytes?.Length > 1)
            {
                RaiseBroadCastReceived(e, receiveBytes);
            }

            r = u?.BeginReceive(new AsyncCallback(ReceiveCallback), _state);
        }
        catch (ObjectDisposedException)
        {
            // Expected when Stop() closes the socket while a receive is in flight.
            _logger.Debug("Broadcast listener socket was closed while a receive was pending.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error while receiving broadcast: {ex.Message}", ex);
        }
    }

    private class UdpState
    {
        public UdpClient udpClient;
        public IPEndPoint ipEndPoint;
        public IAsyncResult result;
    }
}
