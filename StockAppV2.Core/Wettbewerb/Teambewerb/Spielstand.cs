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

    /// <summary>
    /// Wemn <see cref="IsSetByHand"/> FALSE ist, dann werden die Live-Punkte in die Master-Punkte kopiert
    /// </summary>
    void CopyLiveToMasterValues();

    /// <summary>
    /// Wenn <see cref="IsSetByHand"/> FALSE ist, dann werden die Master-Punkte auf 0 gesetzt.<br></br>
    /// Sobald der Parameter <paramref name="force"/> TRUE ist, werden die Master-Punkte auf 0 gesetzt und <see cref="IsSetByHand"/> auf FALSE
    /// </summary>
    /// <param name="force"></param>
    void Reset(bool force = false);

    /// <summary>
    /// Wenn <see cref="IsSetByHand"/> FALSE ist, werden erst alle Kehren in <see cref="Kehren_Live"/> gelöscht, <br>
    /// </br> und eine neue Kehre mit den Werten angefügt
    /// </summary>
    /// <param name="teamA"></param>
    /// <param name="teamB"></param>
    void SetLiveValues(int teamA, int teamB);

    /// <summary>
    /// Wenn <see cref="IsSetByHand"/> TRUE ist, werden alle Kehren in <see cref="Kehren_Live"/> gelöscht, <br>
    /// </br>und dann alle übergegebenen Kehren angefügt
    /// </summary>
    /// <param name="kehren"></param>
    void SetLiveValues(IOrderedEnumerable<IKehre> kehren);

    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE <br></br>
    /// und schreibt in die erste Kehre von <see cref="Kehren_Master"/> und <see cref="Kehren_Live"/> den übergebenen Wert.<br></br>
    /// !! Alle anderen Kehren werden gelöscht !!
    /// </summary>
    /// <param name="punkteTeamA"></param>
    void SetMasterTeamAValue(int punkteTeamA);
    
    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE <br></br>
    /// und schreibt in die erste Kehre von <see cref="Kehren_Master"/> und <see cref="Kehren_Live"/> den übergebenen Wert.<br></br>
    /// !! Alle anderen Kehren werden gelöscht !!
    /// </summary>
    /// <param name="punkteTeamB"></param>
    void SetMasterTeamBValue(int punkteTeamB);

    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE<br></br>
    /// Es wird die Kehre in  <see cref="Kehren_Live"/> und <see cref="Kehren_Master"/> gelöscht und dann neu angefügt
    /// </summary>
    /// <param name="kehre"></param>
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
    #region Properties (all as readonly)

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

    #endregion

    /// <summary>
    /// TRUE, wenn Werte in die Master-Punkte geschrieben werden (nicht, wenn  <see cref="CopyLiveToMasterValues"/> genutzt wird)
    /// </summary>
    public bool IsSetByHand { get; set; }

    public IOrderedEnumerable<IKehre> Kehren_Live => _liveKehren.OrderBy(k => k.KehrenNummer);
    public IOrderedEnumerable<IKehre> Kehren_Master => _masterKehren.OrderBy(k => k.KehrenNummer);


    public event EventHandler SpielStandChanged;
    protected virtual void RaiseSpielstandChanged()
    {
        var handler = SpielStandChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }


    private readonly List<IKehre> _masterKehren;
    private readonly List<IKehre> _liveKehren;

    #region Constructor

    /// <summary>
    /// Default-Constructor
    /// </summary>
    private Spielstand()
    {
        _masterKehren = new List<IKehre>() { Kehre.Create(1, 0, 0) };
        _liveKehren = new List<IKehre>();
    }

    public static ISpielstand Create() => new Spielstand();

    #endregion

    #region Set-Functions

    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE <br></br>
    /// und schreibt in <see cref="Punkte_Master_TeamA"/> und <see cref="Punkte_Live_TeamA"/> den übergebenen Wert
    /// !! Alle anderen Kehren werden gelöscht !!
    /// /// </summary>
    /// <param name="punkteTeamA"></param>
    public void SetMasterTeamAValue(int punkteTeamA)
    {
        IsSetByHand = true;

        //Live-Kehren
        if (_liveKehren.Count > 1)
            _liveKehren.Clear();

        if (_liveKehren.Any())
            _liveKehren[0].PunkteTeamA = punkteTeamA;
        else
            _liveKehren.Add(Kehre.Create(1, punkteTeamA, 0));

        //Master-Kehren
        if (_masterKehren.Count > 1)
            _masterKehren.Clear();

        if (_masterKehren.Any())
            _masterKehren[0].PunkteTeamA = punkteTeamA;
        else
            _masterKehren.Add(Kehre.Create(1, punkteTeamA, 0));

        RaiseSpielstandChanged();
    }

    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE <br></br>
    /// und schreibt in die erste Kehre von <see cref="Kehren_Master"/> und <see cref="Kehren_Live"/> den übergebenen Wert.<br></br>
    /// !! Alle anderen Kehren werden gelöscht !!
    /// </summary>
    /// <param name="punkteTeamB"></param>
    public void SetMasterTeamBValue(int punkteTeamB)
    {
        IsSetByHand = true;

        if (_liveKehren.Count > 1)
            _liveKehren.Clear();

        if (_liveKehren.Any())
            _liveKehren[0].PunkteTeamB = punkteTeamB;
        else
            _liveKehren.Add(Kehre.Create(1, 0, punkteTeamB));


        if (_masterKehren.Count > 1)
            _masterKehren.Clear();

        if (_masterKehren.Any())
            _masterKehren[0].PunkteTeamB = punkteTeamB;
        else
            _masterKehren.Add(Kehre.Create(1, 0, punkteTeamB));

        RaiseSpielstandChanged();
    }

    /// <summary>
    /// Setzt <see cref="IsSetByHand"/> auf TRUE<br></br>
    /// Es wird die Kehre in  <see cref="Kehren_Live"/> und <see cref="Kehren_Master"/> gelöscht und dann neu angefügt
    /// </summary>
    /// <param name="kehre"></param>
    public void SetMasterValue(IKehre kehre)
    {
        IsSetByHand = true;

        //Kehre LIVE
        _liveKehren.RemoveAll(k => k.KehrenNummer == kehre.KehrenNummer);
        _liveKehren.Add(kehre);

        //Kehre MASTER
        _masterKehren.RemoveAll(k => k.KehrenNummer == kehre.KehrenNummer);
        _masterKehren.Add(kehre);

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
            _masterKehren.Clear();
            foreach (var kehre in _liveKehren)
            {
                _masterKehren.Add(kehre);
            }

            RaiseSpielstandChanged();
        }
    }

    /// <summary>
    /// Wenn <see cref="IsSetByHand"/> FALSE ist, werden erst alle Kehren in <see cref="Kehren_Live"/> gelöscht, <br>
    /// </br> und eine neue Kehre mit den Werten angefügt
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
            _liveKehren.Clear();
            _liveKehren.Add(Kehre.Create(1, teamA, teamB));

            RaiseSpielstandChanged();
        }
    }

    /// <summary>
    /// Wenn <see cref="IsSetByHand"/> TRUE ist, werden alle Kehren in <see cref="Kehren_Live"/> gelöscht, <br>
    /// </br>und dann alle übergegebenen Kehren angefügt
    /// </summary>
    /// <param name="kehren"></param>
    public void SetLiveValues(IOrderedEnumerable<IKehre> kehren)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Spielstand schreiben. Anzahl an Kehren:{kehren.Count()} -> kann geschrieben werden:{!IsSetByHand}");
#endif

        if (!IsSetByHand)
        {
            _liveKehren.Clear();
            foreach (var kehre in kehren)
            {
                _liveKehren.Add(kehre);
            }
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
