﻿using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;

namespace StockApp.Core.Wettbewerb.Zielbewerb;

public interface IZielBewerb : IBewerb
{
    /// <summary>
    /// Liste der Teilnehmer nach Startnummer sortiert
    /// </summary>
    IOrderedEnumerable<ITeilnehmer> Teilnehmerliste { get; }
    /// <summary>
    /// Liste aller Bahnen nach Bahnnummer sortiert
    /// </summary>
    IOrderedEnumerable<int> Bahnen { get; }
    /// <summary>
    /// Liste aller freier Bahnen nach Bahnnummer sortiert
    /// </summary>
    IOrderedEnumerable<int> FreieBahnen { get; }
    /// <summary>
    /// Dem <see cref="IZielBewerb"/> einen Teilnehmer anfügen
    /// </summary>
    /// <param name="teilnehmer"></param>
    void AddTeilnehmer(ITeilnehmer teilnehmer);
    /// <summary>
    /// Dem <see cref="IZielBewerb" einen neuen Teilnehmer anfügen
    /// </summary>
    void AddNewTeilnehmer();
    /// <summary>
    /// True, wenn ein neuer Teilnehmer angefügt werden kann
    /// </summary>
    /// <returns></returns>
    bool CanAddTeilnehmer();
    /// <summary>
    /// TRUE, wenn ein Teilnhemer entfernt werden kann
    /// </summary>
    /// <returns></returns>
    bool CanRemoveTeilnehmer();
    /// <summary>
    /// Teilnehmer entfernen
    /// </summary>
    /// <param name="teilnehmer"></param>
    void RemoveTeilnehmer(ITeilnehmer teilnehmer);
    /// <summary>
    /// Alle Teilnehmer einer Spielklasse sortiert nach Gesamtpunkte
    /// </summary>
    /// <param name="spielKlass"></param>
    /// <returns></returns>
    IEnumerable<ITeilnehmer> GetTeilnehmerRanked(string spielKlass);
    /// <summary>
    /// Alle Teilnehmer soritert nach Gesamtpunkte
    /// </summary>
    /// <returns></returns>
    IEnumerable<ITeilnehmer> GetTeilnehmerRanked();
    /// <summary>
    /// Einen Teilnehmer in der Startliste verschieben
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    void MoveTeilnehmer(int oldIndex, int newIndex);

    /// <summary>
    /// Text, der am Ende der Ergebnisliste steht
    /// </summary>
    string EndText { get; set; }
    /// <summary>
    /// Schriftgröße der Teilnehmerliste in der Ergebnisliste
    /// </summary>
    int FontSize { get; set; }
    /// <summary>
    /// Vertikaler Abstand zwischen Teilnehmer in der Ergebnisliste
    /// </summary>
    int RowSpace { get; set; }
    /// <summary>
    /// Bild, das auf der Ergebnisliste oben mittig ist (Pfad zur Datei)
    /// </summary>
    string ImageHeaderFileName { get; set; }
    /// <summary>
    /// Bild, das auf der Ergebnisliste oben rechts ist (Pfad zur Datei)
    /// </summary>
    string ImageTopRightFileName { get; set; }
    /// <summary>
    /// Bild, das auf der Ergebnisliste oben links ist (Pfad zur Datei)
    /// </summary>
    string ImageTopLeftFileName { get; set; }
    /// <summary>
    /// TRUE, Wenn <see cref="ITeilnehmer.Vereinsname"> auf der Ergebnisteliste angedruckt werden soll
    /// </summary>
    bool HasTeamname { get; set; }
    /// <summary>
    /// TRUE, wenn <see cref="ITeilnehmer.Name"/> auf der Ergebnisliste angedruckt werden soll
    /// </summary>
    bool HasNation { get; set; }

    /// <summary>
    /// Ocours after Collection of Teilnehmer has changed (add, remove oor move)
    /// </summary>
    event EventHandler TeilnehmerCollectionChanged;
}

internal class ZielBewerb : IZielBewerb
{
    public event EventHandler TeilnehmerCollectionChanged;
    protected void RaiseTeilnehmerCollectionChanged()
    {
        var handler = TeilnehmerCollectionChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }



    /// <summary>
    /// Liste der Teilnehmer
    /// </summary>
    private readonly IList<ITeilnehmer> _teilnehmerliste;

    /// <summary>
    /// fixtive Liste mit Nummern für Bahnen, die zur Auswahl stehen
    /// </summary>
    private readonly IList<int> _zielBahnen;

