using NUnit.Framework;
using StockApp.Core.Factories;
using System.Linq;

namespace StockApp.Test.Core.Factories;


public class GameFactoryTest
{

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestGameplanFacotryFunctionGeneral()
    {

        foreach (var gameplan in GamePlanFactory.LoadAllGameplans())
        {
            var allGames = gameplan.GetAllGames();

            for (int startNr = 1; startNr <= gameplan.Teams; startNr++)
            {
                var gamesFromStartNr = allGames.Where(g => g.game.A == startNr || g.game.B == startNr);

                //Gegen jeden Gegener ein Spiel
                if(!gameplan.Name.Contains("Vergleich"))
                Assert.That(gamesFromStartNr.Select(t => t.game.A).
                    Union(gamesFromStartNr.Select(t => t.game.B))
                    .Distinct().OrderBy(x => x)
                    .SequenceEqual(Enumerable.Range(1, gameplan.Teams)), Is.True, $"ID:{gameplan.ID}..StartNr:{startNr} hat nicht gegen jeden Gegner ein Spiel");



                //kein Spiel gegen sich selber
                foreach(var game in allGames.Where(g=>g.game.A == startNr))
                {
                    Assert.That(game.game.B, Is.Not.EqualTo(startNr));
                }
                foreach (var game in allGames.Where(g => g.game.B == startNr))
                {
                    Assert.That(game.game.A, Is.Not.EqualTo(startNr));
                }
            }
        }
    }

   
    [Test]
    public void TestGameplanFactory1()
    {
        //T E S T für ungerade Anzahl an Mannschaften mit Standardspielplan
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans().Where(t => t.Teams % 2 != 0))
        {
            var games = gameplan.GetAllGames();

            //Anzahl der Runden ist gleich Anzahl der Mannschaften
            Assert.That(gameplan.GameplanGamenumbers.Count(), Is.EqualTo(gameplan.Teams));

            //jeder gegen jeden, jeder Startnummer testen
            for (int startNummer = 1; startNummer <= gameplan.Teams; startNummer++)
            {
                var gamesFromStartNr = games.Where(g => g.game.A == startNummer || g.game.B == startNummer);

                //auf jeder Seite gleich viele Spiele
                Assert.That(gamesFromStartNr.Where(g=>g.game.A == startNummer).Count(), Is.EqualTo(gamesFromStartNr.Where(g=>g.game.B==startNummer).Count()));

                //anzahl der Startnummern gleich anzahl der Teams
                Assert.That(gamesFromStartNr.Select(t => t.game.A).Union(gamesFromStartNr.Select(t => t.game.B)).Distinct().Count(), Is.EqualTo(gameplan.Teams));

                //Bahnen
                Assert.That(gamesFromStartNr.Select(t => t.game.Court).Distinct().Count(), Is.EqualTo(gameplan.Teams/2));

                foreach (var r in gameplan.GameplanGamenumbers)
                {
                    //pro spielrunde max ein Spiel
                    Assert.That(r.Games.Where(x => x.A == startNummer || x.B == startNummer).Count(), Is.LessThanOrEqualTo(1));

                    //kein Spiel gegen sich selber
                    Assert.That(r.Games.Where(x => x.A == startNummer && x.B == startNummer).Any(), Is.Not.True);

                    //Anzahl der Spiele
                    Assert.That(r.Games.Count(), Is.EqualTo((gameplan.Teams - 1) / 2));
                }
            }
        }
    }

    [Test]
    public void TestGameplanFactory2()
    {
        //T E S T für gerade Anzahl an Mannschaften
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans().Where(t => t.Teams % 2 == 0))
        {
            var games = gameplan.GetAllGames();
            //jeder gegen jeden, jeder Startnummer testen
            for (int startNummer = 1; startNummer <= gameplan.Teams; startNummer++)
            {
                var gamesFromStartNr = games.Where(g => g.game.A == startNummer || g.game.B == startNummer);

                if (gameplan.Courts > 3)
                {
                    Assert.That(gamesFromStartNr.Count(t => t.game.A == startNummer), Is.GreaterThanOrEqualTo(1), $"gameplan:{gameplan.ID}. Startnummer {startNummer} ist auf A Seite nicht mind. 1x");
                    Assert.That(gamesFromStartNr.Count(t => t.game.B == startNummer), Is.GreaterThanOrEqualTo(1), $"gameplan:{gameplan.ID}. Startnummer {startNummer} ist auf B Seite nicht mind. 1x");
                }
                foreach (var r in gameplan.GameplanGamenumbers)
                {
                    //pro spielrunde max ein Spiel
                    Assert.That(r.Games.Where(x => x.A == startNummer || x.B == startNummer).Count(), Is.LessThanOrEqualTo(1), $"gameplan:{gameplan.ID}..Startnummer {startNummer}: Pro Spielrunde max. ein Spiel ");

                    //kein Spiel gegen sich selber
                    Assert.That(r.Games.Where(x => x.A == startNummer && x.B == startNummer).Any(), Is.Not.True);
                }
            }
        }
    }
}
