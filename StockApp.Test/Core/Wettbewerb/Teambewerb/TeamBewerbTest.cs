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
}
