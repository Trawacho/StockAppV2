namespace StockApp.Comm.NetMqStockTV;

public interface IStockTVZielDisziplin
{
    StockTVZielDisziplinName Name { get; }
    List<byte> Versuche { get; set; }
}

public class StockTVZielDisziplin : IStockTVZielDisziplin
{
    public List<byte> Versuche { get; set; }

    public StockTVZielDisziplinName Name { get; set; }

    public StockTVZielDisziplin()
    {
            
    }

    public StockTVZielDisziplin(StockTVZielDisziplinName name, IEnumerable<byte> versuche)
    {
        Name = name;
        Versuche = new List<byte>(versuche);
    }
}
