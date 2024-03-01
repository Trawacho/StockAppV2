using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace StockApp.UI.com;
//TODO: AutoComplete Textbox implementieren für Vereinsnamen; Laden und handeln der Vereine.json Datei;
public class Verein
{
    public Verein() { }
    public string Name { get; set; }
    public string Kreis { get; set; }
    public string Bundesland { get; set; }
    public string Region { get; set; }
}

public static class VereineFactory
{
    public static IEnumerable<Verein> Load()
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

