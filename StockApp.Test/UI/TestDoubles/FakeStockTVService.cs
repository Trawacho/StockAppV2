using StockApp.Comm.NetMqStockTV;
using System;
using System.Collections.Generic;

namespace StockApp.Test.UI.TestDoubles;

public class FakeStockTVService : IStockTVService
{
    private readonly List<IStockTV> _stockTvList = new();

    public IEnumerable<IStockTV> StockTVCollection => _stockTvList;

    public int GameOffset { get; set; }

    public event EventHandler<StockTVCollectionChangedEventArgs> StockTVCollectionChanged;
#pragma warning disable CS0067 // von IStockTVService gefordert, in Tests bisher nicht benötigt
    public event EventHandler<StockTVResultChangedEventArgs> StockTVResultChanged;
#pragma warning restore CS0067

    public void Discover() { }
    public void AddManual(string hostname, string ipAddress) { }

    public void AddStockTv(IStockTV stockTv)
    {
        _stockTvList.Add(stockTv);
        StockTVCollectionChanged?.Invoke(this, new StockTVCollectionChangedEventArgs(added: true));
    }

    public void RemoveStockTv(IStockTV stockTv)
    {
        _stockTvList.Remove(stockTv);
        StockTVCollectionChanged?.Invoke(this, new StockTVCollectionChangedEventArgs(added: false));
    }

    public void RaiseCollectionChanged(bool added) => StockTVCollectionChanged?.Invoke(this, new StockTVCollectionChangedEventArgs(added));

    public void Dispose() { }
}
