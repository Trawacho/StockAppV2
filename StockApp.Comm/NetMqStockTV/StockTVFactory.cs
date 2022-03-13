namespace StockApp.Comm.NetMqStockTV;

internal static class StockTVFactory
{
    internal static IStockTVSubscriberClient Create(string ipAddress, int port)
    {
        return new StockTVSubscriberClient(ipAddress, port);
    }
    internal static IStockTVAppClient Create(string ipAddress, int port, string identifier)
    {
        return new StockTVAppClient(ipAddress, port, identifier);
    }

    internal static IStockTVSettings CreateDefaultSettings(GameMode gameMode)
    {
        return StockTVSettings.CreateDefaultSetting(gameMode);
    }

    internal static IStockTVSettings CreateSettings(byte[] array)
    {
        var settings = new StockTVSettings();
        settings.SetSettings(array);
        return settings;
    }

    internal static IStockTVResult CreateResult()
    {
        return new StockTVResult();
    }
}

