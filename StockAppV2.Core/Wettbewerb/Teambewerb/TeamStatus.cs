namespace StockApp.Core.Wettbewerb.Teambewerb;

public enum TeamStatus
{
    /// <summary>
    /// Normal
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Entschuldigt, nicht angetreten
    /// </summary>
    Entschuldigt = 1,

    /// <summary>
    /// Unentschuldigt nicht angetreten
    /// </summary>
    Unentschuldigt = 2,

    /// <summary>
    /// Mannschaft scheidet vorzeitig aus dem Wettbewerb aus
    /// </summary>
    Vorzeitig = 3
}

public static class TeamStatusExtension
{
    private static readonly Dictionary<TeamStatus, (string name, string description, string abbreviation)> _names = new();
    static TeamStatusExtension()
    {
        _names.Add(TeamStatus.Normal, ("Normal", "", ""));
        _names.Add(TeamStatus.Entschuldigt, ("Entschuldigt", "Entschuldigt nicht angetreten", "e.n.a."));
        _names.Add(TeamStatus.Unentschuldigt, ("Unentschuldigt", "Unentschuldigt nicht angetreten", "u.n.a."));
        _names.Add(TeamStatus.Vorzeitig, ("Vorzeitig ausgeschieden", "Vorzeitig ausgeschieden", "v.a."));
    }
    public static string Name(this TeamStatus status) => _names[status].name;
    public static string Description(this TeamStatus status) => _names[status].description;
    public static string Abbreviation(this TeamStatus status) => _names[status].abbreviation;
    public static TeamStatus FromName(string name) => _names.Where(w => w.Value.name == name).FirstOrDefault().Key;

    public static string FooterText()
    {
        var returnValue = "";
        foreach (var item in _names.Where(v => v.Key != TeamStatus.Normal))
        {
            if (returnValue.Length > 3) returnValue += "; ";
            returnValue += item.Value.abbreviation + " = " + item.Value.description;
        }
        return returnValue;
    }
}


