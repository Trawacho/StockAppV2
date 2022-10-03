namespace StockApp.Comm.NetMqStockTV
{

    public class StockTVGameResult : IStockTVGameResult
    {
        public StockTVGameResult(byte gameNumber, byte a, byte b)
        {
            Turns = new List<StockTVTurn>()
            {
                new StockTVTurn()
                {
                    TurnNumber = 1,
                    PointsA = Convert.ToByte(a),
                    PointsB = Convert.ToByte(b)
                }
            }.OrderBy(t => t.TurnNumber);

            GameNumber = gameNumber;
        }
        public StockTVGameResult(byte gameNumber, List<StockTVTurn> turns)
        {
            GameNumber = gameNumber;
            Turns = turns.OrderBy(t => t.TurnNumber);
        }


        public byte GameNumber { get; private set; }
        public byte ValueA => Convert.ToByte(Turns?.Sum(t => t.PointsA));
        public byte ValueB => Convert.ToByte(Turns?.Sum(t => t.PointsB));

        public IOrderedEnumerable<StockTVTurn> Turns { get; set; }
        public override string ToString()
        {
            return $"{ValueA}:{ValueB}";
        }
    }


    public interface IStockTVGameResult
    {
        byte GameNumber { get; }

        /// <summary>
        /// Summe der Werte der einzelnen Kehre 
        /// </summary>
        /// 
        byte ValueA { get; }

        /// <summary>
        /// Summe der Werte der einzelnen Kehre
        /// </summary>
        byte ValueB { get; }

        IOrderedEnumerable<StockTVTurn> Turns { get; }
    }
}
