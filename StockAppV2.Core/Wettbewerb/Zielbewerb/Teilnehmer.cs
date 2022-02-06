namespace StockApp.Core.Wettbewerb.Zielbewerb;

public interface ITeilnehmer
{
    public string Name { get; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string LicenseNumber { get; set; }
    public int Startnummer { get; set; }
    public string Vereinsname { get; set; }
    public string Nation { get; set; }
    public int AktuelleBahn { get; }
    public bool HasOnlineWertung { get; }
    public IWertung OnlineWertung { get; }
    public IEnumerable<IWertung> Wertungen { get; }

    public int GesamtPunkte { get; }

    void SetOnline(int bahnNummer, int wertungsNummer);
    void SetOffline();
    IWertung AddNewWertung();
    void RemoveWertung(IWertung wertung);
    bool CanAddWertung();
    bool CanRemoveWertung();
    /// <summary>
    /// Occours after a Wertung has added or removed
    /// </summary>
    event EventHandler WertungenChanged;
    event EventHandler OnlineStatusChanged;
    event EventHandler StartNumberChanged;
}

public class Teilnehmer : TBasePlayer, ITeilnehmer
{
    #region Fields

    private readonly List<IWertung> _wertungen = new();

    #endregion


    public event EventHandler WertungenChanged;
    protected void RaiseWertungenChanged()
    {
        var hanlder = WertungenChanged;
        hanlder?.Invoke(this, EventArgs.Empty);
    }


    public event EventHandler OnlineStatusChanged;
    protected void RaiseOnlineStatusChanged()
    {
        var handler = OnlineStatusChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }


    public event EventHandler StartNumberChanged;
    protected void RaiseStartNumberChanged()
    {
        var handler = StartNumberChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }


    private Teilnehmer()
    {
        Startnummer = default;
        AddNewWertung();
    }

    public static ITeilnehmer Create() => new Teilnehmer();

    /// <summary>
    /// AnzeigeName. Wenn Online, mit Bahnnummer
    /// </summary>
    public new string Name => OnlineWertung != null
                            ? $"{base.Name} ({AktuelleBahn}) "
                            : base.Name;

    public int Startnummer { get; set; }

    public string Vereinsname { get; set; }

    public string Nation { get; set; }

    /// <summary>
    /// Wert > 0 wenn Teilnehmer aktuell auf einer Bahn spielt.
    /// </summary>
    public int AktuelleBahn { get; private set; }

    /// <summary>
    /// Die Wertung aus <see cref="Wertungen"/>, die <see cref="Wertung.IsOnline"/> == true ist
    /// </summary>
    public IWertung OnlineWertung => _wertungen?.FirstOrDefault(w => w.IsOnline) ?? null;

    /// <summary>
    /// True, wenn eine Wertung online ist
    /// </summary>
    public bool HasOnlineWertung => _wertungen?.Any(w => w.IsOnline) ?? false;

    /// <summary>
    /// Setzt den Wert für <see cref="AktuelleBahn"/>. 
    /// <br>Die <see cref="Wertung"/> wird <see cref="Wertung.IsOnline"/> = true gesetzt.</br>
    /// <br>Die restlichen auf FALSE</br>
    /// </summary>
    /// <param name="bahnNummer"></param>
    public void SetOnline(int bahnNummer, int wertungsNummer)
    {
        AktuelleBahn = bahnNummer;
        _wertungen.ToList().ForEach(w => w.IsOnline = false);
        _wertungen.First(w => w.Nummer == wertungsNummer).IsOnline = true;
        RaiseOnlineStatusChanged();
    }

    /// <summary>
    /// <see cref="AktuelleBahn"/> wird auf -1 gesetzt. Alle <see cref="Wertungen"/> werden <see cref="Wertung.IsOnline"/> auf FALSE gesetzt
    /// </summary>
    public void SetOffline()
    {
        AktuelleBahn = -1;
        _wertungen.ToList().ForEach(w => w.IsOnline = false);
        RaiseOnlineStatusChanged();
    }

    /// <summary>
    /// Wenn möglich, wird die nächste Wertung online geschaltet, oder Spieler geht offline
    /// </summary>
    public void SetWertungOfflineOrNext()
    {
        int aktuelleWertung = OnlineWertung.Nummer;

        _wertungen.ToList().ForEach(w => w.IsOnline = false);
        if (_wertungen.Any(w => w.Nummer == aktuelleWertung + 1))
        {
            var nextWertung = _wertungen.First(w => w.Nummer == aktuelleWertung + 1);
            if (!nextWertung.VersucheAllEntered())
                nextWertung.IsOnline = true;
        }

        if (!_wertungen.Any(w => w.IsOnline))
            AktuelleBahn = -1;

        RaiseOnlineStatusChanged();
    }

    /// <summary>
    /// Liste der Wertungen
    /// </summary>
    public IEnumerable<IWertung> Wertungen => _wertungen;

    /// <summary>
    /// Es wird der Versuch in der OnlineWertung eingetragen
    /// </summary>
    /// <param name="versuchNr"></param>
    /// <param name="value"></param>
    internal void SetVersuch(int versuchNr, int value)
    {
        if (OnlineWertung == null)
            return;

        switch (versuchNr)
        {
            case 1:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch1 = value; break;
            case 2:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch2 = value; break;
            case 3:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch3 = value; break;
            case 4:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch4 = value; break;
            case 5:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch5 = value; break;
            case 6:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch6 = value; break;

            case 7:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch1 = value; break;
            case 8:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch2 = value; break;
            case 9:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch3 = value; break;
            case 10:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch4 = value; break;
            case 11:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch5 = value; break;
            case 12:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch6 = value; break;

            case 13:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Versuch1 = value; break;
            case 14:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Versuch2 = value; break;
            case 15:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Versuch3 = value; break;
            case 16:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Versuch4 = value; break;
            case 17:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Versuch5 = value; break;
            case 18:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Versuch6 = value; break;

            case 19:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch1 = value; break;
            case 20:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch2 = value; break;
            case 21:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch3 = value; break;
            case 22:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch4 = value; break;
            case 23:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch5 = value; break;
            case 24:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch6 = value; break;

            default:
                break;
        }
    }

    /// <summary>
    /// True, wenn eine neue Wertung angefügt werden kann
    /// </summary>
    public bool CanAddWertung() => Wertungen.Count() < 5;

    /// <summary>
    /// True, wenn eine Wertung entfernt werden kann
    /// </summary>
    public bool CanRemoveWertung() => Wertungen.Count() > 1;

    /// <summary>
    /// Eine zusätzliche Wertung am Ende der Liste anfügen
    /// </summary>
    /// <param name="wertung"></param>
    public IWertung AddNewWertung()
    {
        var wertung = Wertung.Create(this._wertungen.Count + 1);

        this._wertungen.Add(wertung);
        RaiseWertungenChanged();
        return wertung;
    }

    /// <summary>
    /// Die Wertung aus der Liste entfernen und neu nummerieren
    /// </summary>
    /// <param name="wertung"></param>
    public void RemoveWertung(IWertung wertung)
    {
        this._wertungen.Remove(wertung);
        for (int i = 0; i < _wertungen.Count; i++)
        {
            _wertungen[i].Nummer = i + 1;
        }
        RaiseWertungenChanged();
    }

    /// <summary>
    /// Summe aller GesamtPunkte der einzelnen Wertungen
    /// </summary>
    public int GesamtPunkte => Wertungen.Sum(w => w.GesamtPunkte);
}
