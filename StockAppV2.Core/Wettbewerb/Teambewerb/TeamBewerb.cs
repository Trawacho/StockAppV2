using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;

namespace StockApp.Core.Wettbewerb.Teambewerb;

public interface ITeamBewerb : IBewerb
{
    /// <summary>
    /// Liste aller Teams
    /// </summary>
    IEnumerable<ITeam> Teams { get; }

    /// <summary>
    /// Liste alles Spiele
    /// </summary>
    IEnumerable<IGame> Games { get; }

    /// <summary>
    /// Anzahl der Stockbahnen / Spielfächen
    /// </summary>
    int NumberOfCourts { get; }

    /// <summary>
    /// Number of rounds to play (default 1) 
    /// </summary>
    int NumberOfGameRounds { get; set; }

    /// <summary>
    /// Anzahl der Aussetzer, bei ungerader Anzahl an Mannschaften (nicht virtuell) immer 1
    /// </summary>
    int BreaksCount { get; set; }

    /// <summary>
    /// On True, the TurnCard has 8 instead of 7 Turns per Team
    /// </summary>
    bool Is8TurnsGame { get; set; }

    public event EventHandler Is8TurnsGameChanged;

    /// <summary>
    /// If true, the Game-Start is switched after every round of game
    /// </summary>
    bool StartingTeamChange { get; set; }

    /// <summary>
    /// Number of Top Teams, where the names of the players are on the printed Result
    /// </summary>
    int NumberOfTeamsWithNamedPlayerOnResult { get; set; }
    int SpielGruppe { get; set; }
    IERVersion IERVersion { get; set; }

    void AddNewTeam();

    void AddVirtualTeams(int count);

    void RemoveTeam(ITeam team);

    void RemoveAllTeams();

    void RemoveAllVirtualTeams();

    /// <summary>
    /// Occours after adding or removing a team
    /// </summary>
    public event EventHandler TeamsChanged;


    /// <summary>
    /// Creates new Games after deleting the old ones
    /// </summary>
    void CreateGames();

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
    public event EventHandler GamesChanged;

    public IEnumerable<IGame> GetAllGames(bool withBreaks = true);

    public IOrderedEnumerable<ITeam> GetTeamsRanked(bool live = false);

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
    private int _breakCount = 1;
    private int _spielGruppe;
    private bool _is8TurnsGame;
    #endregion

    #region Properties

    /// <summary>
    /// Liste aller Teams
    /// </summary>
    public IEnumerable<ITeam> Teams => _teams.AsReadOnly();

    public IEnumerable<IGame> Games => _teams.SelectMany(t => t.Games);

