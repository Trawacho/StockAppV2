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

/// <summary>
/// Repräsentiert einen einzelnen Teilnehmer im Zielbewerb.
/// <br/>
/// Verwaltet die Wertungen (Durchgänge) des Teilnehmers und steuert den Online-Status
/// für die Echtzeit-Datenübertragung von StockTV.
/// <br/><br/>
/// Unterstützte Ziel-Modi:
/// <list type="bullet">
///   <item><b>Ziel (6 Versuche/Disziplin):</b> Ein Durchgang mit 24 Versuchen → eine Wertung.</item>
///   <item><b>Ziel (12 Versuche/Disziplin):</b> StockTV sendet 12 Werte pro Disziplin;
///     Versuche 1–6 gehen in Wertung N, Versuche 7–12 automatisch in Wertung N+1.</item>
///   <item><b>Ziel2:</b> Zwei Runden à 24 Versuche. StockTV löscht nach Runde 1 intern seine
///     Listen und sendet Runde 2 als frische Werte 1–24. Die Klasse erkennt diesen
///     Übergang anhand der Ziel2-Zustands-Flags und schreibt Runde 2 in die nächste Wertung.</item>
/// </list>
/// </summary>
public class Teilnehmer : TBasePlayer, ITeilnehmer
{
    #region Fields

    private readonly List<IWertung> _wertungen = new();

    // Ziel2-Zustandsverwaltung für den Runden-Übergang (Runde 1 → Runde 2).
    // Beide Flags werden bei jedem SetOnline()-Aufruf zurückgesetzt, sodass jede
    // neue Online-Session sauber beginnt – unabhängig davon, ob die Wertung bereits
    // Daten aus einer früheren Session enthält (Schutz vor False-Positive-Übergängen).
    //
    // Ablauf:
    //   SetOnline()          → _ziel2NeedsInitialReset = true,  _ziel2CanTransition = false
    //   Erster Ziel2-Aufruf  → Wertung resetten, _ziel2NeedsInitialReset = false, _ziel2CanTransition = true
    //   Runde 1 läuft        → _ziel2CanTransition = true, aber VersucheAllEntered = false
    //   Runde 2 startet      → _ziel2CanTransition = true + VersucheAllEntered = true → Übergang
    //                          danach: _ziel2CanTransition = false (kein weiterer Übergang)
    //   Runde 2 komplett     → _ziel2CanTransition = false → keine Wertung N+2 wird angelegt
    private bool _ziel2NeedsInitialReset = false;
    private bool _ziel2CanTransition = false;

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
    /// Schaltet den Teilnehmer auf einer Bahn online und aktiviert die angegebene Wertung.
    /// Setzt außerdem die Ziel2-Zustands-Flags zurück, damit die neue Session unabhängig
    /// von eventuell vorhandenen Altdaten in der Wertung korrekt beginnt.
    /// </summary>
    /// <param name="bahnNummer">Nummer der Bahn, auf der der Teilnehmer spielt.</param>
    /// <param name="wertungsNummer">Nummer der Wertung, die für diese Session aktiv ist.</param>
    public void SetOnline(int bahnNummer, int wertungsNummer)
    {
        AktuelleBahn = bahnNummer;

        // Alle Wertungen deaktivieren, dann die gewünschte aktivieren
        _wertungen.ToList().ForEach(w => w.IsOnline = false);
        _wertungen.First(w => w.Nummer == wertungsNummer).IsOnline = true;

        // Ziel2-Flags für die neue Session zurücksetzen
        _ziel2NeedsInitialReset = true;
        _ziel2CanTransition = false;

        RaiseOnlineStatusChanged();
    }

    /// <summary>
    /// Schaltet den Teilnehmer offline. <see cref="AktuelleBahn"/> wird auf 0 gesetzt
    /// und alle Wertungen werden deaktiviert.
    /// </summary>
    public void SetOffline()
    {
        AktuelleBahn = 0;
        _wertungen.ToList().ForEach(w => w.IsOnline = false);
        RaiseOnlineStatusChanged();
    }

    /// <summary>
    /// Wechselt zur nächsten noch nicht vollständig ausgefüllten Wertung,
    /// oder schaltet den Teilnehmer offline, wenn keine weitere Wertung verfügbar ist.
    /// Wird aufgerufen, wenn StockTV nach Abschluss aller Versuche ein leeres Ergebnis sendet.
    /// </summary>
    public void SetWertungOfflineOrNext()
    {
        int aktuelleWertung = OnlineWertung.Nummer;

        // Alle Wertungen deaktivieren
        _wertungen.ToList().ForEach(w => w.IsOnline = false);

        // Nächste Wertung aktivieren, falls vorhanden und noch nicht vollständig
        if (_wertungen.Any(w => w.Nummer == aktuelleWertung + 1))
        {
            var nextWertung = _wertungen.First(w => w.Nummer == aktuelleWertung + 1);
            if (!nextWertung.VersucheAllEntered())
                nextWertung.IsOnline = true;
        }

        // Keine aktive Wertung mehr → Teilnehmer offline setzen
        if (!_wertungen.Any(w => w.IsOnline))
            AktuelleBahn = -1;

        RaiseOnlineStatusChanged();
    }

