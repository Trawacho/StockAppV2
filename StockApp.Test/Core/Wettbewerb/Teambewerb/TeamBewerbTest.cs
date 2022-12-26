using NUnit.Framework;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
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
        Assert.IsTrue(_teamBewerb.GetCountOfGames() == 0);
        Assert.IsFalse(_teamBewerb.GetAllGames().Any());
        Assert.IsTrue(_teamBewerb.GetCountOfGamesPerCourt() == 0);
        Assert.IsFalse(_teamBewerb.GetGamesOfCourt(1).Any());
        Assert.IsFalse(_teamBewerb.GetTeamsRanked(false).Any());

        int t = 7;
        int f = t / 2;

        for (int i = 0; i < t; i++)
        {
            _teamBewerb.AddNewTeam();
        }

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans()
                        .First(p => p.Teams == 7), _teamBewerb.Teams);


        Assert.IsTrue(_teamBewerb.GetTeamsRanked(false).Count() == t);
        Assert.IsTrue(_teamBewerb.GetCountOfGames() == t * f);
        Assert.IsTrue(_teamBewerb.GetAllGames(false).Count() == t * f);
        Assert.IsTrue(_teamBewerb.GetCountOfGamesPerCourt() >= t);
        Assert.IsTrue(_teamBewerb.GetGamesOfCourt(1).Count() == t);



    }
}
