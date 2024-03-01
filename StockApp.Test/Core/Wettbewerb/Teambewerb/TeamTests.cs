using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Linq;

namespace StockApp.Test;

public class TeamTests
{
    ITeam _team1;
    [SetUp]
    public void Setup()
    {
        _team1 = Team.Create("erstes Team");
        _team1.AddGame(Game.Create(1, 1, 1));
    }

    [Test]
    public void TestPublicFunctions()
    {
        Assert.That(_team1.Games.Count == 1, Is.True);
        _team1.ClearGames();
        Assert.That(_team1.Games.Count == 0, Is.True);

        _team1.AddPlayer();
        Assert.That(_team1.Players.Any(), Is.True);
        var player = _team1.Players.First();
        _team1.RemovePlayer(player);
        Assert.That(_team1.Players.Any(), Is.False);

    }

}
