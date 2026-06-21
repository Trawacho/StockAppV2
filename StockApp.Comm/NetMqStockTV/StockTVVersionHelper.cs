namespace StockApp.Comm.NetMqStockTV;

/// <summary>
/// Helper class to determine StockTV feature support based on firmware version
/// </summary>
public static class StockTVVersionHelper
{
    private static readonly Version Ziel2RequiredVersion = new Version(1, 6, 0, 0);

    /// <summary>
    /// Gets available GameModes based on StockTV firmware version
    /// </summary>
    /// <param name="firmwareVersion">Firmware version string (e.g., "1.6.0.0")</param>
    /// <returns>List of available GameModes for the given firmware version</returns>
    public static List<GameMode> GetAvailableGameModes(string firmwareVersion)
    {
        var allModes = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().ToList();

        if (!IsZiel2Supported(firmwareVersion))
        {
            allModes.Remove(GameMode.Ziel2);
        }

        return allModes;
    }

    /// <summary>
    /// Checks if Ziel2 game mode is supported (requires StockTV version > 1.6.0.0)
    /// </summary>
    public static bool IsZiel2Supported(string firmwareVersion)
    {
        if (string.IsNullOrWhiteSpace(firmwareVersion))
            return false;

        if (Version.TryParse(firmwareVersion, out var fwVersion))
        {
            return fwVersion > Ziel2RequiredVersion;
        }

        return false;
    }

    /// <summary>
    /// Validates that the given game mode is supported by the firmware version
    /// </summary>
    public static bool IsGameModeSupportedByVersion(GameMode gameMode, string firmwareVersion)
    {
        return gameMode switch
        {
            GameMode.Ziel2 => IsZiel2Supported(firmwareVersion),
            _ => true
        };
    }
}
