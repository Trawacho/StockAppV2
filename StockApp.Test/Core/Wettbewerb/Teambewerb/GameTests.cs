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
    public void TestGetStartingTeam_WhenTeamAIsStarting_ReturnsTeamA()
    {
        Assert.That(_game.GetStartingTeam(), Is.EqualTo(_team1));
        Assert.That(_game.GetStartingTeam(), Is.Not.EqualTo(_game.GetNotStartingTeam()));
    }

    [Test]
    public void TestGetNotStartingTeam_WhenTeamAIsStarting_ReturnsTeamB()
    {
        Assert.That(_game.GetNotStartingTeam(), Is.EqualTo(_team2));
        Assert.That(_game.GetNotStartingTeam(), Is.Not.EqualTo(_team1));
    }

    [Test]
    public void TestIsPauseGame_NormalGame_ReturnsFalse()
    {
        Assert.That(_game.IsPauseGame(), Is.False);
    }

    [Test]
    public void TestIsPauseGame_CourtNumberZero_ReturnsTrue()
    {
        _game.CourtNumber = 0;
        Assert.That(_game.IsPauseGame(), Is.True);
    }

    [Test]
    public void TestIsPauseGame_NonZeroCourtNumber_ReturnsFalse()
    {
        _game.CourtNumber = 3;
        Assert.That(_game.IsPauseGame(), Is.False);
    }

    [Test]
    public void TestIsPauseGame_TeamAEqualsTeamB_ReturnsTrue()
    {
        _game.TeamB = _game.TeamA;
        Assert.That(_game.IsPauseGame(), Is.True);
    }
}
