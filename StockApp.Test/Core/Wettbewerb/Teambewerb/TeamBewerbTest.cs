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

    [Test]
    public void TestGetHighestPlayedRound()
    {
        for (int t = 0; t < 5; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 5), _teamBewerb.Teams, rounds: 3);

        Assert.That(_teamBewerb.GetHighestPlayedRound(), Is.EqualTo(0));

        var round1Game = _teamBewerb.GetAllGames(false).First(g => g.RoundOfGame == 1);
        round1Game.Spielstand.SetMasterTeamAValue(15);
        round1Game.Spielstand.SetMasterTeamBValue(5);

        Assert.That(_teamBewerb.GetHighestPlayedRound(), Is.EqualTo(1));

        // Live-Ergebnis in Runde 2 muss ebenfalls erkannt werden (nicht nur Master)
        var round2Game = _teamBewerb.GetAllGames(false).First(g => g.RoundOfGame == 2);
        round2Game.Spielstand.SetLiveValues(10, 3);

        Assert.That(_teamBewerb.GetHighestPlayedRound(), Is.EqualTo(2));
    }

    [Test]
    public void TestNumberOfGameRoundsRejectsReducingBelowPlayedRound()
    {
        for (int t = 0; t < 5; t++)
            _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 5), _teamBewerb.Teams, rounds: 3);

        _teamBewerb.NumberOfGameRounds = 3;

        var round3Game = _teamBewerb.GetAllGames(false).First(g => g.RoundOfGame == 3);
        round3Game.Spielstand.SetMasterTeamAValue(15);
        round3Game.Spielstand.SetMasterTeamBValue(2);

        _teamBewerb.NumberOfGameRounds = 2; // muss abgelehnt werden, Runde 3 hat bereits ein Ergebnis

        Assert.That(_teamBewerb.NumberOfGameRounds, Is.EqualTo(3));

        _teamBewerb.NumberOfGameRounds = 4; // weiterhin >= höchste gespielte Runde -> erlaubt
        Assert.That(_teamBewerb.NumberOfGameRounds, Is.EqualTo(4));
    }
}
