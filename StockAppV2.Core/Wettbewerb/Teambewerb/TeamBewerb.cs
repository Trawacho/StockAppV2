﻿using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;

namespace StockApp.Core.Wettbewerb.Teambewerb;

public interface ITeamBewerb : IBewerb
{
    int ID { get; }

    /// <summary>
    /// Liste aller Teams
    /// </summary>
    IEnumerable<ITeam> Teams { get; }

    /// <summary>
    /// Liste alles Spiele
    /// </summary>
    IEnumerable<IGame> Games { get; }

    /// <summary>
    /// ID des Spielplans
    /// </summary>
    int GameplanId { get; set; }

    /// <summary>
    /// Number of rounds to play (default 1) 
    /// </summary>
    int NumberOfGameRounds { get; set; }

    /// <summary>
    /// On True, the TurnCard has 8 instead of 7 Turns per Team
    /// </summary>
    bool Is8TurnsGame { get; set; }

    /// <summary>
    /// Occours after <see cref="Is8TurnsGame"/> value changed
    /// </summary>
    public event EventHandler Is8TurnsGameChanged;

    /// <summary>
    /// If true, the Game-Start is switched after every round of game
    /// </summary>
    bool StartingTeamChange { get; set; }

    /// <summary>
    /// Number of Top Teams, where the names of the players are on the printed Result
    /// </summary>
    int NumberOfTeamsWithNamedPlayerOnResult { get; set; }

    /// <summary>
    /// Text, der unterhalb der Tabelle bei einer Ergebnisliste angezeigt wird
    /// </summary>
    public string Endtext { get; set; }

    /// <summary>
    /// Text, der oberhalb der Tabelle bei einer Ergebnisliste angezeigt wird
    /// </summary>
    public string VorText { get; set; }

    /// <summary>
    /// Anzahl der Mannschaften die als Aufsteiger in der Ergebnisliste gekennzeichnet werden
    /// </summary>
    int AnzahlAufsteiger { get; set; }

    /// <summary>
    /// Anzahl der Mannschaften die als Absteiger in der Ergebnisliste gekennzeichnet werden
    /// </summary>
    int AnzahlAbsteiger { get; set; }

    /// <summary>
    /// Beim Mannschaftsnamen wird die Startnummer mit angegeben
    /// </summary>
    bool TeamNameWithStartnumber { get; set; }

	/// <summary>
	/// Welche Info zu dem Verein soll angezeigt werden, Kreis, Region, usw...
	/// </summary>
	public TeamInfo TeamInfo { get; set; }

	/// <summary>
	/// Image Links Oben - Dateiname
	/// </summary>
	string ImageTopLeftFilename { get; set; }

    /// <summary>
    /// Image Rechts Oben - Dateiname
    /// </summary>
    string ImageTopRightFilename { get; set; }

    /// <summary>
    /// Image oben in der Mitte - Dateiname
    /// </summary>
    string ImageHeaderFilename { get; set; }

    /// <summary>
    /// Zeilenabstand in der Ergebnisliste
    /// </summary>
    int RowSpace { get; set; }

    /// <summary>
    /// FontSize für die Ergebnistabelle
    /// </summary>
    int FontSize { get; set; }

    /// <summary>
    /// FontSize für den EndText in der Ergebnisliste
    /// </summary>
    int FontSizeEndText { get; set; }

    /// <summary>
    /// FontSize für den VorText in der Ergebnisliste
    /// </summary>
    int FontSizeVorText { get; set; }

    /// <summary>
    /// Bei Splitgruppen nach der ersten Tabelle einen Seitenumbruch einfügen
    /// </summary>
    bool PageBreakSplitGroup { get; set; }

    /// <summary>
    /// Nummer der Spielgruppe
    /// </summary>
    int SpielGruppe { get; set; }

    /// <summary>
    /// Kennzeichen einer Splitgruppe
    /// </summary>
    bool IsSplitGruppe { get; set; }

    /// <summary>
    /// Name der Gruppe
    /// </summary>
    string Gruppenname { get; set; }

