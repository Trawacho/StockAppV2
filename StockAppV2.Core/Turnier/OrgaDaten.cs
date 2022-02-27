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
    private string _venue;
    private string _organizer;
    private DateTime _dateOfTournament;
    private string _operator;
    private string _tournamentName;

    /// <summary>
    /// Veranstaltungsort
    /// </summary>
    public string Venue { get => _venue; set => _venue = value.Trim(); }

    /// <summary>
    /// Organisator / Veranstalter
    /// </summary>
    public string Organizer { get => _organizer; set => _organizer = value.Trim(); }

    /// <summary>
    /// Datum / Zeit des Turniers
    /// </summary>
    public DateTime DateOfTournament { get => _dateOfTournament; set => _dateOfTournament = value; }

    /// <summary>
    /// Durchführer
    /// </summary>
    public string Operator { get => _operator; set => _operator = value.Trim(); }

    /// <summary>
    /// Turniername
    /// </summary>
    public string TournamentName { get => _tournamentName; set => _tournamentName = value.Trim(); }

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
