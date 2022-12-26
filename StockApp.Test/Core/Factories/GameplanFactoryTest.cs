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
}