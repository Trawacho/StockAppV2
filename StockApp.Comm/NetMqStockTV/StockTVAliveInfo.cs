namespace StockApp.Comm.NetMqStockTV;

public interface IStockTVAliveInfo
{
    string HostName { get; set; }
    string AppVersion { get; set; }
    string IpAddress { get; set; }
}

internal class StockTVAliveInfo : IStockTVAliveInfo
{
    public string IpAddress { get; set; }
    public string HostName { get; set; }
    public string AppVersion { get; set; }
}