    private ZielBewerb()
    {
        _teilnehmerliste = new List<ITeilnehmer>();
        AddTeilnehmer(Teilnehmer.Create());

        _zielBahnen = new List<int>();

        for (int i = 0; i <= 15; i++)
        {
            _zielBahnen.Add(i);
        }
    }
    public static IZielBewerb Create() => new ZielBewerb();
    public void Reset()
    {
        _teilnehmerliste.Clear();
        AddNewTeilnehmer();
        EndText = string.Empty;
        FontSize = 12;
        RowSpace = 0;
        ImageHeaderFileName = string.Empty;
        ImageTopLeftFileName = string.Empty;
        ImageTopRightFileName = string.Empty;
        HasTeamname = false;
        HasNation = false;
    }


    /// <summary>
    /// Liste aller Teilnehmer
    /// </summary>
    public IOrderedEnumerable<ITeilnehmer> Teilnehmerliste => _teilnehmerliste.OrderBy(t => t.Startnummer);

    /// <summary>
    /// Auflistung aller Bahnen (15 Bahnen werden automatisch ergzeugt)
    /// </summary>
    public IOrderedEnumerable<int> Bahnen => _zielBahnen.OrderBy(b => b);

    /// <summary>
    /// Auflistung aller freier Bahnen
    /// </summary>
    public IOrderedEnumerable<int> FreieBahnen
    {
        get
        {
            var belegteBahnen = _teilnehmerliste.Where(t => t.AktuelleBahn > 0).Select(b => b.AktuelleBahn);
            return Bahnen.Where(b => !belegteBahnen.Contains(b)).OrderBy(x => x);
        }
    }

    public string EndText { get; set; }
    public int FontSize { get; set; } = 14;
    public int RowSpace { get; set; } = 0;
    public string ImageHeaderFileName { get; set; }
    public string ImageTopRightFileName { get; set; }
    public string ImageTopLeftFileName { get; set; }
    public bool HasTeamname { get; set; } = true;
    public bool HasNation { get; set; }



    /// <summary>
    /// True, wenn die Teilnehmerliste nicht voll ist (<= 30)
    /// </summary>
    /// <returns></returns>
    public bool CanAddTeilnehmer() => _teilnehmerliste.Count <= 30;

    /// <summary>
    /// True, solange die Anzahl der Teilnehmeer > 1 ist
    /// </summary>
    /// <returns></returns>
    public bool CanRemoveTeilnehmer() => _teilnehmerliste.Count > 1;

    /// <summary>
    /// Einen Teilnehmer anfügen
    /// </summary>
    /// <param name="teilnehmer"></param>
    public void AddTeilnehmer(ITeilnehmer teilnehmer)
    {
        if (teilnehmer.Startnummer < 1 ||
            _teilnehmerliste.Any(t => t.Startnummer == teilnehmer.Startnummer))
            teilnehmer.Startnummer = _teilnehmerliste.Count + 1;

        _teilnehmerliste.Add(teilnehmer);
        RaiseTeilnehmerCollectionChanged();
    }

    public void AddNewTeilnehmer() => AddTeilnehmer(Teilnehmer.Create());

    /// <summary>
    /// Der Teilnehmer wird aus der Liste entfernt
    /// </summary>
    /// <param name="teilnehmer"></param>
    public void RemoveTeilnehmer(ITeilnehmer teilnehmer)
    {
        _teilnehmerliste.Remove(teilnehmer);
        for (int i = 0; i < _teilnehmerliste.Count; i++)
        {
            _teilnehmerliste[i].Startnummer = i + 1;
        }
        RaiseTeilnehmerCollectionChanged();
    }
    public IEnumerable<ITeilnehmer> GetTeilnehmerRanked(string spielKlasse)
    {
        /*
         *  Rangfestsetzung
                531 Sieger im Zielwettbewerb ist derjenige Teilnehmer, der die höchste Punktezahl erreicht hat.
                Erreichen mehrere Teilnehmer eines Wettbewerbes die gleiche Punktezahl, so gilt für die Rangfestsetzung das
                höchste Ergebnis aus dem 4. Durchgang (bei mehreren gewerteten Durchgängen werden die Ergebnisse aller
                4. Durchgänge zusammengezählt). 
                Bei weiterer Punktegleichheit gilt das höhere Ergebnis aus dem 3. Durchgang und dann aus dem 2. Durchgang.
                Bei Punktegleichheit in allen 4 Durchgängen werden die Spieler auf den gleichen Rang gesetzt. 
        */

        return _teilnehmerliste.Where(t => t.Spielklasse == spielKlasse)
                                .OrderByDescending(a => a.GesamtPunkte)
                                .ThenByDescending(b => b.Wertungen.Sum(x => x.PunkteKombinieren))
                                .ThenByDescending(c => c.Wertungen.Max(x => x.PunkteMassenSeitlich))
                                .ThenByDescending(d => d.Wertungen.Max(x => x.PunkteSchuesse))
                                .ThenByDescending(e => e.Wertungen.Max(x => x.PunkteMassenMitte));

    }

