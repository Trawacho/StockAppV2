using NUnit.Framework;
using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Test.Core.Wettbewerb.Teambewerb;

public class TeamBewerbTest
{
    private TeamBewerb _teamBewerb;

    [SetUp]
    public void Setup()
    {
        _teamBewerb = TeamBewerb.Create(1);

    }

    [Test]
    public void TestPublicFunctions()
    {
        Assert.That(_teamBewerb.GetCountOfGames() == 0, Is.True);
        Assert.That(_teamBewerb.GetAllGames().Any(), Is.False);
        Assert.That(_teamBewerb.GetCountOfGamesPerCourt() == 0, Is.True);
        Assert.That(_teamBewerb.GetGamesOfCourt(1).Any(), Is.False);
        Assert.That(_teamBewerb.GetTeamsRanked(false).Any(),Is.False);

        int t = 7;
        int f = t / 2;

        for (int i = 0; i < t; i++)
        {
            _teamBewerb.AddNewTeam();
        }

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans()
                        .First(p => p.Teams == 7), _teamBewerb.Teams);


        Assert.That(_teamBewerb.GetTeamsRanked(false).Count() == t, Is.True);
        Assert.That(_teamBewerb.GetCountOfGames() == t * f, Is.True);
        Assert.That(_teamBewerb.GetAllGames(false).Count() == t * f, Is.True);
        Assert.That(_teamBewerb.GetCountOfGamesPerCourt() >= t, Is.True);
        Assert.That(_teamBewerb.GetGamesOfCourt(1).Count() == t, Is.True);



    }

    [Test]
    public void TestGetHighestPlayedRound()
    {
        for (int t = 0; t < 5; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 5), _teamBewerb.Teams, rounds: 3);

        Assert.That(_teamBewerb.GetHighestPlayedRound(), Is.EqualTo(0));

        var round1Game = _teamBewerb.GetAllGames(false).First(g => g.RoundOfGame == 1);
        round1Game.Spielstand.SetMasterTeamAValue(15);
        round1Game.Spielstand.SetMasterTeamBValue(5);

        Assert.That(_teamBewerb.GetHighestPlayedRound(), Is.EqualTo(1));

        // Live-Ergebnis in Runde 2 muss ebenfalls erkannt werden (nicht nur Master)
        var round2Game = _teamBewerb.GetAllGames(false).First(g => g.RoundOfGame == 2);
        round2Game.Spielstand.SetLiveValues(10, 3);

        Assert.That(_teamBewerb.GetHighestPlayedRound(), Is.EqualTo(2));
    }

    [Test]
    public void TestNumberOfGameRoundsRejectsReducingBelowPlayedRound()
    {
        for (int t = 0; t < 5; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 5), _teamBewerb.Teams, rounds: 3);

        _teamBewerb.NumberOfGameRounds = 3;

        var round3Game = _teamBewerb.GetAllGames(false).First(g => g.RoundOfGame == 3);
        round3Game.Spielstand.SetMasterTeamAValue(15);
        round3Game.Spielstand.SetMasterTeamBValue(2);

        _teamBewerb.NumberOfGameRounds = 2; // muss abgelehnt werden, Runde 3 hat bereits ein Ergebnis

        Assert.That(_teamBewerb.NumberOfGameRounds, Is.EqualTo(3));

        _teamBewerb.NumberOfGameRounds = 4; // weiterhin >= höchste gespielte Runde -> erlaubt
        Assert.That(_teamBewerb.NumberOfGameRounds, Is.EqualTo(4));
    }

    /// <summary>
    /// Bei nur einem Spieldurchgang (Standardfall, <see cref="ITeamBewerb.NumberOfGameRounds"/> = 1) haben ALLE Spiele
    /// dieselbe <see cref="IGame.RoundOfGame"/>. Die "aktuelle Runde" (parallel gespielte Spiele auf den Bahnen)
    /// muss deshalb über <see cref="IGame.GameNumberOverAll"/> bestimmt werden, nicht über RoundOfGame - sonst würde
    /// jede Mannschaft, die irgendwann im Turnier einmal aussetzt, dauerhaft als "pausierend" markiert.
    /// </summary>
    [Test]
    public void TestGetCurrentGameNumberOverAll_AdvancesPerTimeslot_NotPerRoundOfGame()
    {
        for (int t = 0; t < 5; t++)
            _teamBewerb.AddNewTeam();

        // 5 Teams, 2 Bahnen -> 5 Zeitslots (GameNumberOverAll 1..5), pro Slot setzt genau 1 Team aus
        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 5), _teamBewerb.Teams);

        var timeslots = _teamBewerb.GetAllGames(false).Select(g => g.GameNumberOverAll).Distinct().OrderBy(n => n).ToList();
        Assert.That(timeslots.Count, Is.GreaterThan(1), "Testvoraussetzung: mehrere Zeitslots in einer einzigen RoundOfGame");
        Assert.That(_teamBewerb.GetAllGames(false).Select(g => g.RoundOfGame).Distinct().Count(), Is.EqualTo(1));

        foreach (var slot in timeslots)
        {
            Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(false), Is.EqualTo(slot));

            // Pro Zeitslot pausiert genau eine Mannschaft, alle anderen spielen
            var pausingTeams = _teamBewerb.Teams.Where(team =>
                team.Games.Any(g => g.GameNumberOverAll == slot && g.IsPauseGame())).ToList();
            Assert.That(pausingTeams.Count, Is.EqualTo(1));

            foreach (var game in _teamBewerb.GetAllGames(false).Where(g => g.GameNumberOverAll == slot))
            {
                game.Spielstand.SetMasterTeamAValue(15);
                game.Spielstand.SetMasterTeamBValue(5);
            }
        }

        // Alle Zeitslots fertig -> kein aktuelles Spiel mehr
        Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(false), Is.Null);
    }

    [Test]
    public void TestGetCurrentGameNumberOverAllReturnsNullWithoutGames()
    {
        Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(false), Is.Null);
    }

    [Test]
    public void TestGetCurrentGameNumberOverAll_Live_AdvancesWithManualMasterEntry()
    {
        for (int t = 0; t < 5; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 5), _teamBewerb.Teams);

        var timeslots = _teamBewerb.GetAllGames(false).Select(g => g.GameNumberOverAll).Distinct().OrderBy(n => n).ToList();

        foreach (var slot in timeslots)
        {
            Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.EqualTo(slot), $"live=true sollte bei Slot {slot} stehen");

            foreach (var game in _teamBewerb.GetAllGames(false).Where(g => g.GameNumberOverAll == slot))
            {
                game.Spielstand.SetMasterTeamAValue(15);
                game.Spielstand.SetMasterTeamBValue(5);
            }
        }

        Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.Null);
    }

    /// <summary>
    /// Ein Live-Ergebnis gilt erst als abgeschlossen, wenn alle (hier: 6, Standardfall <see cref="ITeamBewerb.Is8TurnsGame"/> = false)
    /// Kehren gemeldet wurden - nicht schon nach der ersten Kehre.
    /// </summary>
    [Test]
    public void TestGetCurrentGameNumberOverAll_Live_AdvancesOnlyAfterAllTurnsReported()
    {
        for (int t = 0; t < 5; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 5), _teamBewerb.Teams);

        var timeslots = _teamBewerb.GetAllGames(false).Select(g => g.GameNumberOverAll).Distinct().OrderBy(n => n).ToList();

        foreach (var slot in timeslots)
        {
            Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.EqualTo(slot), $"live=true sollte bei Slot {slot} stehen");

            foreach (var game in _teamBewerb.GetAllGames(false).Where(g => g.GameNumberOverAll == slot))
            {
                // Erst 5 von 6 Kehren -> Spiel (und damit die Runde) ist noch NICHT fertig
                game.Spielstand.SetLiveValues(MakeTurns(5));
                Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.EqualTo(slot),
                    "Runde darf nicht weiterspringen, solange noch nicht alle Kehren aller Bahnen gemeldet sind");

                // Jetzt alle 6 Kehren
                game.Spielstand.SetLiveValues(MakeTurns(6));
            }
        }

        Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.Null);
    }

    private static IOrderedEnumerable<IKehre> MakeTurns(int count) =>
        Enumerable.Range(1, count).Select(n => Kehre.Create(n, 15, 5)).OrderBy(k => k.KehrenNummer);

    /// <summary>
    /// End-to-End-Simulation des realen Anwendungsfalls: 9 Mannschaften, 4 Bahnen, Ergebnisse werden
    /// wie von echter StockTV-Hardware Bahn für Bahn per <see cref="TeamBewerb.SetStockTVResult"/> übertragen.
    /// Prüft, dass die "aktuelle Runde" (und damit die pausierende Mannschaft) korrekt weiterrückt,
    /// sobald alle Bahnen einer Runde ein Live-Ergebnis gemeldet haben.
    /// </summary>
    [Test]
    public void TestGetCurrentGameNumberOverAll_StockTVIntegration_9Teams4Courts()
    {
        for (int t = 0; t < 9; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 9 && p.Courts == 4), _teamBewerb.Teams);

        var timeslots = _teamBewerb.GetAllGames(false).Select(g => g.GameNumberOverAll).Distinct().OrderBy(n => n).ToList();
        Assert.That(timeslots.Count, Is.EqualTo(9));

        foreach (var slot in timeslots)
        {
            Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.EqualTo(slot),
                $"Vor Übertragung: Slot {slot} sollte aktuell sein");

            var pausingTeams = _teamBewerb.Teams.Where(team =>
                team.Games.Any(g => g.GameNumberOverAll == slot && g.IsPauseGame())).ToList();
            Assert.That(pausingTeams.Count, Is.EqualTo(1), $"In Slot {slot} sollte genau ein Team pausieren");

            var gamesInSlot = _teamBewerb.GetAllGames(false).Where(g => g.GameNumberOverAll == slot).ToList();
            Assert.That(gamesInSlot.Count, Is.EqualTo(4), $"In Slot {slot} sollten alle 4 Bahnen belegt sein");

            // "Bahn für Bahn" das komplette Ergebnis (alle 6 Kehren) übertragen - genau wie ein echtes StockTV-Gerät
            foreach (var game in gamesInSlot)
                SendStockTVTurns(game, turnCount: 6);

            Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.EqualTo(slot == timeslots.Last() ? (int?)null : slot + 1),
                $"Nach Übertragung aller 4 Bahnen sollte Slot {slot} abgeschlossen sein");
        }
    }

    /// <summary>
    /// Exakte Reproduktion des gemeldeten Bugs: Bahn 1-3 einer Runde sind komplett (6 Kehren) übertragen,
    /// Bahn 4 hat erst 4 von 6 Kehren gemeldet. Die "aktuelle Runde" (und damit die pausierende Mannschaft)
    /// darf erst weiterspringen, wenn auch Bahn 4 fertig ist.
    /// </summary>
    [Test]
    public void TestGetCurrentGameNumberOverAll_StockTVIntegration_StaysOnSlotUntilLastCourtFinishes()
    {
        for (int t = 0; t < 9; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 9 && p.Courts == 4), _teamBewerb.Teams);

        var slot1Games = _teamBewerb.GetAllGames(false).Where(g => g.GameNumberOverAll == 1).OrderBy(g => g.CourtNumber).ToList();
        Assert.That(slot1Games.Count, Is.EqualTo(4));

        var pauseTeamSlot1 = _teamBewerb.Teams.Single(team => team.Games.Any(g => g.GameNumberOverAll == 1 && g.IsPauseGame()));
        var pauseTeamSlot2 = _teamBewerb.Teams.Single(team => team.Games.Any(g => g.GameNumberOverAll == 2 && g.IsPauseGame()));

        // Bahn 1-3 komplett fertig (6 Kehren), Bahn 4 nur 4 von 6 Kehren
        SendStockTVTurns(slot1Games[0], turnCount: 6);
        SendStockTVTurns(slot1Games[1], turnCount: 6);
        SendStockTVTurns(slot1Games[2], turnCount: 6);
        SendStockTVTurns(slot1Games[3], turnCount: 4);

        Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.EqualTo(1),
            "Runde 1 ist noch nicht fertig, solange Bahn 4 noch spielt");
        Assert.That(pauseTeamSlot1.Games.Any(g => g.GameNumberOverAll == _teamBewerb.GetCurrentGameNumberOverAll(true) && g.IsPauseGame()), Is.True,
            "Solange Bahn 4 noch läuft, muss weiterhin das Team aus Runde 1 als pausierend gelten");

        // Jetzt auch Bahn 4 fertigspielen
        SendStockTVTurns(slot1Games[3], turnCount: 6);

        Assert.That(_teamBewerb.GetCurrentGameNumberOverAll(true), Is.EqualTo(2));
        Assert.That(pauseTeamSlot2.Games.Any(g => g.GameNumberOverAll == _teamBewerb.GetCurrentGameNumberOverAll(true) && g.IsPauseGame()), Is.True);
    }

    private void SendStockTVTurns(IGame game, int turnCount)
    {
        var tvResult = new TestStockTVResult
        {
            TVSettings = new StockTVSettings
            {
                Bahn = game.CourtNumber,
                Spielgruppe = _teamBewerb.SpielGruppe,
                GameModus = GameMode.Turnier,
                NextBahnModus = NextCourtMode.Left,
                MessageVersion = 1
            },
            Results = new List<IStockTVGameResult>
            {
                new StockTVGameResult((byte)game.GameNumberOverAll,
                    Enumerable.Range(1, turnCount).Select(n => new StockTVTurn { TurnNumber = n, PointsA = 15, PointsB = 5 }).ToList())
            }
        };

        _teamBewerb.SetStockTVResult(tvResult, gameOffset: 0);
    }

    private class TestStockTVResult : IStockTVResult
    {
        public byte[] Data => throw new NotImplementedException();
        public IStockTVSettings TVSettings { get; set; }
        public IList<IStockTVGameResult> Results { get; set; } = new List<IStockTVGameResult>();
        public IStockTVZielbewerb ResultZielbewerb => throw new NotImplementedException();
#pragma warning disable CS0067
        public event Action ResultChanged;
#pragma warning restore CS0067
        public IBroadCastTelegram AsBroadCastTelegram() => throw new NotImplementedException();
        public void SetResult(byte[] array) => throw new NotImplementedException();
    }
}
