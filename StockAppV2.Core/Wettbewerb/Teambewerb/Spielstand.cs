namespace StockApp.Core.Wettbewerb.Teambewerb;


/// <summary>
/// Spielstand in einem Spiel, mit den Punkten für Mannschaft A und Mannschaft B <br></br>
/// Die Werte können sowohl im Master als auch im Live gesetzt werden. Sobald Masterwerte gesetzt sind, können diese vom Live-Teil nicht mehr geändert werden, <see cref="IsSetByHand"/> TRUE wird
/// </summary>
public interface ISpielstand
{
    int Punkte_Master_TeamA { get; }
    int Punkte_Master_TeamB { get; }
    int Punkte_Live_TeamA { get; }
    int Punkte_Live_TeamB { get; }
    IOrderedEnumerable<IKehre> Kehren_Live { get; }
    IOrderedEnumerable<IKehre> Kehren_Master { get; }
    bool IsSetByHand { get; }
    void CopyLiveToMasterValues();
    void Reset(bool force = false);
    void SetLiveValues(int teamA, int teamB);
    void SetLiveValues(IOrderedEnumerable<IKehre> kehren);
    void SetMasterTeamAValue(int punkteTeamA);
    void SetMasterTeamBValue(int punkteTeamB);
    void SetMasterValue(IKehre kehre);
    int GetSpielPunkteTeamA(bool live);
    int GetSpielPunkteTeamB(bool live);
    int GetStockPunkteTeamB(bool live);
    int GetStockPunkteTeamA(bool live);

    int GetCountOfWinningTurnsTeamA(bool live);
    int GetCountOfWinningTurnsTeamB(bool live);

    event EventHandler SpielStandChanged;
}


/// <summary>
/// Spielstand in einem Spiel, mit den Punkten für Mannschaft A und Mannschaft B <br></br>
/// Die Werte können sowohl im Master als auch im Live gesetzt werden. Sobald Masterwerte gesetzt sind, können diese vom Live-Teil nicht mehr geändert werden, <see cref="IsSetByHand"/> TRUE wird
/// </summary>
public class Spielstand : ISpielstand
{

    #region Properties (all as private set)

    /// <summary>
    /// Punkte von TeamA
    /// </summary>
    public int Punkte_Master_TeamA { get => Kehren_Master?.Sum(k => k.PunkteTeamA) ?? 0; }

    /// <summary>
    /// Punkte von TeamB
    /// </summary>
    public int Punkte_Master_TeamB { get => Kehren_Master?.Sum(k => k.PunkteTeamB) ?? 0; }

    /// <summary>
    /// Punkte von TeamA aus dem NetzwerkService
    /// </summary>
    public int Punkte_Live_TeamA { get => Kehren_Live?.Sum(k => k.PunkteTeamA) ?? 0; }

    /// <summary>
    /// Punkte von TeamV aus dem NetzwerkService
    /// </summary>
    public int Punkte_Live_TeamB { get => Kehren_Live?.Sum(k => k.PunkteTeamB) ?? 0; }

    /// <summary>
    /// TRUE, wenn Werte in die Master-Punkte geschrieben werden (nicht, wenn  <see cref="CopyLiveToMasterValues"/> genutzt wird)
    /// </summary>
    public bool IsSetByHand { get; set; }

    public IOrderedEnumerable<IKehre> Kehren_Live { get; private set; }
    public IOrderedEnumerable<IKehre> Kehren_Master { get; private set; }

    #endregion

    public event EventHandler SpielStandChanged;
    protected virtual void RaiseSpielstandChanged()
    {
        var handler = SpielStandChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }


    #region Constructor

    /// <summary>
    /// Default-Constructor
    /// </summary>
    private Spielstand()
    {

    }
    public static ISpielstand Create() => new Spielstand();

    #endregion

