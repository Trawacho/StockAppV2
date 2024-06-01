namespace StockApp.UI.com;

public interface IVerein
{
    string Bundesland { get; set; }
    string Kreis { get; set; }
    string Name { get; set; }
    string Region { get; set; }
    string Land { get; set; }
}

public class Verein : IVerein
{
    public Verein() { }
    public string Name { get; set; }
    public string Kreis { get; set; }
    public string Bundesland { get; set; }
    public string Region { get; set; }
    public string Land { get; set; }
}