    /// <summary>
    /// Anzahl der Stockbahnen / Spielfächen
    /// </summary>
    public int NumberOfCourts => Teams.Count(t => !t.IsVirtual) / 2;

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
    /// Anzahl der Aussetzer, bei ungerader Anzahl an Mannschaften (nicht virtuell) immer 1
    /// </summary>
    public int BreaksCount
    {
        get => Teams.Count(t => !t.IsVirtual) % 2 == 0 ? _breakCount : 1;
        set => _breakCount = value;
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
    /// Nummer der Gruppe, wenn mehrere Gruppen gleichzeitig auf der Spielfläche sind
    /// 
    /// Default: 0
    /// </summary>
    public int SpielGruppe
    {
        get => this._spielGruppe;
        set
        {
            this._spielGruppe = value;
        }
    }

    public IERVersion IERVersion { get; set; } = IERVersion.v2022;

    #endregion

    #region Constructor

    private TeamBewerb()
    {
        _is8TurnsGame = false;
    }

    public static TeamBewerb Create() => new();

    #endregion

    public void Reset()
    {
        RemoveAllTeams();
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

        if (gamesPerCourt.Count == 0) return 0;
        return gamesPerCourt.Max(m => m.Value);
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

        return Teams.Where(t => !t.IsVirtual).OrderBy(t => t, comparer);
    }

    #endregion

    #region Team Functions

    public void AddVirtualTeams(int count)
    {
        RemoveAllVirtualTeams();
        for (int i = 0; i < count; i++)
        {
            AddTeam(
                Team.Create("Virtual Team", true));
        }
    }

    public void RemoveAllVirtualTeams() => _teams.RemoveAll(t => t.IsVirtual);

    public void RemoveAllTeams() => _teams.Clear();

    public void AddNewTeam()
    {
        RemoveAllVirtualTeams();
        AddTeam(Team.Create($"default {Teams.Count() + 1}", false));
    }

    /// <summary>
    /// Adds a Team to the Tournament.<br></br>
    /// - Deletes all virtual Teams<br></br>
    /// - All Games were deleted<br></br>
    /// - Startnumbers were reOrganized
    /// </summary>
    /// <param name="team">The <see cref="Team"/> to add</param>
    /// <param name="deleteVirtualTeamsFirst">If TRUE, all VirtualTeams are removed first</param>
    private void AddTeam(ITeam team)
    {
        if (!team.IsVirtual)
            team.GamesChanged += Team_GamesChanged;

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

        RemoveAllVirtualTeams();

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

    /// <summary>
    /// Funktion zur Erzeugung des Spielplans
    /// </summary>
    public void CreateGames()
    {
        //Entferne alle Spiele von allen Teams
        foreach (var t in _teams)
            t.ClearGames();

        int iBahnCor = 0;               //Korrektur-Wert für Bahn

        #region Virtual Teams, es wird immer eine Gerade Anzahl an Mannschaften benötigt

        //Remove Virtual Teams to check the number of VirtualTeams needed
        RemoveAllVirtualTeams();


        //Bei ungerade Zahl an Teams ein virtuelles Team hinzufügen
        if (Teams.Count() % 2 == 1)
        {
            AddVirtualTeams(1);
        }
        else
        {
            //Gerade Anzahl an Mannschaften
            //Entweder kein Aussetzer oder ZWEI Aussetzer
            if (BreaksCount == 2)
            {
                AddVirtualTeams(2);
            }
        }
        #endregion

        // Jede Spielrunde berechnen
        for (int spielRunde = 1; spielRunde <= this.NumberOfGameRounds; spielRunde++)
        {

            //Über Schleifen die Spiele erstellen, Teams, Bahnen und Anspiel festlegen
            for (int i = 1; i < Teams.Count(); i++)
            {
                var game = Game.Create(Teams.First(t => t.StartNumber == i),
                                        Teams.First(t => t.StartNumber == Teams.Count()),
                                        Teams.Count() - i,
                                        spielRunde,
                                        (spielRunde - 1) * (Teams.Count() - 1) + (Teams.Count() - i));



                #region Bahn Berechnen

                if (game.TeamA.IsVirtual || game.TeamB.IsVirtual)
                {
                    game.CourtNumber = 0;
                }
                else
                {
                    if (i <= Teams.Count() / 2)
                    {
                        game.CourtNumber = (Teams.Count() / 2) - i + 1;
                    }
                    else
                    {
                        game.CourtNumber = i - (Teams.Count() / 2) + 1;
                    }
                    iBahnCor = game.CourtNumber;
                }

                #endregion

                #region Anspiel festlegen

                //Die grundsätzliche Festlegung des Anspiels aufgrund des Schleifenverlaufs, nicht in die Funktion CheckAnspiel verlagert.
                game.IsTeamA_Starting = !(i % 2 == 0);
                CheckAnspiel(ref game, StartingTeamChange);

                #endregion

                Teams.First(t => t.StartNumber == game.TeamA.StartNumber).AddGame(game);
                Teams.First(t => t.StartNumber == game.TeamB.StartNumber).AddGame(game);

                for (int k = 1; k <= (Teams.Count() / 2 - 1); k++)
                {
                    game = Game.Create(Teams.Count() - i, spielRunde, (spielRunde - 1) * (Teams.Count() - 1) + (Teams.Count() - i));

                    #region Team A festlegen

                    if ((i + k) % (Teams.Count() - 1) == 0)
                    {
                        game.TeamA = Teams.First(t => t.StartNumber == (Teams.Count() - 1));
                    }
                    else
                    {
                        var nrTb = (i + k) % (Teams.Count() - 1);
                        if (nrTb < 0)
                        {
                            nrTb = (Teams.Count() - 1) + nrTb;
                        }
                        game.TeamA = Teams.First(t => t.StartNumber == nrTb);
                    }

                    #endregion

                    #region Team B festlegen

                    if ((i - k) % (Teams.Count() - 1) == 0)
                    {
                        game.TeamB = Teams.First(t => t.StartNumber == (Teams.Count() - 1));
                    }
                    else
                    {
                        var nrTa = (i - k) % (Teams.Count() - 1);
                        if (nrTa < 0)
                        {
                            nrTa = Teams.Count() - 1 + nrTa;
                        }
                        game.TeamB = Teams.First(t => t.StartNumber == nrTa);

                    }

                    #endregion

                    #region Bahn berechnen

                    if (game.TeamA.IsVirtual || game.TeamB.IsVirtual)
                    {
                        game.CourtNumber = 0;
                    }
                    else
                    {
                        if (iBahnCor != k)
                        {
                            game.CourtNumber = k;
                        }
                        else
                        {
                            game.CourtNumber = Teams.Count() / 2;
                        }
                    }

                    #endregion

                    #region Anspiel berechnen  

                    //Die grundsätzliche Festlegung des Anspiels aufgrund des Schleifenverlaufs, nicht in die Funktion CheckAnspiel verlagert.
                    game.IsTeamA_Starting = !(k % 2 == 0);
                    CheckAnspiel(ref game, StartingTeamChange);

                    #endregion

                    Teams.First(t => t.StartNumber == game.TeamA.StartNumber).AddGame(game);
                    Teams.First(t => t.StartNumber == game.TeamB.StartNumber).AddGame(game);
                }
            }

#if DEBUG
            foreach (var g in GetAllGames().OrderBy(a => a.RoundOfGame).ThenBy(b => b.GameNumber).ThenBy(c => c.CourtNumber))
                System.Diagnostics.Debug.WriteLine(g.ToStringExtended());

#endif
        }

    }

    /// <summary>
    /// Es wird bei Bedarf, die Position von TeamA und TeamB vertauscht, sodass auf jeder zweiten Bahn das TeamA Anspiel hat <br></br>
    /// Wenn Game ein "Pause-Game" ist, wird nichts geändert<br></br>
    /// Der zweite Parameter <param name="changeStartingTeam"></param> ändert das Anspiel bei jeder zweiten Runde, wenn der Parameter TRUE ist
    /// </summary>
    /// <param name="g"></param>
    /// <param name="changeStartingTeam"></param>
    private static void CheckAnspiel(ref IGame g, bool changeStartingTeam)
    {
        if (g.IsPauseGame()) return;      //Do nothing if it is a PauseGame

        if (g.CourtNumber % 2 != 0)
        {
            //Spiel auf Bahn 1, 3, 5, 7,...
            if (!g.IsTeamA_Starting)
            {
                var t1 = g.TeamA;
                var t2 = g.TeamB;
                g.TeamA = t2;
                g.TeamB = t1;
                g.IsTeamA_Starting = !g.IsTeamA_Starting;
            }
        }
        else
        {
            //Spiel auf Bahn 2, 4, 6, 8, ...
            if (g.IsTeamA_Starting)
            {
                var t1 = g.TeamA;
                var t2 = g.TeamB;
                g.TeamA = t2;
                g.TeamB = t1;
                g.IsTeamA_Starting = !g.IsTeamA_Starting;
            }
        }

        if (changeStartingTeam && g.RoundOfGame % 2 == 0)
        {
            g.IsTeamA_Starting = !g.IsTeamA_Starting;
        }
    }

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
            if (preGame != null) preGame.Spielstand.CopyLiveToMasterValues();

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
                if (preGame != null) preGame.Spielstand.CopyLiveToMasterValues();

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

