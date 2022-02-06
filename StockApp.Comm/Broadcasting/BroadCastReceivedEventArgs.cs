using System.Net;

namespace StockApp.Comm.Broadcasting
{
    public class BroadCastReceivedEventArgs : EventArgs
    {
        public BroadCastReceivedEventArgs(IPEndPoint sender, byte[] data)
        {
            NetworkTelegram = new(data);
            Sender = sender;
        }

        public BroadCastTelegram NetworkTelegram { get; }
        public IPEndPoint Sender { get; }
    }
}
