using NUnit.Framework;
using StockApp.Core.Factories;
using System;
using System.Collections.Generic;
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

        foreach (var gameplan in GamePlanFactory.LoadAllGameplans().Where(g => !g.IsSplit))
        {
            var allGames = gameplan.GetAllGames();

            for (int startNr = 1; startNr <= gameplan.Teams; startNr++)
            {
                var gamesFromStartNr = allGames.Where(g => g.game.A == startNr || g.game.B == startNr);

                //Gegen jeden Gegener ein Spiel
                if (!gameplan.IsVergleich)
                    Assert.That(gamesFromStartNr.Select(t => t.game.A).
                        Union(gamesFromStartNr.Select(t => t.game.B))
                        .Distinct().OrderBy(x => x)
                        .SequenceEqual(Enumerable.Range(1, gameplan.Teams)), Is.True, $"ID:{gameplan.ID}..StartNr:{startNr} hat nicht gegen jeden Gegner ein Spiel");
            }
        }

        foreach (var gameplan in GamePlanFactory.LoadAllGameplans().Where(g => g.IsSplit))
        {
            var games = gameplan.GetAllGames().Where(g => g.game.A <= gameplan.Teams / 2 && g.game.B <= gameplan.Teams / 2).Where(t => t.game.A != 0 && t.game.B != 0);
            var ersteStartnummer = games.Select(g => g.game.A).Union(games.Select(g => g.game.B)).Distinct().Min();
            var letzteStartnummer = games.Select(g => g.game.A).Union(games.Select(g => g.game.B)).Distinct().Max();
            var anzzahlStartnummern = letzteStartnummer - ersteStartnummer + 1;
           
            for (int startnummer = ersteStartnummer; startnummer <= letzteStartnummer; startnummer++)
            {
                var gamesFromStartNr = games.Where(g => g.game.A == startnummer || g.game.B == startnummer);
                //Gegen jeden Gegener ein Spiel
                Assert.That(gamesFromStartNr.Select(t => t.game.A).
                      Union(gamesFromStartNr.Select(t => t.game.B))
                         .Distinct().OrderBy(x => x)
                            .SequenceEqual(Enumerable.Range(ersteStartnummer, anzzahlStartnummern)), Is.True, $"ID:{gameplan.ID}..StartNr:{startnummer} hat nicht gegen jeden Gegner ein Spiel");
               
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
                Assert.That(gamesFromStartNr.Where(g => g.game.A == startNummer).Count(), Is.EqualTo(gamesFromStartNr.Where(g => g.game.B == startNummer).Count()));

                //anzahl der Startnummern gleich anzahl der Teams
                Assert.That(gamesFromStartNr.Select(t => t.game.A).Union(gamesFromStartNr.Select(t => t.game.B)).Distinct().Count(), Is.EqualTo(gameplan.Teams));

                //Bahnen
                Assert.That(gamesFromStartNr.Select(t => t.game.Court).Distinct().Count(), Is.EqualTo(gameplan.Teams / 2));

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

    /// <summary>
    /// Test, dass Jedes Team pro Runde nur 1x spielt und nie gegen sich selber
    /// </summary>
    [Test]
    public void TestGameplanFactory_Basic()
    {
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans())
        {
            var games = gameplan.GetAllGames();
            for (int startNummer = 1; startNummer <= gameplan.Teams; startNummer++)
            {
                foreach (var gameNumber in gameplan.GameplanGamenumbers)
                {
                    //pro spielrunde max ein Spiel
                    Assert.That(
                        gameNumber.Games.Where(x => x.A == startNummer || x.B == startNummer).Count(), 
                        Is.LessThanOrEqualTo(1),
                        $"gameplan:{gameplan.ID}..Startnummer:{startNummer}..Runde:{gameNumber.Number}: Pro Spielrunde max. ein Spiel ");

                    //kein Spiel gegen sich selber
                    Assert.That(
                        gameNumber.Games.Where(x => x.A == startNummer && x.B == startNummer).Any(),
                        Is.Not.True, 
                        $"gameplan:{gameplan.ID}..Startnummer:{startNummer}..Runde:{gameNumber.Number}: spielt gegen sich selber");
                }
            }
        }
    }


    [Test]
    public void TestGameplanFactory2()
    {
        //T E S T für gerade Anzahl an Mannschaften ohne SplitGruppen
        foreach (var gameplan in GamePlanFactory.LoadAllGameplans().Where(t => t.Teams % 2 == 0 && !t.IsSplit))
        {
            if (gameplan.ID == 842) continue; //842 ist Spielplan aus MERLIN. Allerdings ist dieser Test zu hinterfragen.

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
               
            }
        }

        foreach (var gameplan in GamePlanFactory.LoadAllGameplans().Where(t => t.Teams % 2 == 0 && t.IsSplit))
        {
            List<IEnumerable<(int gamenumber, IGameplanGame game)>> gameList = new()
            {
                gameplan.GetAllGames().Where(g => g.game.A <= gameplan.Teams / 2 && g.game.B <= gameplan.Teams / 2),
                gameplan.GetAllGames().Where(g => g.game.A > gameplan.Teams / 2 && g.game.B > gameplan.Teams / 2)
            };

            foreach (var games in gameList)
            {
                var ersteStartnummer = games.Select(g => g.game.A).Union(games.Select(g => g.game.B)).Distinct().Min();
                var letzteStartnummer = games.Select(g => g.game.A).Union(games.Select(g => g.game.B)).Distinct().Max();

                for (int startNummer = ersteStartnummer; startNummer <= letzteStartnummer; startNummer++)
                {
                    var gamesFromStartNr = games.Where(g => g.game.A == startNummer || g.game.B == startNummer);

                    if (gameplan.Courts > 3)
                    {
                        Assert.That(gamesFromStartNr.Count(t => t.game.A == startNummer), Is.GreaterThanOrEqualTo(1), $"gameplan:{gameplan.ID}. Startnummer {startNummer} ist auf A Seite nicht mind. 1x");
                        Assert.That(gamesFromStartNr.Count(t => t.game.B == startNummer), Is.GreaterThanOrEqualTo(1), $"gameplan:{gameplan.ID}. Startnummer {startNummer} ist auf B Seite nicht mind. 1x");
                    }
                   
                }
            }

        }

    }
}
