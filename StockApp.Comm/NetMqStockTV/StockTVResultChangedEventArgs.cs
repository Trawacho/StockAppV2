namespace StockApp.Comm.NetMqStockTV;

public class StockTVResultChangedEventArgs : EventArgs
{
    public StockTVResultChangedEventArgs()
    {

    }

    public StockTVResultChangedEventArgs(IStockTVResult tVResult) : this()
    {
        TVResult = tVResult;
    }

    public IStockTVResult TVResult { get; }
}
