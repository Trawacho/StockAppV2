namespace StockApp.Comm.NetMqStockTV;

public class StockTVGame : IStockTVGame
{
    public byte GameNumber { get; set; }

    public List<StockTVTurn> Turns { get; set; }
}
public interface IStockTVGame
{
    byte GameNumber { get; set; }

    List<StockTVTurn> Turns { get; set; }
}
