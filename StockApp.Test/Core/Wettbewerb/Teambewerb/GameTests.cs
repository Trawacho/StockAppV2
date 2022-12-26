using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Test;

public class GameTests
{
    private IGame _game;
    private ITeam _team1;
    private ITeam _team2;

    [SetUp]
    public void Setup()
    {
        _team1 = Team.Create("erstes Team");
        _team1.StartNumber = 1;
        _team2 = Team.Create("zweites Team");
        _team2.StartNumber = 2;
        _game = Game.Create(2, 1, 2);

        _game.CourtNumber = 1;
        _game.IsTeamA_Starting = true;
        _game.TeamA = _team1;
        _game.TeamB = _team2;

    }

    [Test]
    public void TestPublicFunctions()
    {
        Assert.That(_game.GetStartingTeam(), Is.EqualTo(_team1));
        Assert.That(_game.GetStartingTeam(), Is.Not.EqualTo(_game.GetNotStartingTeam()));

        Assert.That(_game.GetNotStartingTeam(), Is.EqualTo(_team2));
        Assert.That(_game.GetNotStartingTeam(), Is.Not.EqualTo(_team1));

        Assert.That(_game.IsPauseGame(), Is.False);

        _game.CourtNumber = 0;
        Assert.That(_game.IsPauseGame(), Is.True);
        _game.CourtNumber = 3;
        Assert.That(_game.IsPauseGame(), Is.False);
        _game.TeamB = _game.TeamA;
        Assert.That(_game.IsPauseGame, Is.True);
    }

}