    public IEnumerable<ITeilnehmer> GetTeilnehmerRanked()
    {
        /*
         *  Rangfestsetzung
                531 Sieger im Zielwettbewerb ist derjenige Teilnehmer, der die höchste Punktezahl erreicht hat.
                Erreichen mehrere Teilnehmer eines Wettbewerbes die gleiche Punktezahl, so gilt für die Rangfestsetzung das
                höchste Ergebnis aus dem 4. Durchgang (bei mehreren gewerteten Durchgängen werden die Ergebnisse aller
                4. Durchgänge zusammengezählt). 
                Bei weiterer Punktegleichheit gilt das höhere Ergebnis aus dem 3. Durchgang und dann aus dem 2. Durchgang.
                Bei Punktegleichheit in allen 4 Durchgängen werden die Spieler auf den gleichen Rang gesetzt. 
        */

        return _teilnehmerliste.OrderByDescending(a => a.GesamtPunkte)
                                .ThenByDescending(b => b.Wertungen.Sum(x => x.PunkteKombinieren))
                                .ThenByDescending(c => c.Wertungen.Max(x => x.PunkteMassenSeitlich))
                                .ThenByDescending(d => d.Wertungen.Max(x => x.PunkteSchuesse))
                                .ThenByDescending(e => e.Wertungen.Max(x => x.PunkteMassenMitte));

    }

    public void MoveTeilnehmer(int oldIndex, int newIndex)
    {
        var teilnehmer = _teilnehmerliste[oldIndex];

        if (newIndex < 0)
        {
            return;
        }

        _teilnehmerliste.RemoveAt(oldIndex);
        _teilnehmerliste.Insert(newIndex, teilnehmer);

        for (int i = 0; i < _teilnehmerliste.Count; i++)
        {
            _teilnehmerliste[i].Startnummer = i + 1;
        }

        RaiseTeilnehmerCollectionChanged();
    }

    private IBroadCastTelegram _lastTelegram;

    public void SetStockTVResult(IStockTVResult tVResult)
    {
        if (tVResult.TVSettings.MessageVersion == 0)
        {
            SetBroadcastData(tVResult.AsBroadCastTelegram());
            return;
        }

        if (tVResult.TVSettings.GameModus != GameMode.Ziel)
            return;

        try
        {
            if (this.Teilnehmerliste.FirstOrDefault(t => t.AktuelleBahn == tVResult.TVSettings.Bahn) is Teilnehmer spieler)
            {
                if (spieler.OnlineWertung.VersucheAllEntered() && tVResult.ResultZielbewerb.HasNoAttempts())
                {
                    spieler.SetWertungOfflineOrNext();
                }
                else
                {
                    spieler?.SetVersuch(tVResult.ResultZielbewerb);
                }
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("SetStockTVResult: " + ex.Message.ToString());
#endif
        }
    }

    public void SetBroadcastData(IBroadCastTelegram telegram)
    {
        if (telegram.StockTVModus != 100) return;
        if (telegram.MessageVersion != 0) return;

        if (telegram.Equals(_lastTelegram))
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"..same telegram..");
#endif
            return;
        }
        _lastTelegram = telegram.Copy();

        /*
         * 03 01 00 00 00 00 00 00 00 00 04 08 00 06 10 02 05 10 02 00 10 
         * 
         * Aufbau: 
         * Im den ersten zehn Bytes (Header) stehen Settings von StocktV wie Bahnnummer (Position1) usw
         * In jedem weiteren Byte kommen die laufenden Versuche (max 24) 
         * Somit ist ein Datagramm max 10+24 Bytes lang
         * 
         */

        try
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Bahnnummer:{telegram.BahnNummer} -- {string.Join("-", telegram.Values)}");
#endif

            if (this.Teilnehmerliste.FirstOrDefault(t => t.AktuelleBahn == telegram.BahnNummer) is Teilnehmer spieler)
            {
                if (spieler.OnlineWertung.VersucheAllEntered() && telegram.Values.Length == 0)  //Alle Versuche auf der entsprechenden Bahn wurden eingegeben und von StockTV kommen keine Values, nur der Header
                {
                    spieler.SetWertungOfflineOrNext();
                }
                else
                {
                    spieler.OnlineWertung.Reset();

                    for (int i = 0; i < telegram.Values.Length; i++)
                    {
                        spieler?.SetVersuch(i + 1, telegram.Values[i]);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SetBroadcastData: {ex.Message}");
        }
        finally
        {
            // RaisePropertyChanged("");
        }

    }

}