    /// <summary>
    /// IER Version für das Ranking
    /// </summary>
    IERVersion IERVersion { get; set; }

    /// <summary>
    /// Add a new Team
    /// </summary>
    void AddNewTeam();

    /// <summary>
    /// Remove the team
    /// </summary>
    /// <param name="team"></param>
    void RemoveTeam(ITeam team);

    /// <summary>
    /// Liste der Teams wird geleert
    /// </summary>
    void RemoveAllTeams();

    /// <summary>
    /// Occours after adding or removing a team
    /// </summary>
    public event EventHandler TeamsChanged;

    /// <summary>
    /// Count of all Games from all Teams without breaks
    /// </summary>
    /// <returns>Count of all Games without breaks</returns>
    int GetCountOfGames();

    /// <summary>
    /// Count of Games per Court without breaks
    /// </summary>
    /// <returns></returns>
    int GetCountOfGamesPerCourt();

    /// <summary>
    /// Occours when a game is added or removed to a team
    /// </summary>
    event EventHandler GamesChanged;

    IEnumerable<IGame> GetAllGames(bool withBreaks = true);

    IOrderedEnumerable<ITeam> GetTeamsRanked(bool live = false);

    IOrderedEnumerable<ITeam> GetSplitTeamsRanked(bool groupOne, bool live = false);

    bool IsEachGameDone(bool live);
}


