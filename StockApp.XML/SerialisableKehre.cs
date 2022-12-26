using StockApp.Core.Wettbewerb.Teambewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Kehre")]
public class SerialisableKehre : IKehre
{
    public SerialisableKehre()
    {

    }

    public SerialisableKehre(IKehre kehre)
    {
        KehrenNummer = kehre.KehrenNummer;
        PunkteTeamA = kehre.PunkteTeamA;
        PunkteTeamB = kehre.PunkteTeamB;
    }

    public int KehrenNummer { get; set; }

    public int PunkteTeamA { get; set; }

    public int PunkteTeamB { get; set; }

    public IKehre SetKehre(int teamA = int.MinValue, int teamB = int.MinValue)
    {
        throw new NotImplementedException();
    }
}
