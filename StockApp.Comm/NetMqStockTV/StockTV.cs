using StockApp.Comm.MDns;
using System.ComponentModel;
using System.Diagnostics;

namespace StockApp.Comm.NetMqStockTV;

public interface IStockTV : IEquatable<IStockTV>, IComparable<IStockTV>, IDisposable
{
    event EventHandler RemoveFromCollectionRequested;
    event EventHandler<StockTVResultChangedEventArgs> StockTVResultChanged;
    event PropertyChangedEventHandler StockTVSettingsChanged;
    event EventHandler<bool> StockTVOnlineChanged;
    event EventHandler<bool> StockTVDirectorChanged;
    Guid StockTVId { get; }

    string HostName { get; }
    string IPAddress { get; }
    string FW { get; }
    bool IsConnected { get; }
    bool UpdateImmediately { get; set; }
    string Url { get; }

    bool Director { get; set; }

    IStockTVSettings TVSettings { get; }

    void TVSettingsGet();
    void TVSettingsSend();

    void Connect();
    void Disconnect();

    void TVResultGet();
    void TVResultReset();

    void SetMarketingImage(byte[] imageAsByteArray, string fileName);
    void ClearMarketingImage();
    void ShowMarketing();

    void SendTeamNames(IEnumerable<StockTVBegegnung> begegnungen);

    /// <summary>
    /// Send Name to StockTV for Zielbewerb
    /// </summary>
    /// <param name="teilnehmer"></param>
    void SendTeilnehmer(string teilnehmer);

    void RemoveFromCollection();
}

public class StockTV : IStockTV
{
    #region EventHandler

    public event EventHandler<bool> StockTVDirectorChanged;
    protected virtual void RaiseStockTVDirectorChanged(bool isDirector)
    {
        var handler = StockTVDirectorChanged;
        handler?.Invoke(this, isDirector);
    }

    public event EventHandler RemoveFromCollectionRequested;
    protected virtual void RaiseRemoveFromCollectionRequested()
    {
        var handler = RemoveFromCollectionRequested;
        handler?.Invoke(this, EventArgs.Empty);
    }



    public event EventHandler<StockTVResultChangedEventArgs> StockTVResultChanged;
    protected void RaiseStockTVResultChanged()
    {
        var handler = StockTVResultChanged;
        handler?.Invoke(this, new StockTVResultChangedEventArgs(TVResult));
    }


