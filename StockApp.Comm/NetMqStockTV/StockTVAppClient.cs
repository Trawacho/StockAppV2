using NetMQ;
using NetMQ.Monitoring;
using NetMQ.Sockets;
using System.Text;

namespace StockApp.Comm.NetMqStockTV
{
    interface IStockTVAppClient : IDisposable
    {
        bool IsConnected { get; }
        void Start();
        void Stop();
        void SendToStockTV(MessageTopic topic, byte[] value, byte[] additionalValue = null);
        void SendToStockTV(MessageTopic topic, string value);
        void SendToStockTV(MessageTopic topic);

        event EventHandler<bool> ConnectedChanged;
        event EventHandler<StockTVMessageReceivedEventArgs> MessageReceived;
    }



    /// <summary>
    /// NetMQ-client to communication with StockTV
    /// </summary>
    internal class StockTVAppClient : IStockTVAppClient
    {

		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


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
                    MessageReceived = null;
                    ConnectedChanged = null;
                }
                _disposed = true;
            }
        }


        #region Events

        //public event StockTVMessageReceivedEventHandler MessageReceived;
        public event EventHandler<StockTVMessageReceivedEventArgs> MessageReceived;
        protected void RaiseMessageReceived(NetMQFrame topic, NetMQFrame value)
        {
			var handler = MessageReceived;
            MessageTopic mt = (MessageTopic)Enum.Parse(typeof(MessageTopic), topic.ConvertToString());
			var valueArr = value.ToByteArray(true);
			handler?.Invoke(this, new StockTVMessageReceivedEventArgs(mt, valueArr));
            
			_logger.Debug($"{mt} received, {string.Join("-", valueArr.Take(10))} {Encoding.UTF8.GetString(valueArr.Skip(10).ToArray())}");
		}

        public event EventHandler<bool> ConnectedChanged;
        protected void RaiseConnectedChanged()
        {
            var handler = ConnectedChanged;
            handler?.Invoke(this, this.IsConnected);
        }

        #endregion


        #region Fields

        private NetMQPoller _poller;
        private DealerSocket _socket;
        private NetMQMonitor _monitor;
        private readonly string _iPAddress;
        private readonly int _port;
        private readonly string _identifier;
        private NetMQQueue<List<NetMQFrame>> _sendQueue;
        private NetMQQueue<NetMQMessage> _receiveQueue;
        private bool _isConnected;
        private bool _disposed;


        #endregion


        #region Konstruktor

        public StockTVAppClient(string ip, int port, string identifier)
        {
            this.IsConnected = false;
            this._iPAddress = ip;
            this._port = port;
            this._identifier = $"{identifier}-{Guid.NewGuid()}";
        }

        #endregion


        #region IsConnected


        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected == value) return;
                _isConnected = value;
                RaiseConnectedChanged();
            }
        }

        /// <summary>
        /// Set the property <see cref="IsConnected"/> 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Monitor_EventReceived(object sender, NetMQMonitorEventArgs e)
        {
            this.IsConnected = e.SocketEvent == SocketEvents.Connected;
        }

        #endregion


        #region Start and Stop functions

        public void Start()
        {
            if (_socket != null) return;

            _socket = new DealerSocket();
            _socket.Options.Identity = Encoding.UTF8.GetBytes(this._identifier);
            _socket.ReceiveReady += Socket_ReceiveReady;
            _socket.Connect($"tcp://{_iPAddress}:{_port}");

            _sendQueue = new NetMQQueue<List<NetMQFrame>>();
            _sendQueue.ReceiveReady += SendQueue_ReceiveReady;

            _receiveQueue = new NetMQQueue<NetMQMessage>();
            _receiveQueue.ReceiveReady += ReceiveQueue_ReceiveReady;

            _poller = new NetMQPoller() { _socket, _receiveQueue, _sendQueue };

            _monitor = new NetMQMonitor(_socket, $"inproc://{_identifier}.inproc", SocketEvents.All);
            _monitor.EventReceived += Monitor_EventReceived;
            _monitor.AttachToPoller(_poller);

            Thread.Sleep(50);
            _poller.RunAsync(_identifier);

            //Sending Hello to StockTV
            SendToStockTV(MessageTopic.Hello);
        }



        public void Stop()
        {
            if (_sendQueue != null)
                _sendQueue.ReceiveReady -= SendQueue_ReceiveReady;
            if (_receiveQueue != null)
                _receiveQueue.ReceiveReady -= ReceiveQueue_ReceiveReady;

            _poller?.Stop();
            _poller?.Remove(_socket);
            _poller?.Remove(_sendQueue); _sendQueue?.Dispose(); _sendQueue = null;
            _poller?.Remove(_receiveQueue); _receiveQueue?.Dispose(); _receiveQueue = null;

            _monitor?.DetachFromPoller();

            _poller?.Dispose(); _poller = null;
            _socket?.Dispose(); _socket = null;

            if (_monitor != null)
                _monitor.EventReceived -= Monitor_EventReceived;
            _monitor?.Dispose(); _monitor = null;

            IsConnected = false;
        }


        #endregion


        #region Receiving from StockTV

        /// <summary>
        /// Receiving from Socket and add message to Queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var msg = new NetMQMessage();
            if (e.Socket.TryReceiveMultipartMessage(TimeSpan.FromSeconds(1), ref msg))
            {
                if (msg.Last.ConvertToString().Equals(MessageTopic.Welcome.ToString()))
                {
                    this.IsConnected = true;
                }
                else
                {
                    _receiveQueue.Enqueue(msg);
                }
            }
        }

        /// <summary>
        /// Dequeue a message received from StockTV and raise messagereceived
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveQueue_ReceiveReady(object sender, NetMQQueueEventArgs<NetMQMessage> e)
        {
            var message = e.Queue.Dequeue();
            if (message.Count() == 3)
                RaiseMessageReceived(message[1], message[2]);
        }

        #endregion


        #region Sending to StockTV

        public void SendToStockTV(MessageTopic topic, string value)
        {
            SendToStockTV(topic, Encoding.UTF8.GetBytes(value));
        }


        public void SendToStockTV(MessageTopic topic, byte[] value, byte[] additionalValue = null)
        {
            var frames = new List<NetMQFrame>()
            {
                new(topic.ToString()),
                new(value)
            };

            if (additionalValue != null)
            {
                frames.Add(new NetMQFrame(additionalValue));
            }

            _sendQueue?.Enqueue(frames);
        }


        public void SendToStockTV(MessageTopic topic)
        {
            var frames = new List<NetMQFrame>()
            {
                new(topic.ToString()),
                NetMQFrame.Empty
            };

            _sendQueue?.Enqueue(frames);
        }

        /// <summary>
        /// Sending a message from Queue to StockTV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendQueue_ReceiveReady(object sender, NetMQQueueEventArgs<List<NetMQFrame>> e)
        {
            List<NetMQFrame> frames = e.Queue.Dequeue();
            var m = new NetMQMessage();
            m.AppendEmptyFrame();

            foreach (var frame in frames)
            {
                m.Append(frame);
            }

            _socket.SendMultipartMessage(m);
            _logger.Info($"Send to StockTV: {ConvertToString(m)}");
        }

        private static string ConvertToString(NetMQMessage message)
        {
            try
            {
                return  message[1].ConvertToString() + "->" + string.Join("-", message[2].ToByteArray()); 
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
