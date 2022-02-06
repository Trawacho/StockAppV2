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
    private bool _disposed;
    private UdpClient _udpClient;
    private UdpState _state;

    public event Action<BroadCastReceivedEventArgs> BroadCastReceived;
    public event Action<bool> IsRunningChanged;

    private protected void RaiseIsRunningChanged(bool isRunning)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"BroadCastService is {(isRunning ? "running" : "stopped")}");
#endif
        var handler = IsRunningChanged;
        handler?.Invoke(isRunning);
    }

    private protected void RaiseBroadCastReceived(IPEndPoint sender, byte[] data)
    {
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

        if (_state == null)
        {
            _state = new UdpState()
            {
                udpClient = _udpClient,
                ipEndPoint = new IPEndPoint(0, 0),
            };
        }

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
                RaiseBroadCastReceived(e, DeCompress(receiveBytes));
            }

            r = u?.BeginReceive(new AsyncCallback(ReceiveCallback), _state);
        }
        catch (Exception e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($" Error while receiving Broadcast: {e.Message}");
#endif
        }
    }

    static byte[] DeCompress(byte[] data)
    {
        if (data == null)
            return null;

        using var input = new MemoryStream(data);
        using var output = new MemoryStream();
        using (var datastream = new DeflateStream(input, CompressionMode.Decompress))
        {
            datastream.CopyTo(output);
        }

        return output.ToArray();
    }

    private class UdpState
    {
        public UdpClient udpClient;
        public IPEndPoint ipEndPoint;
        public IAsyncResult result;
    }
}
