namespace StockApp.Core.Wettbewerb;
public abstract class TBasePlayer
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string LicenseNumber { get; set; }

    public string Name => $"{(string.IsNullOrEmpty(LastName) ? "NACHNAME" : LastName.ToUpper())}, {(string.IsNullOrEmpty(FirstName) ? "Vorname" : FirstName)}";

}
