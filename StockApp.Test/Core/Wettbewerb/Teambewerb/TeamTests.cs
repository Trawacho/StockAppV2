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

    [Test]
    public void TestVorergebnisOnlyWithoutGames()
    {
        var team = Team.Create("Nur Vorergebnis");
        team.VorergebnisSpielpunktePlus = 12;
        team.VorergebnisSpielpunkteMinus = 4;
        team.VorergebnisStockpunktePlus = 87;
        team.VorergebnisStockpunkteMinus = 65;

        Assert.That(team.GetSpielPunkte(), Is.EqualTo((12, 4)));
        Assert.That(team.GetStockPunkte(), Is.EqualTo((87, 65)));
    }

    [Test]
    public void TestVorergebnisCombinedWithGameAndStrafpunkte()
    {
        var teamA = Team.Create("Team A");
        teamA.StartNumber = 1;
        var teamB = Team.Create("Team B");
        teamB.StartNumber = 2;

        var game = Game.Create(teamA, teamB, courtNumber: 1, gameNumber: 1, roundOfGame: 1, gameNumberOverAll: 1, isTeamA_Starting: true);
        game.Spielstand.SetMasterTeamAValue(15);
        game.Spielstand.SetMasterTeamBValue(5);
        teamA.AddGame(game);
        teamB.AddGame(game);

        teamA.VorergebnisSpielpunktePlus = 10;
        teamA.VorergebnisSpielpunkteMinus = 2;
        teamA.VorergebnisStockpunktePlus = 50;
        teamA.VorergebnisStockpunkteMinus = 20;
        teamA.StrafSpielpunkte = 1;

        // Aus dem Spiel: TeamA gewinnt -> 2 Spielpunkte, 0 Gegenpunkte, 15:5 Stockpunkte
        // + Vorergebnis (10:2 Spielpunkte, 50:20 Stockpunkte) - Strafpunkte (1) auf die Spielpunkte
        Assert.That(teamA.GetSpielPunkte(), Is.EqualTo((2 + 10 - 1, 0 + 2)));
        Assert.That(teamA.GetStockPunkte(), Is.EqualTo((15 + 50, 5 + 20)));
    }

}
