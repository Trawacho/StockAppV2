using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace StockApp.UI.com;

public static class VereineFactory
{
    public static IEnumerable<IVerein> Load()
    {
        try
        {
            var json = File.ReadAllText(@"./com/Vereine.json");
            var vereine = JsonSerializer.Deserialize<IEnumerable<Verein>>(json);
            return vereine;
        }
        catch
        {
            return null;
        }
    }
}

