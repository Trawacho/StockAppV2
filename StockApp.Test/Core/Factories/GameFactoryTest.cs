using NUnit.Framework;
using StockApp.Core.Factories;
using System.Linq;

namespace StockApp.Test.Core.Factories;


public class GameFactoryTest
{
    bool _triggerChange;

    [SetUp]
    public void Setup()
    {
        _triggerChange = false;
    }

    /// <summary>
    /// Test <see cref="GameFactory.CreateGames(int, bool, int, bool)"/> with 3,5,7,9..21 teams and 1...5 rounds
    /// twoBreaks, triggerChange and 2,4,6,8.....22 teams are not tested yet
    /// </summary>
    [Test]
    public void TestGameFactoryFunction()
    {
        for (int rounds = 1; rounds < 5; rounds++)
        {
            for (int teams = 3; teams <= 21; teams += 2)
            {
                var games = GameFactory.CreateGames(teams, 1, rounds, _triggerChange).OrderBy(a => a.GameNumberOverAll).ThenBy(b => b.CourtNumber);

                //jeder gegen jeden, jeder Startnummer testen
                for (int startNummer = 1; startNummer <= teams; startNummer++)
                {
                    //pro Runde einen Aussetzer (als Team A)
                    Assert.IsTrue(games.Where(g => g.IsBreakGame && g.TeamA == startNummer).Count() == rounds);

                    //kein Spiel gegen sich selber
                    Assert.IsFalse(games.Any(g => g.TeamA == g.TeamB));

                    //pro Runde ein Spiel weniger als Mannschaften
                    Assert.IsTrue(games.Where(g => (g.TeamA == startNummer || g.TeamB == startNummer) && !g.IsBreakGame).Count() == (teams - 1) * rounds);


                    var filter1 = games.Where(b => !b.IsBreakGame).Where(g => startNummer == g.TeamA || startNummer == g.TeamB);

                    //auf jeder Seite gleich viele Gegner
                    Assert.IsTrue(filter1.Where(f => f.TeamA != startNummer).Count() == filter1.Where(f => f.TeamB != startNummer).Count());

                    //jeden Gegner nur 1x, durch Distinct() keine Beachtung der Runden! 
                    var q1 = from g in filter1 select new { a = g.TeamA };
                    var q2 = from g in filter1 select new { a = g.TeamB };
                    Assert.IsTrue(q1.Union(q2).Where(a => a.a != startNummer).Distinct().Count() == (teams - 1));

                    //Bahnen
                    var courts = filter1.Select(g => new { g.CourtNumber })
                                    .Distinct();
                    Assert.IsTrue(courts.Count() == teams / 2);
                }


            }
        }

    }

    [Test]
    public void TestGameFacotry2()
    {
        //4 Runden 
        for (int rounds = 1; rounds < 5; rounds++)
        {
            // von 4 bis 22 Mannschaften , nur eine gerade Anzahl an Mannschaften
            for (int teams = 4; teams <= 22; teams += 2) 
            {
                //Spiele generieren
                var games = GameFactory.CreateGames2(teams, rounds).OrderBy(a => a.GameNumberOverAll).ThenBy(b => b.CourtNumber);

                //jeder gegen jeden, jeder Startnummer testen
                for (int startNummer = 1; startNummer <= teams; startNummer++)
                {
                    //pro Runde ein Spiel weniger als Mannschaften
                    Assert.IsTrue(games.Where(g => (g.TeamA == startNummer || g.TeamB == startNummer) && !g.IsBreakGame).Count() == (teams - 1) * rounds);

                    var filter1 = games.Where(b => !b.IsBreakGame).Where(g => startNummer == g.TeamA || startNummer == g.TeamB);

                    //jeden Gegner nur 1x, durch Distinct() keine Beachtung der Runden! 
                    var q1 = from g in filter1 select new { a = g.TeamA };
                    var q2 = from g in filter1 select new { a = g.TeamB };
                    Assert.IsTrue(q1.Union(q2).Where(a => a.a != startNummer).Distinct().Count() == (teams - 1));
                }
                //pro Runde einen Aussetzer 
                Assert.IsTrue(games.Where(g => g.IsBreakGame).Count() == teams / 2 * rounds);

                //kein Spiel gegen sich selber
                Assert.IsFalse(games.Any(g => g.TeamA == g.TeamB));

                var courts = games.Where(b => !b.IsBreakGame).Select(g => new { g.CourtNumber }).Distinct();

                //Anzahl Bahnen
                Assert.IsTrue(courts.Count() == teams / 2);

                //Auf Bahnen 1,3,5,7,9 hat TeamA Anspiel
                Assert.IsFalse(games.Where(g => g.CourtNumber % 2 != 0).Any(t => !t.IsTeamA_Starting));

                //Auf Bahnen 2,3,6,8,10 hat TeamB Anspiel
                Assert.IsFalse(games.Where(g => g.CourtNumber % 2 == 0).Any(t => t.IsTeamA_Starting));


            }
        }

    }
}
