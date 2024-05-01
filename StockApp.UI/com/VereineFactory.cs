using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockApp.UI.com;

public static class VereineFactory
{
    public static IEnumerable<IVerein> Load()
    {
        try
        {
            var resourceName = "StockApp.UI.com.Vereine.json";
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stram = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stram);
            var json = reader.ReadToEnd();
            var vereine = JsonSerializer.Deserialize<IEnumerable<Verein>>(json);
            return vereine;
        }
        catch
        {
            return null;
        }
    }
}