    #region Set-Functions

    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE <br></br>
    /// und schreibt in <see cref="Punkte_Master_TeamA"/> und <see cref="Punkte_Live_TeamA"/> den übergebenen Wert
    /// </summary>
    /// <param name="punkteTeamA"></param>
    public void SetMasterTeamAValue(int punkteTeamA)
    {
        IsSetByHand = true;

        if (Kehren_Live == null || !Kehren_Live.Any())
        {
            Kehren_Live = new List<IKehre>() { Kehre.Create(1, punkteTeamA, 0) }.OrderBy(k => k.KehrenNummer);
        }
        else
        {
            Kehren_Live.First().PunkteTeamA = punkteTeamA;
        }

        if (Kehren_Master == null || !Kehren_Master.Any())
        {
            Kehren_Master = new List<IKehre>() { Kehre.Create(1, punkteTeamA, 0) }.OrderBy(k => k.KehrenNummer);
        }
        else
        {
            Kehren_Master.First().PunkteTeamA = punkteTeamA;
        }


        RaiseSpielstandChanged();
    }

    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE <br></br>
    /// und schreibt in <see cref="Punkte_Master_TeamB"/> und <see cref="Punkte_Live_TeamB"/> den übergebenen Wert
    /// </summary>
    /// <param name="punkteTeamB"></param>
    public void SetMasterTeamBValue(int punkteTeamB)
    {
        IsSetByHand = true;

        if (Kehren_Live == null || !Kehren_Live.Any())
        {
            Kehren_Live = new List<IKehre>() { Kehre.Create(1, 0, punkteTeamB) }.OrderBy(k => k.KehrenNummer);
        }
        else
        {
            Kehren_Live.First().PunkteTeamB = punkteTeamB;
        }

        if (Kehren_Master == null || !Kehren_Master.Any())
        {
            Kehren_Master = new List<IKehre>() { Kehre.Create(1, 0, punkteTeamB) }.OrderBy(k => k.KehrenNummer);
        }
        else
        {
            Kehren_Master.First().PunkteTeamB = punkteTeamB;
        }

        RaiseSpielstandChanged();
    }

    public void SetMasterValue(IKehre kehre)
    {
        IsSetByHand = true;


        //Kehre LIVE
        if (Kehren_Live == null || !Kehren_Live.Any())
        {
            Kehren_Live = new List<IKehre>() { kehre }.OrderBy(k => k.KehrenNummer);
        }
        else if (Kehren_Live.Any(k => k.KehrenNummer == kehre.KehrenNummer))
        {
            Kehren_Live.First(k => k.KehrenNummer == kehre.KehrenNummer).PunkteTeamA = kehre.PunkteTeamA;
            Kehren_Live.First(k => k.KehrenNummer == kehre.KehrenNummer).PunkteTeamB = kehre.PunkteTeamB;
        }
        else
        {
            var kehrenLive = Kehren_Live.ToList();
            kehrenLive.Add(kehre);
            Kehren_Live = kehrenLive.OrderBy(k => k.KehrenNummer);
        }

        //Kehre MASTER
        if (Kehren_Master == null || !Kehren_Master.Any())
        {
            Kehren_Master = new List<IKehre>() { kehre }.OrderBy(k => k.KehrenNummer);
        }
        else if (Kehren_Master.Any(k => k.KehrenNummer == kehre.KehrenNummer))
        {
            Kehren_Master.First(k => k.KehrenNummer == kehre.KehrenNummer).PunkteTeamA = kehre.PunkteTeamA;
            Kehren_Master.First(k => k.KehrenNummer == kehre.KehrenNummer).PunkteTeamB = kehre.PunkteTeamB;
        }
        else
        {
            var kehrenMaster = Kehren_Master.ToList();
            kehrenMaster.Add(kehre);
            Kehren_Master = kehrenMaster.OrderBy(k => k.KehrenNummer);
        }

        RaiseSpielstandChanged();
    }


    /// <summary>
    /// Wenn <see cref="IsSetByHand"/> FALSE ist, dann werden die Master-Punkte auf 0 gesetzt.<br></br>
    /// Sobald der Parameter <paramref name="force"/> TRUE ist, werden die Master-Punkte auf 0 gesetzt und <see cref="IsSetByHand"/> auf FALSE
    /// </summary>
    /// <param name="force"></param>
    public void Reset(bool force = false)
    {
        if (force) IsSetByHand = false;

        if (!IsSetByHand)
        {
            SetMasterTeamAValue(0);
            SetMasterTeamBValue(0);
            IsSetByHand = false;
            RaiseSpielstandChanged();
        }
    }

