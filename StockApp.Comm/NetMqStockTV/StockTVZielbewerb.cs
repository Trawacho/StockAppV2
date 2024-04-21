namespace StockApp.Comm.NetMqStockTV;

public interface IStockTVZielbewerb
{
    StockTVZielDisziplin MassenVorne { get; set; }
    StockTVZielDisziplin Schiessen { get; set; }
    StockTVZielDisziplin MassenSeite { get; set; }
    StockTVZielDisziplin Kombinieren { get; set; }

    /// <summary>
    /// TRUE, wenn in den Disziplinen keine Werte vorhanden sind
    /// </summary>
    /// <returns></returns>
    bool HasNoAttempts();
    IEnumerable<IStockTVZielDisziplin> Disziplinen();
}

public class StockTVZielbewerb : IStockTVZielbewerb
{
    public StockTVZielDisziplin MassenVorne { get; set; }

    public StockTVZielDisziplin Schiessen { get; set; }

    public StockTVZielDisziplin MassenSeite { get; set; }

    public StockTVZielDisziplin Kombinieren { get; set; }
   
    public StockTVZielbewerb()
    {

    }

    public bool HasNoAttempts()
    {
        return !MassenVorne.Versuche.Any() &&
               !Schiessen.Versuche.Any() &&
               !MassenSeite.Versuche.Any() &&
               !Kombinieren.Versuche.Any();
    }

    public IEnumerable<IStockTVZielDisziplin> Disziplinen()
    {
        yield return MassenVorne;
        yield return Schiessen;
        yield return MassenSeite;
        yield return Kombinieren;
    }
}
