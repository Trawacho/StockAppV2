using System.Text.Json.Serialization;

namespace StockApp.Comm.NetMqStockTV;

public class StockTVTurn : IStockTVTurn
{
    [JsonPropertyName("TurnNumber")]
    public int TurnNumber { get; set; }

    [JsonPropertyName("PointsLeft")]
    public byte PointsB { get; set; }

    [JsonPropertyName("PointsRight")]
    public byte PointsA { get; set; }
}

public interface IStockTVTurn
{
    int TurnNumber { get; set; }
    byte PointsB { get; set; }
    byte PointsA { get; set; }
}
