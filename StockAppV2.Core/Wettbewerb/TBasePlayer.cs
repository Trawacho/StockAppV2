namespace StockApp.Core.Wettbewerb;
public abstract class TBasePlayer
{
    private string _firstName;
    private string _lastName;
    private string _licenseNumber;

    public string FirstName { get => _firstName; set => _firstName = value.Trim(); }

    public string LastName { get => _lastName; set => _lastName = value.Trim(); }

    public string LicenseNumber { get => _licenseNumber; set => _licenseNumber = value.Trim(); }

    public string Name => $"{(string.IsNullOrEmpty(LastName) ? "NACHNAME" : LastName.ToUpper())}, {(string.IsNullOrEmpty(FirstName) ? "Vorname" : FirstName)}";

}
