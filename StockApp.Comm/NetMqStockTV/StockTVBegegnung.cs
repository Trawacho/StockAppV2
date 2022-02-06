namespace StockApp.Comm.NetMqStockTV
{
    public class StockTVBegegnung
    {
        public StockTVBegegnung()
        {

        }
        public StockTVBegegnung(int spielNummer, string teamNameA, string teamNameB) : this()
        {
            SpielNummer = spielNummer;
            TeamNameA = teamNameA;
            TeamNameB = teamNameB;
        }
        public int SpielNummer { get; set; }
        public string TeamNameA { get; set; }
        public string TeamNameB { get; set; }
    }
}
