namespace StockApp.Comm.NetMqStockTV
{
    public interface IStockTVGameResult
    {
        public byte GameNumber { get; }
        public byte ValueA { get; }
        public byte ValueB { get; }

    }
    public class StockTVGameResult : IStockTVGameResult
    {
        public StockTVGameResult(byte gameNumber, byte a, byte b)
        {
            ValueA = Convert.ToByte(a);
            ValueB = Convert.ToByte(b);
            GameNumber = gameNumber;
        }
        public byte GameNumber { get; private set; }
        public byte ValueA { get; private set; }
        public byte ValueB { get; private set; }
        public override string ToString()
        {
            return $"{ValueA}:{ValueB}";
        }
    }
}
