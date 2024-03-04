using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace StockApp.UI.com;

public interface IVerein
{
    string Bundesland { get; set; }
    string Kreis { get; set; }
    string Name { get; set; }
    string Region { get; set; }
    string Land { get; set; }
}

//TODO: AutoComplete Textbox implementieren für Vereinsnamen; Laden und handeln der Vereine.json Datei;
public class Verein : IVerein
{
    public Verein() { }
    public string Name { get; set; }
    public string Kreis { get; set; }
    public string Bundesland { get; set; }
    public string Region { get; set; }
    public string Land { get; set; }    
}



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

