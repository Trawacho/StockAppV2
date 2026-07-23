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