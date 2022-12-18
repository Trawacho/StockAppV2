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
    public void Test1()
    {
        var jsonGameplans = GamePlanFactory.LoadAllGameplans();
        Assert.That(jsonGameplans.Last().Teams, Is.GreaterThan(0));
        Assert.That(jsonGameplans.Take(5).Last().Courts, Is.GreaterThan(0));
    }


    [Test]
    public void Test3()
    {
        var _teamBewerb = TeamBewerb.Create(1);
        _teamBewerb = TeamBewerb.Create(1);
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddVirtualTeams(1);


        var factoryGames = GamePlanFactory.LoadAllGameplans().Where(x => x.Teams == 5 && x.Courts == 2).First();
        GamePlanFactory.MatchTeamAndGames(factoryGames, _teamBewerb.Teams);

       
    }
}