    public event PropertyChangedEventHandler StockTVSettingsChanged;
    private void RaiseStockTVSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        var handler = StockTVSettingsChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        if (UpdateImmediately)
            TVSettingsSend();
    }


    public event EventHandler<bool> StockTVOnlineChanged;
    protected void RaiseStockTVOnlineChanged()
    {
        var handler = StockTVOnlineChanged;
        handler?.Invoke(this, this.IsConnected);
    }


    #endregion

    #region IEquatable- and ICompareable Implementation

    /// <summary>
    /// True if Hostname is equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IStockTV other)
    {
        return this.HostName.Equals(other.HostName) &&
            this.IPAddress.Equals(other.IPAddress) &&
            this.TVSettings.Bahn.Equals(other.TVSettings.Bahn);
    }

    public int CompareTo(IStockTV other)
    {
        var equalGameGroup = TVSettings.Spielgruppe.CompareTo(other.TVSettings.Spielgruppe);
        if (equalGameGroup != 0)
            return equalGameGroup;

        var equalBahn = TVSettings.Bahn.CompareTo(other.TVSettings.Bahn);
        if (equalBahn != 0)
            return equalBahn;

        var equalHostName = this.HostName.CompareTo(other.HostName);
        if (equalHostName != 0)
            return equalHostName;

        return this.IPAddress.CompareTo(other.IPAddress);

    }

    #endregion

    #region Fields

    //private readonly IMDnsInformation _mDNSInformation;
    private readonly IMDnsHost _mDnsHost;
    private IStockTVAppClient _appClient;
    private IStockTVSubscriberClient _subscriberClient;
    private readonly List<string> _infos;
    private bool _isOnline;
    private bool _disposed;
    private bool _director;
    #endregion

    #region Properties

    /// <summary>
    /// True, if <see cref="_appClient"/>.IsConnected is True
    /// </summary>
    public bool IsConnected
    {
        get => _isOnline;
        private set
        {
            _isOnline = value;
            RaiseStockTVOnlineChanged();
        }
    }

    public bool UpdateImmediately { get; set; }

    #endregion

    #region Read-only Properties

    public Guid StockTVId { get; }
    public bool Director
    {
        get => _director;
        set
        {
            _director = value;
            RaiseStockTVDirectorChanged(value);
        }
    }
    public IEnumerable<string> Informationen => _infos;
    public string FW => _mDnsHost.Version;
    public string IPAddress => _mDnsHost.IPAddress;
    public string HostName => _mDnsHost.HostName;

    public string Url => $@"http://{IPAddress}:8080/#Apps%20manager";

    public IStockTVSettings TVSettings { get; }

    /// <summary>
    /// Updated through the <see cref="_subscriberClient"/>
    /// </summary>
    public IStockTVResult TVResult { get; }

    #endregion

    #region Konstruktor

    internal StockTV(IMDnsHost mDnsHost)
    {
        _mDnsHost = mDnsHost;
        _infos = new List<string>();
        TVSettings = StockTVFactory.CreateDefaultSettings(GameMode.Training);
        TVResult = StockTVFactory.CreateResult();
        TVResult.ResultChanged += RaiseStockTVResultChanged;
        TVSettings.SettingsChanged += RaiseStockTVSettingsChanged;
        Connect();
        StockTVId = new Guid();
    }

    #endregion

    #region IDisposable 

    public void Dispose()
    {
        this.Dispose(true);
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
                TVResult.ResultChanged -= RaiseStockTVResultChanged;
                TVSettings.SettingsChanged -= RaiseStockTVSettingsChanged;

                RemoveFromCollectionRequested = null;

                StockTVResultChanged = null;
                StockTVSettingsChanged = null;

                _appClient.ConnectedChanged -= AppClient_ConnectedChanged;
                _appClient.MessageReceived -= NetMQMessage_Received;
                _appClient?.Dispose();
                _appClient = null;

                _subscriberClient.SubscriberMessageReceived -= NetMQMessage_Received;
                _subscriberClient?.Dispose();
                _subscriberClient = null;
                _infos.Clear();

            }
            _disposed = true;
        }
    }

    #endregion

    #region Methods 
    public void RemoveFromCollection() => RaiseRemoveFromCollectionRequested();

    #region (Connect/Disconnet) Start and Stop Network clients (netMQ)

    /// <summary>
    /// Start the <see cref="_subscriberClient"/> and the <see cref="_appClient"/>
    /// </summary>
    public void Connect()
    {
        if (_appClient == null)
        {
            _appClient = StockTVFactory.Create(IPAddress, this._mDnsHost.ControlServicePort, HostName);
            _appClient.ConnectedChanged += AppClient_ConnectedChanged;
            _appClient.MessageReceived += NetMQMessage_Received;
        }
        _appClient.Start();


        if (_subscriberClient == null)
        {
            _subscriberClient = StockTVFactory.Create(IPAddress, this._mDnsHost.PublisherServicePort);
            _subscriberClient.SubscriberMessageReceived += NetMQMessage_Received;
        }
        _subscriberClient.Start();


    }

    /// <summary>
    /// Stops the <see cref="_subscriberClient"/> and the <see cref="_appClient"/>
    /// </summary>
    public void Disconnect()
    {
        _subscriberClient?.Stop();
        _appClient?.Stop();
    }

    #endregion

    #region NetMQ - Message received / ConnectedChanged

    private void AppClient_ConnectedChanged(object sender, bool isConnected)
    {
        IsConnected = isConnected;
        if (isConnected) TVSettingsGet();
    }

    private void NetMQMessage_Received(object sender, StockTVMessageReceivedEventArgs args)
    {
        if (args.MessageTopic == MessageTopic.Alive)
        {
            if (args.MessageValue.Length > 0)
            {
                var aliveInfo = System.Text.Json.JsonSerializer.Deserialize<StockTVAliveInfo>(args.MessageValue);
                this._mDnsHost.Update(aliveInfo);
            }
            return;
        }

#if DEBUG
        Debug.WriteLine(sender.ToString() + " Receive: " + args.MessageTopic + " ==> " + string.Join("-", args));
#endif

        if (args.MessageTopic == MessageTopic.GetResult)
        {
            TVResult?.SetResult(args.MessageValue);
        }
        else if (args.MessageTopic == MessageTopic.GetSettings)
        {
            TVSettings.SetSettings(args.MessageValue);
        }
    }

    #endregion

    #region Methods to send commands to StockTV

    /// <summary>
    /// Returns <see cref="StockTVResult"/> after sending a request to StockTV
    /// </summary>
    /// <returns></returns>
    public void TVResultGet() => _appClient?.SendToStockTV(MessageTopic.GetResult);

    public void TVResultReset() => _appClient?.SendToStockTV(MessageTopic.ResetResult);

    public void TVSettingsGet() => _appClient?.SendToStockTV(MessageTopic.GetSettings);

    public void TVSettingsSend() => _appClient?.SendToStockTV(MessageTopic.SetSettings, TVSettings.GetSettings());

    public void SendTeamNames(IEnumerable<StockTVBegegnung> begegnungen)
    {
        string valueString = string.Empty;
        foreach (var b in begegnungen)
        {
            valueString += b.GetStockTVString(TVSettings.NextBahnModus == NextCourtMode.Left);
        }
        _appClient?.SendToStockTV(MessageTopic.SetTeamNames, valueString);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="teilnehmer"></param>
    public void SendTeilnehmer(string teilnehmer)
    {
        _appClient?.SendToStockTV(MessageTopic.SetTeilnehmer, teilnehmer);
    }

    public void SetMarketingImage(byte[] imageAsByteArray, string fileName) =>
        _appClient?.SendToStockTV(MessageTopic.SetImage, imageAsByteArray, System.Text.Encoding.UTF8.GetBytes(fileName));

    public void ShowMarketing() =>
        _appClient?.SendToStockTV(MessageTopic.GoToImage);

    public void ClearMarketingImage() =>
        _appClient?.SendToStockTV(MessageTopic.ClearImage);

    #endregion

    #endregion
}
