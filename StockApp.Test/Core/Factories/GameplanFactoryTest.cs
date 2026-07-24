using NUnit.Framework;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Linq;

namespace StockApp.Test.Core.Factories;

public class GameplanFactoryTest
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestLoadAllGameplans()
    {
        var jsonGameplans = GamePlanFactory.LoadAllGameplans();
        Assert.That(jsonGameplans.Last().Teams, Is.GreaterThan(0));
        Assert.That(jsonGameplans.Take(5).Last().Courts, Is.GreaterThan(0));
    }


    /// <summary>
    /// Grenzwerte laut CLAUDE.md: gpf.json muss Spielpläne für 2 (Minimum) und 22 (Maximum) Teams
    /// enthalten, und <see cref="GamePlanFactory.MatchTeamAndGames"/> muss für JEDEN davon einen
    /// gültigen, vollständigen Spielplan erzeugen (kein Selbstspiel, jedes Team taucht auf).
    /// </summary>
    [TestCase(2)]
    [TestCase(22)]
    public void TestMatchTeamAndGames_BoundaryTeamCounts_ProducesValidSchedule(int teamCount)
    {
        var gameplansForCount = GamePlanFactory.LoadAllGameplans().Where(p => p.Teams == teamCount).ToList();
        Assert.That(gameplansForCount, Is.Not.Empty, $"gpf.json sollte mindestens einen Spielplan für {teamCount} Teams enthalten");

        foreach (var gamePlan in gameplansForCount)
        {
            var teamBewerb = TeamBewerb.Create(1);
            for (int t = 0; t < teamCount; t++)
                teamBewerb.AddNewTeam();

            GamePlanFactory.MatchTeamAndGames(gamePlan, teamBewerb.Teams);

            Assert.That(teamBewerb.Teams.Count(), Is.EqualTo(teamCount));

            var realGames = teamBewerb.GetAllGames(withBreaks: false).ToList();
            Assert.That(realGames, Is.Not.Empty, $"Plan {gamePlan.ID} ({teamCount} Teams) sollte reale Spiele erzeugen");
            Assert.That(realGames.Any(g => g.TeamA == g.TeamB), Is.False, $"Plan {gamePlan.ID}: kein Team darf gegen sich selbst spielen");

            // Jedes Team muss mindestens an einem Spiel beteiligt sein
            foreach (var team in teamBewerb.Teams)
            {
                Assert.That(realGames.Any(g => g.TeamA == team || g.TeamB == team), Is.True,
                    $"Plan {gamePlan.ID}: Team mit Startnummer {team.StartNumber} hat kein einziges Spiel");
            }
        }
    }

    /// <summary>
    /// gpf.json wird von Hand gepflegt (siehe CLAUDE.md / Absprache mit dem Projektinhaber) - dieser Test
    /// prüft die Kern-Invarianten jedes einzelnen hinterlegten Spielplans direkt auf den rohen Paarungen
    /// (<see cref="IGameplanGame.A"/>/<see cref="IGameplanGame.B"/>), unabhängig von <see cref="TeamBewerb"/>:
    /// eine Mannschaft darf nie gegen sich selbst antreten. Ausnahme: das Sentinel-Paar (0,0), mit dem eine
    /// Bahn in einer Spielrunde bewusst leer bleibt (siehe z.B. Plan-ID 20101, "2x10 Teams, 10 Bahnen, 1 Aussetzer").
    /// </summary>
    [Test]
    public void AllGameplans_NoTeamPlaysAgainstItself()
    {
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans())
        {
            foreach (var (gamenumber, game) in gameplan.GetAllGames())
            {
                bool isEmptyCourtSentinel = game.A == 0 && game.B == 0;

                Assert.That(game.A == 0, Is.EqualTo(game.B == 0),
                    $"Plan {gameplan.ID} ({gameplan.Name}), Runde {gamenumber}, Bahn {game.Court}: " +
                    $"0 als Startnummer darf nur als Leer-Bahn-Sentinel (0,0) vorkommen, nicht einseitig (A={game.A}, B={game.B}).");

                if (!isEmptyCourtSentinel)
                {
                    Assert.That(game.A, Is.Not.EqualTo(game.B),
                        $"Plan {gameplan.ID} ({gameplan.Name}), Runde {gamenumber}, Bahn {game.Court}: " +
                        $"Team {game.A} spielt gegen sich selbst.");
                }
            }
        }
    }

    /// <summary>
    /// Bahnen liegen in der Halle immer nebeneinander und müssen im Spielplan lückenlos ab 1 durchnummeriert sein.
    /// Manche Pläne nutzen bewusst nicht in jeder Runde alle <see cref="IGameplan.Courts"/> Bahnen (z.B. Plan 631,
    /// 831, 841 - siehe <see cref="VariableCourtCountPlans_FactoryDerivesPausesFromActualPairsPerRound"/>), deshalb
    /// wird hier nur "lückenlos ab 1" geprüft, nicht "immer exakt alle Courts".
    /// </summary>
    [Test]
    public void AllGameplans_CourtsAreContiguousFromOneToN()
    {
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans())
        {
            foreach (var gameround in gameplan.GameplanGamenumbers)
            {
                var courtsInRound = gameround.Games.Select(g => g.Court).OrderBy(c => c).ToList();

                Assert.That(courtsInRound, Is.EqualTo(Enumerable.Range(1, courtsInRound.Count).ToList()),
                    $"Plan {gameplan.ID} ({gameplan.Name}), Runde {gameround.Number}: " +
                    $"Bahnen müssen lückenlos ab 1 durchnummeriert sein, tatsächlich [{string.Join(", ", courtsInRound)}].");
                Assert.That(courtsInRound.Count, Is.LessThanOrEqualTo(gameplan.Courts),
                    $"Plan {gameplan.ID} ({gameplan.Name}), Runde {gameround.Number}: " +
                    $"nutzt {courtsInRound.Count} Bahnen, mehr als die deklarierten {gameplan.Courts}.");
            }
        }
    }

    /// <summary>
    /// Schutz gegen Tippfehler beim manuellen Pflegen von gpf.json: jede referenzierte Startnummer muss
    /// innerhalb von 1..Teams liegen (0 ist als Leer-Bahn-Sentinel erlaubt, siehe <see cref="AllGameplans_NoTeamPlaysAgainstItself"/>).
    /// </summary>
    [Test]
    public void AllGameplans_TeamNumbersAreWithinValidRange()
    {
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans())
        {
            foreach (var (gamenumber, game) in gameplan.GetAllGames())
            {
                foreach (var startNumber in new[] { game.A, game.B })
                {
                    if (startNumber == 0) continue;

                    Assert.That(startNumber, Is.InRange(1, gameplan.Teams),
                        $"Plan {gameplan.ID} ({gameplan.Name}), Runde {gamenumber}, Bahn {game.Court}: " +
                        $"Startnummer {startNumber} liegt außerhalb von 1..{gameplan.Teams}.");
                }
            }
        }
    }

    /// <summary>
    /// Manche Pläne (z.B. 631 "6 Teams, 3(2) Bahnen, 1 Aussetzer", 831 "8 Teams, 3(2) Bahnen, 3 Aussetzer",
    /// 841 "8 Teams, 4(3) Bahnen, 1 Aussetzer") nutzen bewusst nicht in jeder Runde alle <see cref="IGameplan.Courts"/>
    /// Bahnen - in einzelnen Runden spielen dadurch mehr Teams gleichzeitig Aussetzer, statt die überzählige Bahn
    /// per (0,0)-Sentinel leer zu halten. <see cref="GamePlanFactory.MatchTeamAndGames"/> berücksichtigt <see cref="IGameplan.Courts"/>
    /// dafür gar nicht direkt - Aussetzer werden ausschließlich per Mengendifferenz (welche Teams tauchen in der
    /// Zeile nicht auf) ermittelt, unabhängig davon, wie viele Bahnen die Zeile tatsächlich befüllt. Dieser Test
    /// belegt, dass genau das für alle drei Pläne korrekt funktioniert: pro Runde so viele reale Spiele wie
    /// tatsächlich befüllte Bahnen, und dementsprechend mehr Aussetzer in den "kurzen" Runden.
    /// </summary>
    [TestCase(631)]
    [TestCase(831)]
    [TestCase(841)]
    public void VariableCourtCountPlans_FactoryDerivesPausesFromActualPairsPerRound(int gameplanId)
    {
        var gameplan = GamePlanFactory.LoadAllGameplans().Single(p => p.ID == gameplanId);

        var teamBewerb = TeamBewerb.Create(1);
        for (int t = 0; t < gameplan.Teams; t++)
            teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(gameplan, teamBewerb.Teams);

        var allGames = teamBewerb.GetAllGames().ToList();

        foreach (var gameround in gameplan.GameplanGamenumbers)
        {
            int pairsThisRound = gameround.Games.Count(g => g.A != 0 && g.B != 0);
            int expectedPauses = gameplan.Teams - (pairsThisRound * 2);

            var gamesThisRound = allGames.Where(g => g.GameNumber == gameround.Number).ToList();
            var realGamesThisRound = gamesThisRound.Where(g => !g.IsPauseGame()).ToList();
            var pauseGamesThisRound = gamesThisRound.Where(g => g.IsPauseGame()).ToList();

            Assert.That(realGamesThisRound, Has.Count.EqualTo(pairsThisRound),
                $"Plan {gameplanId}, Runde {gameround.Number}: erwarte {pairsThisRound} reale Spiele (so viele Paare wie in der Planzeile).");
            Assert.That(pauseGamesThisRound, Has.Count.EqualTo(expectedPauses),
                $"Plan {gameplanId}, Runde {gameround.Number}: erwarte {expectedPauses} Aussetzer bei nur {pairsThisRound} befüllten Bahnen von {gameplan.Courts}.");

            var courtsUsed = realGamesThisRound.Select(g => g.CourtNumber).OrderBy(c => c).ToList();
            Assert.That(courtsUsed, Is.EqualTo(Enumerable.Range(1, pairsThisRound).ToList()),
                $"Plan {gameplanId}, Runde {gameround.Number}: reale Spiele sollten lückenlos Bahn 1..{pairsThisRound} belegen.");
        }

        // Über das gesamte Turnier: jedes Team hat pro Runde genau ein Spiel (real oder Aussetzer) - nie mehr, nie weniger.
        foreach (var team in teamBewerb.Teams)
        {
            Assert.That(team.Games.Select(g => g.GameNumber).Distinct().Count(), Is.EqualTo(gameplan.GameplanGamenumbers.Count()),
                $"Plan {gameplanId}: Team {team.StartNumber} sollte in jeder der {gameplan.GameplanGamenumbers.Count()} Runden genau ein Spiel (Real oder Aussetzer) haben.");
        }
    }

    /// <summary>
    /// Kern-Invariante von Split-Gruppen (siehe CLAUDE.md "Split-Gruppe"): innerhalb einer einzelnen Runde
    /// dürfen sich die beiden Gruppenhälften niemals dieselbe Bahn teilen und niemals gegeneinander antreten -
    /// das würde die Trennung der beiden Halb-Turniere aufheben. Dass sich eine Bahn über den Turnierverlauf
    /// (also über mehrere Runden hinweg) zwischen den Hälften verschiebt, ist dagegen ausdrücklich gewollt
    /// (z.B. Plan 1252: Bahn 3 gehört in Runde 1 zur ersten Hälfte, in Runde 4 zur zweiten).
    /// </summary>
    [Test]
    public void SplitGruppenPlans_HalvesNeverShareACourtWithinTheSameRound()
    {
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans().Where(g => g.IsSplit))
        {
            int half = gameplan.Teams / 2;
            foreach (var gameround in gameplan.GameplanGamenumbers)
            {
                var firstHalfCourts = gameround.Games.Where(g => g.A != 0 && g.B != 0 && g.A <= half && g.B <= half).Select(g => g.Court).ToList();
                var secondHalfCourts = gameround.Games.Where(g => g.A != 0 && g.B != 0 && g.A > half && g.B > half).Select(g => g.Court).ToList();
                var mixedPairs = gameround.Games.Where(g => g.A != 0 && g.B != 0 && (g.A <= half) != (g.B <= half)).ToList();

                var overlap = firstHalfCourts.Intersect(secondHalfCourts).ToList();
                Assert.That(overlap, Is.Empty,
                    $"Plan {gameplan.ID} ({gameplan.Name}), Runde {gameround.Number}: Bahn(en) [{string.Join(",", overlap)}] werden von beiden Gruppenhälften GLEICHZEITIG genutzt.");

                Assert.That(mixedPairs, Is.Empty,
                    $"Plan {gameplan.ID} ({gameplan.Name}), Runde {gameround.Number}: Paarung(en) über die Gruppenhälften-Grenze hinweg gefunden.");
            }
        }
    }

    /// <summary>
    /// Der Anspiel-Spiegel-Zweig in <see cref="GamePlanFactory.MatchTeamAndGames"/> (aktiv wenn
    /// <see cref="IGameplan.IsSplit"/> und <see cref="IGameplan.Courts"/> % 4 == 2) hatte bisher keinen
    /// eigenen Test - er wird nur für die zweite Gruppenhälfte (Bahn > Courts/2) ausgelöst und dreht dort
    /// die sonst übliche "ungerade Bahn -> TeamA beginnt"-Regel um.
    /// </summary>
    [Test]
    public void SplitGruppenAnspielMirror_FiresForCourtsMod4Equals2()
    {
        var gameplan = GamePlanFactory.LoadAllGameplans().Single(p => p.ID == 20101); // 2x10 Teams, 10 Bahnen -> Courts % 4 == 2
        Assert.That(gameplan.Courts % 4, Is.EqualTo(2));

        var teamBewerb = TeamBewerb.Create(1);
        for (int t = 0; t < gameplan.Teams; t++)
            teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(gameplan, teamBewerb.Teams);

        var realGames = teamBewerb.GetAllGames(withBreaks: false).ToList();

        foreach (var game in realGames)
        {
            bool expectedNormal = game.CourtNumber % 2 != 0;
            bool expected = game.CourtNumber > gameplan.Courts / 2 ? !expectedNormal : expectedNormal;

            Assert.That(game.IsTeamA_Starting, Is.EqualTo(expected),
                $"Bahn {game.CourtNumber} (Runde {game.RoundOfGame}): erwarte IsTeamA_Starting={expected} " +
                $"({(game.CourtNumber > gameplan.Courts / 2 ? "gespiegelt, da 2. Gruppenhälfte" : "normal")}).");
        }
    }

    [Test]
    public void TestLoadAndMatchGameplan()
    {
        for (int maxTeams = 3; maxTeams <= 11; maxTeams++)
        {
            var _teamBewerb = TeamBewerb.Create(1);
            _teamBewerb = TeamBewerb.Create(1);
            for (int t = 0; t < maxTeams; t++)
            {
                _teamBewerb.AddNewTeam();
            }

            foreach (var t in _teamBewerb.Teams)
                t.ClearGames();

            foreach (var gamePlan in GamePlanFactory.LoadAllGameplans().Where(x=> x.Teams == _teamBewerb.Teams.Count()))
            {
                GamePlanFactory.MatchTeamAndGames(gamePlan, _teamBewerb.Teams, 1);
            }

        }
    }

    /// <summary>
    /// Additiv erzeugte Runden (erst Runde 1-2, danach Runde 3-4 mit startRound/startGameNumberOverAll angehängt)
    /// müssen exakt dieselben Paarungen/Bahnen/Nummerierungen liefern wie eine Generierung aller 4 Runden in einem Schritt.
    /// </summary>
    [Test]
    public void TestAppendRoundsMatchesOneShotGeneration()
    {
        for (int maxTeams = 3; maxTeams <= 11; maxTeams++)
        {
            foreach (var gamePlan in GamePlanFactory.LoadAllGameplans().Where(x => x.Teams == maxTeams))
            {
                var oneShotBewerb = TeamBewerb.Create(1);
                for (int t = 0; t < maxTeams; t++) oneShotBewerb.AddNewTeam();
                GamePlanFactory.MatchTeamAndGames(gamePlan, oneShotBewerb.Teams, rounds: 4);

                var stepBewerb = TeamBewerb.Create(1);
                for (int t = 0; t < maxTeams; t++) stepBewerb.AddNewTeam();
                GamePlanFactory.MatchTeamAndGames(gamePlan, stepBewerb.Teams, rounds: 2);

                int nextGameNumberOverAll = stepBewerb.Teams.SelectMany(t => t.Games).Max(g => g.GameNumberOverAll) + 1;
                GamePlanFactory.MatchTeamAndGames(gamePlan, stepBewerb.Teams, rounds: 4,
                    startRound: 3, startGameNumberOverAll: nextGameNumberOverAll);

                foreach (var oneShotTeam in oneShotBewerb.Teams)
                {
                    var stepTeam = stepBewerb.Teams.First(t => t.StartNumber == oneShotTeam.StartNumber);

                    // bewusst die rohe (nicht Distinct-gefilterte) Liste vergleichen, um doppelte/fehlende Spiele zu erkennen
                    Assert.That(stepTeam.Games.Count, Is.EqualTo(oneShotTeam.Games.Count),
                        $"Games count mismatch for {maxTeams} teams, plan {gamePlan.ID}");

                    var oneShotSorted = oneShotTeam.Games.OrderBy(g => g.GameNumberOverAll).ToList();
                    var stepSorted = stepTeam.Games.OrderBy(g => g.GameNumberOverAll).ToList();

                    for (int i = 0; i < oneShotSorted.Count; i++)
                    {
                        Assert.That(stepSorted[i].RoundOfGame, Is.EqualTo(oneShotSorted[i].RoundOfGame));
                        Assert.That(stepSorted[i].GameNumber, Is.EqualTo(oneShotSorted[i].GameNumber));
                        Assert.That(stepSorted[i].GameNumberOverAll, Is.EqualTo(oneShotSorted[i].GameNumberOverAll));
                        Assert.That(stepSorted[i].CourtNumber, Is.EqualTo(oneShotSorted[i].CourtNumber));
                        Assert.That(stepSorted[i].IsTeamA_Starting, Is.EqualTo(oneShotSorted[i].IsTeamA_Starting));
                        Assert.That(stepSorted[i].TeamA?.StartNumber, Is.EqualTo(oneShotSorted[i].TeamA?.StartNumber));
                        Assert.That(stepSorted[i].TeamB?.StartNumber, Is.EqualTo(oneShotSorted[i].TeamB?.StartNumber));
                    }
                }
            }
        }
    }
}