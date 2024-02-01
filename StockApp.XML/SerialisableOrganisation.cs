using StockApp.Core.Turnier;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Organisation")]
public class SerialisableOrganisation : IOrgaDaten
{
    public SerialisableOrganisation() { }

    public SerialisableOrganisation(IOrgaDaten orgaDaten)
    {
        Venue = orgaDaten.Venue;
        Organizer = orgaDaten.Organizer;
        DateOfTournament = orgaDaten.DateOfTournament.Date;
        Operator = orgaDaten.Operator; ;
        TournamentName = orgaDaten.TournamentName;
        Starttgebuehr = new SerialisableStartGebuehr(orgaDaten.EntryFee);
        Schiedsrichter = new SerialisableExecutive(orgaDaten.Referee);
        Wettbewerbsleiter = new SerialisableExecutive(orgaDaten.CompetitionManager);
        Rechenbuero = new SerialisableExecutive(orgaDaten.ComputingOfficer);
    }


    public string Venue { get; set; }
    public string Organizer { get; set; }
    public DateTime DateOfTournament { get; set; }
    public string Operator { get; set; }
    public string TournamentName { get; set; }
    public string Endtext { get; set; }
    public SerialisableStartGebuehr Starttgebuehr { get; set; }

    public SerialisableExecutive Schiedsrichter { get; set; }

    public SerialisableExecutive Wettbewerbsleiter { get; set; }

    public SerialisableExecutive Rechenbuero { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public IExecutive ComputingOfficer { get; set; }
    [XmlIgnore]
    public IExecutive CompetitionManager { get; set; }

    [XmlIgnore]
    public IExecutive Referee { get; set; }

    [XmlIgnore]
    public IStartgebuehr EntryFee { get; set; }

    #endregion

}

public static class SerialisableOrganisationExtension
{
    public static void ToNormal(this SerialisableOrganisation value, IOrgaDaten normal)
    {
        normal.Venue = value.Venue; ;
        normal.Organizer = value.Organizer;
        normal.DateOfTournament = value.DateOfTournament;
        normal.Operator = value.Operator;
        normal.TournamentName = value.TournamentName;
        normal.Endtext = value.Endtext;

        value.Starttgebuehr.ToNormal(normal.EntryFee);
        value.Rechenbuero.ToNormal(normal.ComputingOfficer);
        value.Schiedsrichter.ToNormal(normal.Referee);
        value.Wettbewerbsleiter.ToNormal(normal.CompetitionManager);

    }
}

