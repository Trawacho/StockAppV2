using StockApp.Comm.Broadcasting;

namespace StockApp.Comm.NetMqStockTV;



public interface IStockTVResult : IEquatable<IStockTVResult>
{
    byte[] Data { get; }
    IStockTVSettings TVSettings { get; }
    IList<IStockTVGameResult> Results { get; }
    event Action ResultChanged;

    IBroadCastTelegram AsBroadCastTelegram();
    void SetResult(byte[] array);
}



/// <summary>
/// It´s a Result Message from StockTV, containing a List of Games with <see cref="StockTVGameResult"/> and also the settings from StockTV
/// </summary>
public class StockTVResult : IStockTVResult
{
    public event Action ResultChanged;
    protected virtual void RaiseResultChanged()
    {
        var handler = ResultChanged;
        handler?.Invoke();
    }

    /// <summary>
    /// Complete Message as Byte-Array from StockTV with <see cref="TVSettings"/> and <see cref="Results"/>
    /// </summary>
    public byte[] Data { get; private set; }


    public IStockTVSettings TVSettings { get; private set; }

    public IList<IStockTVGameResult> Results { private set; get; }

    public void SetResult(byte[] array)
    {
        if (Data?.Equals(array) ?? false) return;

        Data = array;

        if (TVSettings == null)
        {
            TVSettings = StockTVFactory.CreateSettings(array.Take(10).ToArray());
        }
        else
        {
            TVSettings?.SetSettings(array.Take(10).ToArray()); //_tvSettings = new StockTVSettings(array.Take(10).ToArray());
        }

        byte gamenumber = 1;

        Results.Clear();
        foreach (var item in array.Skip(10).Split(2))
        {
            Results.Add(
                new StockTVGameResult(gamenumber,
                                      item.First(),
                                      item.Last()));
            gamenumber++;
        }

        RaiseResultChanged();
    }

    public StockTVResult()
    {
        Results = new List<IStockTVGameResult>();

    }

    public StockTVResult(byte[] array) : this()
    {
        SetResult(array);
    }



    /// <summary>
    /// True, if Results and Settings are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IStockTVResult other)
    {
        return Data.Equals(other.Data);
    }


    public override string ToString()
    {
        return $"Gruppe: {TVSettings.Spielgruppe}; Bahn: {TVSettings.Bahn}; Spiele:{Results.Count} | {string.Join("-", Results.Select(x => String.Format("{0}:{1}", x.ValueA, x.ValueB)))}";
    }

    /// <summary>
    /// Creates a copy of the given StockTVResult
    /// </summary>
    /// <param name="stockTVResult"></param>
    /// <returns></returns>
#pragma warning disable IDE0051 // Remove unused private members
    private static StockTVResult GetCopy(StockTVResult stockTVResult)
#pragma warning restore IDE0051 // Remove unused private members
    {
        byte[] newVal = new byte[stockTVResult.Data.Length];
        Array.Copy(stockTVResult.Data, 0, newVal, 0, stockTVResult.Data.Length);

        return new StockTVResult(newVal);
    }

    public IBroadCastTelegram AsBroadCastTelegram()
    {
        return new BroadCastTelegram(this.Data);
    }
}