    /// <summary>
    /// Wemn <see cref="IsSetByHand"/> FALSE ist, dann werden die Live-Punkte in die Master-Punkte kopiert
    /// </summary>
    public void CopyLiveToMasterValues()
    {
        if (!IsSetByHand)
        {
            Kehren_Master = Kehren_Live.OrderBy(k => k.KehrenNummer);

            RaiseSpielstandChanged();
        }
    }

    /// <summary>
    /// Wenn <see cref="IsSetByHand"/> FALSE ist, dann werden die Live-Punkte gesetzt
    /// </summary>
    /// <param name="teamA"></param>
    /// <param name="teamB"></param>
    public void SetLiveValues(int teamA, int teamB)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Spielstand mit neuer Kehre schreiben -> TeamA:{teamA} - TeamB:{teamB} -> Kann geschrieben werden:{!IsSetByHand}");
#endif

        if (!IsSetByHand)
        {
            Kehren_Live = new List<IKehre>() { Kehre.Create(1, teamA, teamB) }.OrderBy(t => t.KehrenNummer);

            RaiseSpielstandChanged();
        }
    }

    public void SetLiveValues(IOrderedEnumerable<IKehre> kehren)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Spielstand schreiben. Anzahl an Kehren:{kehren.Count()} -> kann geschrieben werden:{!IsSetByHand}");
#endif

        if (!IsSetByHand)
        {
            this.Kehren_Live = kehren;
            RaiseSpielstandChanged();
        }
    }

    #endregion

    #region Get-Functions (Spielpunkte)

    public int GetSpielPunkteTeamA(bool live = false)
    {
        if (live)
        {
            if (Punkte_Live_TeamA == 0 && Punkte_Live_TeamB == 0)
                return 0;

            if (Punkte_Live_TeamA > Punkte_Live_TeamB)
                return 2;

            if (Punkte_Live_TeamB > Punkte_Live_TeamA)
                return 0;

            return 1;
        }
        else
        {
            if (Punkte_Master_TeamA == 0 && Punkte_Master_TeamB == 0)
                return 0;

            if (Punkte_Master_TeamA > Punkte_Master_TeamB)
                return 2;

            if (Punkte_Master_TeamB > Punkte_Master_TeamA)
                return 0;

            return 1;
        }
    }

    public int GetSpielPunkteTeamB(bool live = false)
    {
        if (live)
        {
            if (Punkte_Live_TeamA == 0 && Punkte_Live_TeamB == 0)
                return 0;

            if (Punkte_Live_TeamA > Punkte_Live_TeamB)
                return 0;

            if (Punkte_Live_TeamB > Punkte_Live_TeamA)
                return 2;

            return 1;
        }
        else
        {
            if (Punkte_Master_TeamA == 0 && Punkte_Master_TeamB == 0)
                return 0;

            if (Punkte_Master_TeamA > Punkte_Master_TeamB)
                return 0;

            if (Punkte_Master_TeamB > Punkte_Master_TeamA)
                return 2;

            return 1;
        }
    }



    public int GetStockPunkteTeamA(bool live = false) => live ? Punkte_Live_TeamA : Punkte_Master_TeamA;

    public int GetStockPunkteTeamB(bool live = false) => live ? Punkte_Live_TeamB : Punkte_Master_TeamB;

    public int GetCountOfWinningTurnsTeamA(bool live) => live
            ? Kehren_Live?.Count(k => k.PunkteTeamA > k.PunkteTeamB) ?? 0
            : Kehren_Master?.Count(k => k.PunkteTeamA > k.PunkteTeamB) ?? 0;

    public int GetCountOfWinningTurnsTeamB(bool live) => live
            ? Kehren_Live?.Count(k => k.PunkteTeamB > k.PunkteTeamA) ?? 0
            : Kehren_Master?.Count(k => k.PunkteTeamB > k.PunkteTeamA) ?? 0;

    #endregion

}
