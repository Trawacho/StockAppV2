using NetMQ;
using NetMQ.Sockets;

namespace StockApp.Comm.NetMqStockTV
{
    interface IStockTVSubscriberClient : IDisposable
    {
        event EventHandler<StockTVMessageReceivedEventArgs> SubscriberMessageReceived;
        void Start();
        void Stop();
    }

    internal class StockTVSubscriberClient : IStockTVSubscriberClient
    {
        private bool _disposed;
        private NetMQActor _actor;
        private readonly string _iP;
        private readonly int _port;


        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    SubscriberMessageReceived = null;
                }
                _disposed = true;
            }
        }
        #endregion

        private class ShimHandler : IShimHandler
        {
            private PairSocket _shim;
            private NetMQPoller _poller;
            private SubscriberSocket _subscriberSocket;
            private readonly string _connectionString;

            private readonly Action<MessageTopic, byte[]> _messageReceiveAction;

            public ShimHandler(string connectionString, Action<MessageTopic, byte[]> messageReceiveAction)
            {
                _connectionString = connectionString;
                _messageReceiveAction = messageReceiveAction;
            }

            public void Run(PairSocket shim)
            {
                using (_subscriberSocket = new SubscriberSocket())
                {
                    _subscriberSocket.Options.ReceiveHighWatermark = 1000;
                    _subscriberSocket.ReceiveReady += ResultSubscriber_ReceiveReady;
                    _subscriberSocket.Connect(_connectionString);
                    _subscriberSocket.Subscribe("");     //Subscribe to all messages

                    _shim = shim;
                    _shim.ReceiveReady += OnShimReady;
                    _shim.SignalOK();
                    _poller = new NetMQPoller { _shim, _subscriberSocket };
                    _poller.Run();
                }
            }



            private void ResultSubscriber_ReceiveReady(object sender, NetMQSocketEventArgs e)
            {
                var message = (e.Socket as SubscriberSocket).ReceiveMultipartMessage();
                if (message.FrameCount >= 2)
                {
                    MessageTopic topic = (MessageTopic)Enum.Parse(typeof(MessageTopic), message[0].ConvertToString());
                    byte[] value = message[1].ToByteArray();

                    _messageReceiveAction?.Invoke(topic, value);
                }
            }

            private void OnShimReady(object sender, NetMQSocketEventArgs e)
            {
                string command = e.Socket.ReceiveFrameString();
                if (command == NetMQActor.EndShimMessage)
                {
                    _poller.Stop();
                }
            }
        }


        public event EventHandler<StockTVMessageReceivedEventArgs> SubscriberMessageReceived;
        protected void RaiseSubscriberMessageReceived(MessageTopic topic, byte[] value)
        {
            var handler = SubscriberMessageReceived;
            handler?.Invoke(this, new StockTVMessageReceivedEventArgs(topic, value));
        }


        #region Constructor

        internal StockTVSubscriberClient(string ip, int port)
        {
            _iP = ip;
            _port = port;
        }

        #endregion

        /// <summary>
        /// e.g. tcp://192.168.100.130:4748
        /// </summary>
        private string Address => String.Format("tcp://{0}:{1}", _iP, _port);

        public void Start()
        {
            _actor ??= NetMQActor.Create(new ShimHandler(Address, (topic, value) => RaiseSubscriberMessageReceived(topic, value)));
        }

        public void Stop()
        {
            if (_actor != null)
            {
                _actor.Dispose();
                _actor = null;
            }
        }
    }
}