    /// <summary>
    /// Liste der Wertungen
    /// </summary>
    public IEnumerable<IWertung> Wertungen => _wertungen;

    /// <summary>
    /// Überträgt die Versuchsdaten von StockTV in die aktive Wertung des Teilnehmers.
    /// <br/><br/>
    /// Das Verhalten hängt vom Spielmodus ab:
    /// <list type="bullet">
    ///   <item><b>Ziel:</b> Versuche 1–6 pro Disziplin gehen in die OnlineWertung.
    ///     Bei mehr als 6 Versuchen pro Disziplin (z.B. 12 Kehren) werden die Versuche 7+
    ///     automatisch in die nächste Wertung geschrieben.</item>
    ///   <item><b>Ziel2:</b> Erkennt den Übergang von Runde 1 zu Runde 2 anhand der
    ///     Ziel2-Zustands-Flags und wechselt dabei in die nächste Wertung.</item>
    /// </list>
    /// Die Methode ist idempotent: StockTV sendet den Gesamtstand nach jedem Versuch
    /// neu, daher wird die Wertung zuerst zurückgesetzt und dann vollständig neu befüllt.
    /// </summary>
    /// <param name="bewerb">Aktueller Zielbewerb-Stand von StockTV (alle Disziplinen).</param>
    /// <param name="modus">Spielmodus; bestimmt die Ziel2-Übergangslogik.</param>
    internal void SetVersuch(IStockTVZielbewerb bewerb, GameMode modus = GameMode.Ziel)
    {
        if (OnlineWertung == null)
            return;

        if (modus == GameMode.Ziel2)
        {
            if (_ziel2NeedsInitialReset)
            {
                // Erster Aufruf nach SetOnline: Altdaten in der Wertung löschen,
                // damit eine volle Wertung aus einer früheren Session keinen
                // vorzeitigen Runden-Übergang auslöst.
                OnlineWertung.Reset();
                _ziel2NeedsInitialReset = false;
                _ziel2CanTransition = true;
            }
            else if (_ziel2CanTransition && OnlineWertung.VersucheAllEntered() && !bewerb.HasNoAttempts())
            {
                // Runde 1 ist abgeschlossen (Wertung voll) und StockTV sendet bereits
                // Runde-2-Daten (StockTV hat seine Listen intern geleert und neu befüllt).
                // → Zur nächsten Wertung wechseln; anlegen falls noch nicht vorhanden.
                var currentNummer = OnlineWertung.Nummer;
                _wertungen.ToList().ForEach(w => w.IsOnline = false);
                var next = _wertungen.FirstOrDefault(w => w.Nummer == currentNummer + 1) ?? AddNewWertung();
                next.IsOnline = true;

                // Übergang abgeschlossen: kein weiterer automatischer Wechsel in dieser Session
                _ziel2CanTransition = false;
                RaiseOnlineStatusChanged();
            }
        }

        // Wertung zurücksetzen und vollständig neu befüllen (idempotentes Update)
        this.OnlineWertung.Reset();

        // Bei mehr als 6 Versuchen pro Disziplin (Ziel mit 12 Kehren): auch die
        // nächste Wertung zurücksetzen, da sie ebenfalls neu befüllt wird
        if (bewerb.MassenVorne.Versuche.Count > 6)
            Wertungen.FirstOrDefault(w => w.Nummer == OnlineWertung.Nummer + 1)?.Reset();

        // Versuche je Disziplin in die Wertung(en) schreiben
        foreach (var zielDisziplin in bewerb.Disziplinen())
        {
            switch (zielDisziplin.Name)
            {
                case StockTVZielDisziplinName.MassenMitte:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(StockTVZielDisziplinName.MassenMitte, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;
                case StockTVZielDisziplinName.Schiessen:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(StockTVZielDisziplinName.Schiessen, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;
                case StockTVZielDisziplinName.MassenSeite:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(StockTVZielDisziplinName.MassenSeite, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;
                case StockTVZielDisziplinName.Kombinieren:
                    for (int i = 0; i < zielDisziplin.Versuche.Count; i++)
                    {
                        SetVersuch(StockTVZielDisziplinName.Kombinieren, i + 1, zielDisziplin.Versuche[i]);
                    }
                    break;

                default:
                    break;
            }
        }

    }

    /// <summary>
    /// Schreibt einen einzelnen Versuch einer Disziplin in die richtige Wertung.
    /// Versuche 1–6 gehen in die aktive OnlineWertung, Versuche ab 7 in die nächste
    /// Wertung (wird automatisch angelegt), verschoben um 6 Positionen.
    /// </summary>
    /// <param name="disziplinart">Die Disziplin, der der Versuch zugeordnet wird.</param>
    /// <param name="versuchNr">Laufende Versuchsnummer aus StockTV (1-basiert).</param>
    /// <param name="value">Punktwert des Versuchs.</param>
    private void SetVersuch(StockTVZielDisziplinName disziplinart, int versuchNr, int value)
    {
        if (versuchNr <= 6)
            OnlineWertung.Disziplinen.First(d => d.Disziplinart == disziplinart).SetVersuch(versuchNr, value);
        else
        {
            // Versuch gehört in die Folgewertung (z.B. bei 12 Kehren pro Disziplin);
            // Wertung anlegen falls noch nicht vorhanden
            var nextWertung = Wertungen.FirstOrDefault(w => w.Nummer == OnlineWertung.Nummer + 1) ?? this.AddNewWertung();
            nextWertung.Disziplinen.First(d => d.Disziplinart == disziplinart).SetVersuch(versuchNr - 6, value);
        }
    }

    /// <summary>
    /// Schreibt einen einzelnen Versuch über die fortlaufende Gesamtnummer (1–24)
    /// in die entsprechende Disziplin der OnlineWertung.
    /// Wird vom Legacy-Broadcast-Protokoll (MessageVersion 0) verwendet.
    /// <br/>
    /// Mapping: 1–6 = MassenMitte, 7–12 = Schiessen, 13–18 = MassenSeite, 19–24 = Kombinieren.
    /// </summary>
    /// <param name="versuchNr">Gesamtnummer des Versuchs (1–24).</param>
    /// <param name="value">Punktwert des Versuchs.</param>
    internal void SetVersuch(int versuchNr, int value)
    {
        if (OnlineWertung == null)
            return;

        switch (versuchNr)
        {
            case 1:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = value; break;
            case 2:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch2 = value; break;
            case 3:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch3 = value; break;
            case 4:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch4 = value; break;
            case 5:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch5 = value; break;
            case 6:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch6 = value; break;

            case 7:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch1 = value; break;
            case 8:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch2 = value; break;
            case 9:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch3 = value; break;
            case 10:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch4 = value; break;
            case 11:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch5 = value; break;
            case 12:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch6 = value; break;

            case 13:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch1 = value; break;
            case 14:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch2 = value; break;
            case 15:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch3 = value; break;
            case 16:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch4 = value; break;
            case 17:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch5 = value; break;
            case 18:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch6 = value; break;

            case 19:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch1 = value; break;
            case 20:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch2 = value; break;
            case 21:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch3 = value; break;
            case 22:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch4 = value; break;
            case 23:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch5 = value; break;
            case 24:
                this.OnlineWertung.Disziplinen.First(d => d.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch6 = value; break;

            default:
                break;
        }
    }

    /// <summary>
    /// Gibt an, ob eine weitere Wertung angefügt werden kann (maximal 5 Wertungen pro Teilnehmer).
    /// </summary>
    public bool CanAddWertung() => Wertungen.Count() < 5;

    /// <summary>
    /// Gibt an, ob eine Wertung entfernt werden kann (mindestens 1 Wertung muss verbleiben).
    /// </summary>
    public bool CanRemoveWertung() => Wertungen.Count() > 1;

    /// <summary>
    /// Fügt eine neue leere Wertung am Ende der Wertungsliste an.
    /// Die Nummer der neuen Wertung entspricht der aktuellen Anzahl + 1.
    /// </summary>
    /// <returns>Die neu angelegte Wertung.</returns>
    public IWertung AddNewWertung()
    {
        var wertung = Wertung.Create(this._wertungen.Count + 1);

        this._wertungen.Add(wertung);
        RaiseWertungenChanged();
        return wertung;
    }

    /// <summary>
    /// Entfernt die angegebene Wertung aus der Liste und nummeriert alle
    /// verbleibenden Wertungen neu (fortlaufend ab 1).
    /// </summary>
    /// <param name="wertung">Die zu entfernende Wertung.</param>
    public void RemoveWertung(IWertung wertung)
    {
        this._wertungen.Remove(wertung);

        // Nummern nach dem Entfernen neu vergeben
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
