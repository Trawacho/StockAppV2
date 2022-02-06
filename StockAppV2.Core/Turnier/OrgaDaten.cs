namespace StockApp.Core.Turnier;

public interface IOrgaDaten
{
    /// <summary>
    /// Veranstaltungsort
    /// </summary>
    public string Venue { get; set; }
    /// <summary>
    /// Organisator
    /// </summary>
    public string Organizer { get; set; }
    /// <summary>
    /// Tag des Turnier
    /// </summary>
    public DateTime DateOfTournament { get; set; }
    /// <summary>
    /// Durchführer
    /// </summary>
    public string Operator { get; set; }
    /// <summary>
    /// Turnierbezeichnung
    /// </summary>
    public string TournamentName { get; set; }
    public IStartgebuehr EntryFee { get; set; }
    public IExecutive Referee { get; set; }
    public IExecutive CompetitionManager { get; set; }
    public IExecutive ComputingOfficer { get; set; }
}

public class OrgaDaten : IOrgaDaten
{
    /// <summary>
    /// Veranstaltungsort
    /// </summary>
    public string Venue { get; set; }

    /// <summary>
    /// Organisator / Veranstalter
    /// </summary>
    public string Organizer { get; set; }

    /// <summary>
    /// Datum / Zeit des Turniers
    /// </summary>
    public DateTime DateOfTournament { get; set; }

    /// <summary>
    /// Durchführer
    /// </summary>
    public string Operator { get; set; }

    /// <summary>
    /// Turniername
    /// </summary>
    public string TournamentName { get; set; }

    /// <summary>
    /// Startgebühr pro Mannschaft
    /// </summary>
    public IStartgebuehr EntryFee { get; set; }
    public IExecutive Referee { get; set; }
    public IExecutive CompetitionManager { get; set; }
    public IExecutive ComputingOfficer { get; set; }


    public OrgaDaten()
    {
        this.DateOfTournament = DateTime.Now;
        this.EntryFee = new Startgebuehr();
        this.Referee = new Schiedsrichter();
        this.CompetitionManager = new Wettbewerbsleiter();
        this.ComputingOfficer = new Rechenbuero();
    }

}
