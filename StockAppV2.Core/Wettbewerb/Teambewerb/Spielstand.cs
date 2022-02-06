namespace StockApp.Core.Wettbewerb.Teambewerb;
public interface ISpielstand
{
    public int Punkte_Master_TeamA { get; }
    public int Punkte_Master_TeamB { get; }
    public int Punkte_Live_TeamA { get; }
    public int Punkte_Live_TeamB { get; }
    public bool IsSetByHand { get; }
    void CopyLiveToMasterValues();
    void Reset(bool force = false);
    void SetLiveValues(int teamA, int teamB);
    void SetMasterTeamAValue(int punkteTeamA);
    void SetMasterTeamBValue(int punkteTeamB);
    int GetSpielPunkteTeamA(bool live);
    int GetSpielPunkteTeamB(bool live);
    int GetStockPunkteTeamB(bool live);
    int GetStockPunkteTeamA(bool live);

    event EventHandler SpielStandChanged;
}


/// <summary>
/// Spielstand in einem Spiel, mit den Punkten für Mannschaft A und Mannschaft B <br>    /// </br>
/// Die Werte können sowohl im Master als auch im Live gesetzt werden. Sobald Masterwerte gesetzt sind, können diese vom Live-Teil nicht mehr geändert werden, <see cref="IsSetByHand"/> TRUE wird
/// </summary>
public class Spielstand : ISpielstand
{

    #region Properties (all as private set)

    /// <summary>
    /// Punkte von TeamA
    /// </summary>
    public int Punkte_Master_TeamA { get; set; }

    /// <summary>
    /// Punkte von TeamB
    /// </summary>
    public int Punkte_Master_TeamB { get; set; }

    /// <summary>
    /// Punkte von TeamA aus dem NetzwerkService
    /// </summary>
    public int Punkte_Live_TeamA { get; set; }

    /// <summary>
    /// Punkte von TeamV aus dem NetzwerkService
    /// </summary>
    public int Punkte_Live_TeamB { get; set; }

    /// <summary>
    /// TRUE, wenn Werte in die Master-Punkte geschrieben werden (nicht, wenn  <see cref="CopyLiveToMasterValues"/> genutzt wird)
    /// </summary>
    public bool IsSetByHand { get; set; }

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
        Punkte_Master_TeamA = punkteTeamA;
        Punkte_Live_TeamA = punkteTeamA;
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
        Punkte_Live_TeamB = punkteTeamB;
        Punkte_Master_TeamB = punkteTeamB;
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
            Punkte_Master_TeamA = Punkte_Live_TeamA;
            Punkte_Master_TeamB = Punkte_Live_TeamB;
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
        if (!IsSetByHand)
        {
            Punkte_Live_TeamA = teamA;
            Punkte_Live_TeamB = teamB;
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

    #endregion

}
