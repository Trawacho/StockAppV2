using StockApp.Comm.NetMqStockTV;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace StockApp.Test.UI.TestDoubles;

/// <summary>
/// Minimaler Test-Double für <see cref="IStockTV"/>. Die echte <see cref="StockTV"/>-Klasse
/// baut im Konstruktor eine echte Netzwerkverbindung auf (siehe CLAUDE.md NetMQ-Hinweise) und
/// ist damit für Unit-Tests ungeeignet - hier reicht die Interface-Oberfläche mit manuell
/// auslösbaren Events.
/// </summary>
public class FakeStockTV : IStockTV
{
    public event EventHandler RemoveFromCollectionRequested;
#pragma warning disable CS0067 // von IStockTV gefordert, in Tests bisher nicht benötigt
    public event EventHandler<StockTVResultChangedEventArgs> StockTVResultChanged;
#pragma warning restore CS0067
    public event PropertyChangedEventHandler StockTVSettingsChanged;
    public event EventHandler<bool> StockTVOnlineChanged;
    public event EventHandler<bool> StockTVDirectorChanged;

    public Guid StockTVId { get; } = Guid.NewGuid();
    public string HostName { get; set; } = "fake-host";
    public string IPAddress { get; set; } = "127.0.0.1";
    public string FW { get; set; } = "1.0";
    public bool IsConnected { get; set; } = true;
    public bool UpdateImmediately { get; set; }
    public string Url => $"http://{IPAddress}";
    public bool Director { get; set; }
    public IStockTVSettings TVSettings { get; } = new StockTVSettings();

    public int DisposeCallCount { get; private set; }

    public void TVSettingsGet() { }
    public void TVSettingsSend() { }
    public void Connect() { }
    public void Disconnect() { }
    public void TVResultGet() { }
    public void TVResultReset() { }
    public void SetMarketingImage(byte[] imageAsByteArray, string fileName) { }
    public void ClearMarketingImage() { }
    public void ShowMarketing() { }
    public void SendTeamNames(IEnumerable<StockTVBegegnung> begegnungen, int gameOffset) { }
    public void SendTeilnehmer(string teilnehmer) { }
    public void RemoveFromCollection() => RemoveFromCollectionRequested?.Invoke(this, EventArgs.Empty);

    public void Dispose() => DisposeCallCount++;

    public bool Equals(IStockTV other) => ReferenceEquals(this, other);
    public override bool Equals(object obj) => Equals(obj as IStockTV);
    public override int GetHashCode() => StockTVId.GetHashCode();
    public int CompareTo(IStockTV other) => 0;

    public void RaiseSettingsChanged(string propertyName) => StockTVSettingsChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public void RaiseOnlineChanged(bool value) => StockTVOnlineChanged?.Invoke(this, value);
    public void RaiseDirectorChanged(bool value) => StockTVDirectorChanged?.Invoke(this, value);
}
