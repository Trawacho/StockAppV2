using StockApp.Core.Wettbewerb.Teambewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Spieler")]
public class SerialisablePlayer : IPlayer
{
    public SerialisablePlayer() { }
    public SerialisablePlayer(IPlayer player)
    {
        FirstName = player.FirstName;
        LastName = player.LastName;
        LicenseNumber = player.LicenseNumber;
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string LicenseNumber { get; set; }
}