public class TeamBewerb : ITeamBewerb
{
    #region TeamsChanged Event
    public event EventHandler TeamsChanged;
    protected virtual void RaiseTeamsChanged()
    {
        var handler = TeamsChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region GamesChanged Event
    public event EventHandler GamesChanged;

    protected virtual void RaiseGamesChanged()
    {
        var handler = GamesChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }
    private void Team_GamesChanged(object sender, EventArgs e) => RaiseGamesChanged();

    #endregion

    #region Is8TurnsGameChanged

    public event EventHandler Is8TurnsGameChanged;
    protected virtual void RaiseIs8TurnsGameChanged() => Is8TurnsGameChanged?.Invoke(this, EventArgs.Empty);

    #endregion

    #region Fields

    private readonly List<ITeam> _teams = new();
    private int _numberOfGameRounds = 1;
    private bool _is8TurnsGame;
    private string _endText;
    private string _vorText;

    #endregion

    #region Properties

    public int ID { get; init; }

    /// <summary>
    /// Liste aller Teams
    /// </summary>
    public IEnumerable<ITeam> Teams => _teams.AsReadOnly();

    /// <summary>
    /// Liste aller Spiele
    /// </summary>
    public IEnumerable<IGame> Games => _teams.SelectMany(t => t.Games);

    /// <summary>
    /// ID des Spielplans
    /// </summary>
    public int GameplanId { get; set; }

    /// <summary>
    /// Number of rounds to play (default 1) 
    /// </summary>
    public int NumberOfGameRounds
    {
        get
        {
            return _numberOfGameRounds;
        }
        set
        {
            if (_numberOfGameRounds == value) return;
            if (value < 1 || value > 7) return;

            _numberOfGameRounds = value;
        }
    }

    /// <summary>
    /// On True, the TurnCard has 8 instead of 7 Turns per Team
    /// </summary>
    public bool Is8TurnsGame { get => _is8TurnsGame; set { _is8TurnsGame = value; RaiseIs8TurnsGameChanged(); } }

    /// <summary>
    /// True, wenn bei einer Mehrfachrunde das Anspiel bei jeder Runde gewechselt wird
    /// </summary>
    public bool StartingTeamChange { get; set; } = false;

    /// <summary>
    /// Bei wieviel Mannschaften werden die Spielernamen auf der Ergebnisliste mit angedruckt
    /// </summary>
    public int NumberOfTeamsWithNamedPlayerOnResult { get; set; } = 3;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int AnzahlAufsteiger { get; set; } = 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int AnzahlAbsteiger { get; set; } = 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Endtext { get => _endText; set => _endText = value?.Trim(); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string VorText { get => _vorText; set => _vorText = value?.Trim(); }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool TeamNameWithStartnumber { get; set; } = false;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public TeamInfo TeamInfo { get; set; } = TeamInfo.Keine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ImageTopLeftFilename { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ImageTopRightFilename { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ImageHeaderFilename { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int RowSpace { get; set; } = 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int FontSize { get; set; } = 14;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int FontSizeVorText { get; set; } = 12;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int FontSizeEndText { get; set; } = 12;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool PageBreakSplitGroup { get; set; }

    /// <summary>
    /// Nummer der Gruppe, wenn mehrere Gruppen gleichzeitig auf der Spielfläche sind
    /// 
    /// Default: 0
    /// </summary>
    public int SpielGruppe { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsSplitGruppe { get; set; }

    /// <summary>
    /// Name der Gruppe
    /// </summary>
    public string Gruppenname { get; set; }

    /// <summary>
    /// IER VErsion für das Ranking
    /// </summary>
    public IERVersion IERVersion { get; set; } = IERVersion.v2022;

    #endregion

    #region Constructor

    private TeamBewerb(int id)
    {
        ID = id;
        _is8TurnsGame = false;
        Gruppenname = $"Gruppe {id}";
    }

    public static TeamBewerb Create(int id) => new(id);

    #endregion

    /// <summary>
    /// Es werden alle Teams entfernt
    /// </summary>
    public void Reset()
    {
        RemoveAllTeams();
        Endtext = string.Empty;
        FontSize = 14;
        FontSizeVorText = 12;
        FontSizeEndText = 12;
        RowSpace = 0;
        ImageHeaderFilename = string.Empty;
        ImageTopLeftFilename = string.Empty;
        ImageTopRightFilename = string.Empty;
        AnzahlAufsteiger = 0;
        AnzahlAbsteiger = 0;
        NumberOfTeamsWithNamedPlayerOnResult = 0;
        PageBreakSplitGroup = false;
        TeamNameWithStartnumber = false;
    }

    #region Functions

    /// <summary>
    /// Alle Spiele aller Mannschaften
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IGame> GetAllGames(bool withBreaks = true)
    {
        var games = Teams.SelectMany(g => g.Games)
            .Distinct()
            .OrderBy(a => a.RoundOfGame)
            .ThenBy(b => b.GameNumber);

        return withBreaks
            ? games
            : games.Where(g => !g.IsPauseGame());
    }

    /// <summary>
    /// Anzahl aller Spiele aller Mannschaften ohne Aussetzer
    /// </summary>
    /// <returns></returns>
    public int GetCountOfGames() => GetAllGames().Where(g => !g.IsPauseGame()).Count();

    /// <summary>
    /// max Count of Games per Court
    /// </summary>
    /// <returns></returns>
    public int GetCountOfGamesPerCourt()
    {
        var gamesPerCourt = GetAllGames()
            .Where(g => !g.IsPauseGame())
            .GroupBy(g => g.CourtNumber)
            .ToDictionary(g => g.Key, g => g.Count());

        return (gamesPerCourt.Count == 0)
            ? 0
            : gamesPerCourt.Max(m => m.Value);
    }


    /// <summary>
    /// Alle Spiele auf der Bahn x
    /// </summary>
    /// <param name="courtNumber"></param>
    /// <returns></returns>
    public IEnumerable<IGame> GetGamesOfCourt(int courtNumber)
    {
        return Teams.SelectMany(g => g.Games)
            .Distinct()
            .Where(c => c.CourtNumber == courtNumber)
            .OrderBy(r => r.RoundOfGame)
            .ThenBy(s => s.GameNumber);
    }

    /// <summary>
    /// Platzierungs Liste der Mannschaften 
    /// </summary>
    /// <param name="live"></param>
    /// <returns></returns>
    public IOrderedEnumerable<ITeam> GetTeamsRanked(bool live = false)
    {
        var comparer = new TeamRankingComparer(live, IERVersion);

        return Teams.OrderBy(t => t, comparer);
    }

    public IOrderedEnumerable<ITeam> GetSplitTeamsRanked(bool groupOne, bool live = false)
    {
        var comparer = new TeamRankingComparer(live, IERVersion);
        return Teams.Where(t =>
            groupOne
                ? t.StartNumber <= Teams.Count() / 2
                : t.StartNumber > Teams.Count() / 2
                ).OrderBy(t => t, comparer);
    }


    public bool IsEachGameDone(bool live = false) => Teams.Where(t => t.TeamStatus == TeamStatus.Normal).All(t => t.IsEachGameDone(live));


    #endregion

    #region Team Functions

    /// <summary>
    /// Liste der Teams wird geleert
    /// </summary>
    public void RemoveAllTeams() => _teams.Clear();

    /// <summary>
    /// Es wird ein neuen Team angefügt
    /// </summary>
    public void AddNewTeam()
    {
        AddTeam(Team.Create($"default {Teams.Count() + 1}"));
    }

    /// <summary>
    /// Adds a Team to the Tournament.<br></br>
    /// - Set <see cref="GameplanId"/> to zero
    /// - All Games were deleted<br></br>
    /// - Startnumbers were reOrganized
    /// </summary>
    /// <param name="team">The <see cref="Team"/> to add</param>
    /// <param name="deleteVirtualTeamsFirst">If TRUE, all VirtualTeams are removed first</param>
    private void AddTeam(ITeam team)
    {
        team.GamesChanged += Team_GamesChanged;

        GameplanId = 0;

        foreach (var t in _teams)
            t.ClearGames();

        _teams.Add(team);

        if (team.StartNumber == 0)
            ReOrganizeTeamStartNumbers();

        RaiseTeamsChanged();
    }


    /// <summary>
    /// Removes the Team from the Tournament
    /// - Remove alle Virtual Teams
    /// - Startnumbers were reOrganized
    /// - all Games were deleted
    /// </summary>
    /// <param name="team"></param>
    public void RemoveTeam(ITeam team)
    {
        team.GamesChanged -= Team_GamesChanged;
        _teams.Remove(team);

        GameplanId = 0;

        foreach (var t in _teams)
            t.ClearGames();

        ReOrganizeTeamStartNumbers();
        RaiseTeamsChanged();
    }

    /// <summary>
    /// ReOrganize Startnumbers of each Team in List as index-based
    /// </summary>
    private void ReOrganizeTeamStartNumbers()
    {
        for (int i = 0; i < _teams.Count; i++)
        {
            _teams[i].StartNumber = i + 1;
        }
    }

    #endregion

    public void SetStockTVResult(IStockTVResult tVResult)
    {
        if (tVResult.TVSettings.MessageVersion == 0)
        {
            SetBroadcastData(tVResult.AsBroadCastTelegram());
            return;
        }

        if (tVResult.TVSettings.GameModus == GameMode.Ziel) return;
        if (tVResult.TVSettings.Spielgruppe != SpielGruppe) return;

        bool spielrichtungRechtsNachLinks = tVResult.TVSettings.NextBahnModus == NextCourtMode.Left;

        var courtGames = GetGamesOfCourt(tVResult.TVSettings.Bahn);       //Alle Spiele im Turnier auf dieser Bahn

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Es wird für {tVResult.Results.Count} Spiel(e) das Ergebnis übergeben.");
#endif

        foreach (var gameResult in tVResult.Results)
        {
            //Prüfen ob das vorherige Spiel abgeschlossen ist und in den Master kopiert werden kann
            var preGame = courtGames.FirstOrDefault(g => g.GameNumberOverAll == gameResult.GameNumber - 1);
            preGame?.Spielstand.CopyLiveToMasterValues();

            var game = courtGames.FirstOrDefault(g => g.GameNumberOverAll == gameResult.GameNumber);
            if (game == null) continue; //Wenn kein Spiel gefunden wird, sofort zur nächsten iteration gehen

            game.Spielstand.Reset();
            var kehren = gameResult.Turns.Select(t => Kehre.Convert(t, spielrichtungRechtsNachLinks)).OrderBy(k => k.KehrenNummer);
            game.Spielstand.SetLiveValues(kehren);
        }
    }


    private IBroadCastTelegram _lastTelegram;
    public void SetBroadcastData(IBroadCastTelegram telegram)
    {
        /* 
         * 03 00 15 09 21 07 09 15
         *
         * Aufbau eines Datagramms: 
         * Im ersten Byte steht die Bahnnummer ( 03 )
         * Im zweiten Byte kann die Spielgruppe stehen ( 00 )
         * In jedem weiteren Byte kommen die laufenden Spiele, erst der Wert der linken Mannschaft,
         * dann der Wert der rechten Mannschaft
         * 
         */
        if (telegram.MessageVersion != 0) return;

        if (telegram.StockTVModus == 100) return;
        if (telegram.SpielGruppe != SpielGruppe) return;
        if (telegram.Equals(_lastTelegram))
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"..same telegram..");
#endif
            return;
        }
        bool SpielrichtungRechtsNachLinks = telegram.Spielrichtung == 0;
        _lastTelegram = telegram.Copy();


        try
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Bahnnummer:{telegram.BahnNummer} -- {string.Join("-", telegram.Values)}");
#endif
            byte bahnNumber = telegram.BahnNummer;          // Im ersten Byte immer die Bahnnummer
            var courtGames = GetGamesOfCourt(bahnNumber);   //Alle Spiele im Turnier auf dieser Bahn


            int spielZähler = 0;

            //Jedes verfügbare Spiel im Datagramm durchgehen, i+2, da jedes Spiel 2 Bytes braucht. Im ersten Byte der Wert für Links, das zweite Byte für den Wert rechts
            for (int i = 0; i < telegram.Values.Length; i += 2)
            {
                spielZähler++;

                //Prüfen ob das vorherige Spiel abgeschlossen ist und in den Master kopiert werden kann
                var preGame = courtGames.FirstOrDefault(g => g.GameNumberOverAll == spielZähler - 1);
                preGame?.Spielstand.CopyLiveToMasterValues();

                var game = courtGames.FirstOrDefault(g => g.GameNumberOverAll == spielZähler);
                if (game == null) continue; //Wenn kein Spiel gefunden wird, sofort zur nächsten iteration gehen

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Spielstand schreiben für Game#OA:{game.GameNumberOverAll} -({spielZähler})- {string.Join("-", telegram.Values[i], telegram.Values[i + 1])}");
#endif

                game.Spielstand.Reset();

                if (SpielrichtungRechtsNachLinks)
                {
                    if (game.TeamA.SpieleAufStartSeite().Contains(game.GameNumberOverAll))
                    {
                        // TeamA befindet sich bei diesem Spiel auf dieser Bahn rechts, 
                        // das nächste Spiel ist auf einer Bahn mit höherer oder gleicher Bahnnummer (1-> 2-> 3-> 4->...)
                        game.Spielstand.SetLiveValues(telegram.Values[i + 1], telegram.Values[i]);
                    }
                    else
                    {
                        // TeamA befindet sich bei diesem Spiel auf der Bahn links, das nächste Spiel ist auf einer Bahn mit niedrigerer Bahnnummer (5->4->3->2->1)
                        game.Spielstand.SetLiveValues(telegram.Values[i], telegram.Values[i + 1]);
                    }
                }
                else
                {
                    if (game.TeamA.SpieleAufStartSeite().Contains(game.GameNumberOverAll))
                    {
                        // TeamA befindet sich in diesem Spiel auf dieser Bahn links, das nächste Spiel ist auf einer Bahn mit einer höheren Bahnnummer
                        game.Spielstand.SetLiveValues(telegram.Values[i], telegram.Values[i + 1]);
                    }
                    else
                    {
                        // TeamA befindet sich in diesem Spiel auf dieser Bahn rechts, das nächste Spiel ist auf einer Bahn mit einer niedrigeren Bahnnummer
                        game.Spielstand.SetLiveValues(telegram.Values[i + 1], telegram.Values[i]);

                    }
                }


            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

}

