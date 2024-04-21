using StockApp.Comm.NetMqStockTV;

namespace StockApp.Core.Wettbewerb.Zielbewerb;

public interface ITeilnehmer
{
    /// <summary>
    /// AnzeigeName. Wenn Online, mit Bahnnummer
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Nachname
    /// </summary>
    public string LastName { get; set; }
    /// <summary>
    /// Vorname
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// Name für StockTV, mit Spielklasse wenn vorhanden
    /// </summary>
    public string NameForTV { get; }
    /// <summary>
    /// Passnummer
    /// </summary>
    public string LicenseNumber { get; set; }
    /// <summary>
    /// Startnummer
    /// </summary>
    public int Startnummer { get; set; }
    /// <summary>
    /// Vereinsname des Spielers
    /// </summary>
    public string Vereinsname { get; set; }
    /// <summary>
    /// Nation des Spieler, kann auch Region, Bundesland, Kreis oder Bezirk sein
    /// </summary>
    public string Nation { get; set; }
    /// <summary>
    /// Spielklasse des Spielers, z.B. U14m, Damen, Herren,usw.
    /// </summary>
    string Spielklasse { get; set; }
    /// <summary>
    /// Nummer der Bahn, auf der sich der Spieler zur Versuchsabgabe befindet
    /// </summary>
    public int AktuelleBahn { get; }
    /// <summary>
    /// TRUE, wenn sicher der Spieler auf einer Bahn befindet
    /// </summary>
    public bool HasOnlineWertung { get; }
    /// <summary>
    /// Die gewählte Wertung, die durch StockTV mit Daten gefüllt wird oder NULL
    /// </summary>
    public IWertung OnlineWertung { get; }
    /// <summary>
    /// Alle Wertungen des Spielers
    /// </summary>
    public IEnumerable<IWertung> Wertungen { get; }
    /// <summary>
    /// Summe aller Punkte aller Wertungen
    /// </summary>
    public int GesamtPunkte { get; }

    /// <summary>
    /// Teilnehmer auf einer Bahn onlinee schalten, Es wird <see cref="AktuelleBahn"/> und <see cref="OnlineWertung"/> aktualisiert
    /// </summary>
    /// <param name="bahnNummer"></param>
    /// <param name="wertungsNummer"></param>
    void SetOnline(int bahnNummer, int wertungsNummer);
    /// <summary>
    /// Teilnehmer Offline schalten. Es wird <see cref="AktuelleBahn"/> auf -1 und <see cref="OnlineWertung"/> auf NULL gesetzt
    /// </summary>
    void SetOffline();
    /// <summary>
    /// Dem Teilnehmer eine neue Wertung anfügen
    /// </summary>
    /// <returns></returns>
    IWertung AddNewWertung();
    /// <summary>
    /// Wertung beim Teilnehmer entfernen
    /// </summary>
    /// <param name="wertung"></param>
    void RemoveWertung(IWertung wertung);
    /// <summary>
    /// TRUE, wenn eine Wertung angefügt werden kann (max 4 Wertungen möglich)
    /// </summary>
    /// <returns></returns>
    bool CanAddWertung();
    /// <summary>
    /// TRUE, wenn eine Wertung entfernt werden kann (mind. 1 Wertung muss bleiben)
    /// </summary>
    /// <returns></returns>
    bool CanRemoveWertung();
    /// <summary>
    /// Occours after a Wertung has added or removed
    /// </summary>
    event EventHandler WertungenChanged;
    /// <summary>
    /// Occours after the State of IsOnline changed
    /// </summary>
    event EventHandler OnlineStatusChanged;
    /// <summary>
    /// Wird ausgeführ, wenn sich die Startnummer ändert
    /// </summary>
    event EventHandler StartNumberChanged;
}

public class Teilnehmer : TBasePlayer, ITeilnehmer
{
    #region Fields

    private readonly List<IWertung> _wertungen = new();

    #endregion

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event EventHandler WertungenChanged;
    protected void RaiseWertungenChanged()
    {
        var hanlder = WertungenChanged;
        hanlder?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event EventHandler OnlineStatusChanged;
    protected void RaiseOnlineStatusChanged()
    {
        var handler = OnlineStatusChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
    /// <inheritdoc/>
    /// </summary>
    public new string Name => OnlineWertung != null
                            ? $"{base.Name} ({AktuelleBahn}) "
                            : base.Name;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string NameForTV
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Spielklasse)
                ? $"{LastName} {FirstName}    ({Spielklasse})"
                : $"{LastName} {FirstName}";
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Startnummer { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Vereinsname { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Nation { get; set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Spielklasse { get; set; }

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

    internal void SetVersuch(IStockTVZielbewerb bewerb)
    {
        if (OnlineWertung == null)
            return;

        this.OnlineWertung.Reset();
        if (bewerb.MassenVorne.Versuche.Count > 6)
            Wertungen.FirstOrDefault(w => w.Nummer == OnlineWertung.Nummer + 1)?.Reset();

        foreach (var zielDisziplin in bewerb.Disziplinen())
        {
            switch (zielDisziplin.Name)
            {
                case StockTVZielDisziplinName.MassenMitte:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(Disziplinart.MassenMitte, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;
                case StockTVZielDisziplinName.Schiessen:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(Disziplinart.Schiessen, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;
                case StockTVZielDisziplinName.MassenSeite:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(Disziplinart.MassenSeite, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;
                case StockTVZielDisziplinName.Kombinieren:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(Disziplinart.Kombinieren, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;

                default:
                    break;
            }
        }

    }

    private void SetVersuch(Disziplinart disziplinart, int versuchNr, int value)
    {
        if (versuchNr <= 6)
            OnlineWertung.Disziplinen.First(d => d.Disziplinart == disziplinart).SetVersuch(versuchNr, value);
        else
        {
            var nextWertung = Wertungen.FirstOrDefault(w => w.Nummer == OnlineWertung.Nummer + 1) ?? this.AddNewWertung();
            nextWertung.Disziplinen.First(d => d.Disziplinart == disziplinart).SetVersuch(versuchNr - 6, value);
        }
    }

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
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Kombinieren).Versuch1 = value; break;
            case 20:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Kombinieren).Versuch2 = value; break;
            case 21:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Kombinieren).Versuch3 = value; break;
            case 22:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Kombinieren).Versuch4 = value; break;
            case 23:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Kombinieren).Versuch5 = value; break;
            case 24:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Kombinieren).Versuch6 = value; break;

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
