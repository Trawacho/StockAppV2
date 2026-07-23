using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Linq;

namespace StockApp.Test.Core.Wettbewerb.Teambewerb;

/// <summary>
/// §610 der Spielordnung: Turnier darf abgebrochen werden, wenn MEHR ALS 50% der geplanten Spiele
/// gespielt wurden. Games werden hier bewusst manuell per <see cref="Game.Create"/> statt über die
/// GamePlanFactory erzeugt, damit die Gesamtzahl der Spiele und die Verteilung auf die Teams exakt
/// kontrollierbar ist (wichtig für die 50%-Grenzwertprüfung).
/// </summary>
public class Paragraph610EvaluatorTest
{
    private TeamBewerb _teamBewerb;
    private ITeam[] _teams;

    [SetUp]
    public void Setup()
    {
        _teamBewerb = TeamBewerb.Create(1);
        for (int i = 0; i < 4; i++)
            _teamBewerb.AddNewTeam();
        _teams = _teamBewerb.Teams.ToArray();
    }

    private static IGame AddGame(ITeam teamA, ITeam teamB, int gameNumberOverAll, int courtNumber = 1)
    {
        var game = Game.Create(teamA, teamB, courtNumber, gameNumberOverAll, roundOfGame: 1, gameNumberOverAll: gameNumberOverAll, isTeamA_Starting: true);
        teamA.AddGame(game);
        teamB.AddGame(game);
        return game;
    }

    private static void MarkDone(IGame game)
    {
        game.Spielstand.SetMasterTeamAValue(15);
        game.Spielstand.SetMasterTeamBValue(5);
    }

    [Test]
    public void IsApplicable_NoGames_ReturnsFalse()
    {
        Assert.That(Paragraph610Evaluator.IsApplicable(_teamBewerb, live: false), Is.False);
    }

    [Test]
    public void IsApplicable_ExactlyHalfGamesPlayed_ReturnsFalse()
    {
        // 4 Spiele gesamt, 2 davon fertig -> genau 50%, §610 verlangt MEHR als 50%
        var g1 = AddGame(_teams[0], _teams[1], 1);
        var g2 = AddGame(_teams[2], _teams[3], 2);
        AddGame(_teams[0], _teams[2], 3);
        AddGame(_teams[1], _teams[3], 4);

        MarkDone(g1);
        MarkDone(g2);

        Assert.That(Paragraph610Evaluator.IsApplicable(_teamBewerb, live: false), Is.False);
    }

    [Test]
    public void IsApplicable_MoreThanHalfGamesPlayed_ReturnsTrue()
    {
        // 4 Spiele gesamt, 3 davon fertig -> 75%
        var g1 = AddGame(_teams[0], _teams[1], 1);
        var g2 = AddGame(_teams[2], _teams[3], 2);
        var g3 = AddGame(_teams[0], _teams[2], 3);
        AddGame(_teams[1], _teams[3], 4);

        MarkDone(g1);
        MarkDone(g2);
        MarkDone(g3);

        Assert.That(Paragraph610Evaluator.IsApplicable(_teamBewerb, live: false), Is.True);
    }

    [Test]
    public void IsApplicable_AllGamesPlayed_ReturnsTrue()
    {
        var g1 = AddGame(_teams[0], _teams[1], 1);
        var g2 = AddGame(_teams[2], _teams[3], 2);
        var g3 = AddGame(_teams[0], _teams[2], 3);
        var g4 = AddGame(_teams[1], _teams[3], 4);

        foreach (var g in new[] { g1, g2, g3, g4 })
            MarkDone(g);

        Assert.That(Paragraph610Evaluator.IsApplicable(_teamBewerb, live: false), Is.True);
    }

    [Test]
    public void IsApplicable_PauseGamesAreExcludedFromTotal()
    {
        // Aussetzer (Bahn 0) zaehlt weder zur Gesamtzahl noch zu den gespielten Spielen
        var g1 = AddGame(_teams[0], _teams[1], 1);
        var pause = AddGame(_teams[2], _teams[3], 2, courtNumber: 0);
        MarkDone(g1);
        MarkDone(pause);

        // Ohne den Aussetzer ist g1 das einzige "echte" Spiel -> 1 von 1 gespielt -> anwendbar
        Assert.That(Paragraph610Evaluator.IsApplicable(_teamBewerb, live: false), Is.True);
    }

    [Test]
    public void IsApplicable_LiveFalse_IgnoresResultsThatAreOnlySetLive()
    {
        var g1 = AddGame(_teams[0], _teams[1], 1);

        // SetLiveValues setzt NUR den Live-Spielstand, nicht den Master-Spielstand
        g1.Spielstand.SetLiveValues(15, 5);

        Assert.That(Paragraph610Evaluator.IsApplicable(_teamBewerb, live: false), Is.False,
            "live=false darf ein rein live gesetztes Ergebnis nicht als 'gespielt' zaehlen");
        Assert.That(Paragraph610Evaluator.IsApplicable(_teamBewerb, live: true), Is.True,
            "live=true muss ein live gesetztes Ergebnis als 'gespielt' zaehlen");
    }

    [Test]
    public void GetAdjustedGames_AllTeamsPlayedSameNumberOfGames_KeepsAllOfThem()
    {
        var g1 = AddGame(_teams[0], _teams[1], 1);
        var g2 = AddGame(_teams[2], _teams[3], 2);
        MarkDone(g1);
        MarkDone(g2);

        var adjusted = Paragraph610Evaluator.GetAdjustedGames(_teamBewerb, live: false);

        Assert.That(adjusted[_teams[0]], Is.EquivalentTo(new[] { g1 }));
        Assert.That(adjusted[_teams[1]], Is.EquivalentTo(new[] { g1 }));
        Assert.That(adjusted[_teams[2]], Is.EquivalentTo(new[] { g2 }));
        Assert.That(adjusted[_teams[3]], Is.EquivalentTo(new[] { g2 }));
    }

    [Test]
    public void GetAdjustedGames_TeamWithMoreGamesThanMinimum_DropsTheLatestGame()
    {
        // Team0 hat 2 Spiele fertig, Team1 (der schwaechste gemeinsame Nenner) nur 1
        var g1 = AddGame(_teams[0], _teams[1], gameNumberOverAll: 1);
        var g2 = AddGame(_teams[0], _teams[2], gameNumberOverAll: 2);
        MarkDone(g1);
        MarkDone(g2);

        var adjusted = Paragraph610Evaluator.GetAdjustedGames(_teamBewerb, live: false);

        // Minimum ueber alle Teams ist 0 (Team3 hat gar nicht gespielt) -> ALLE Listen werden auf 0 gekuerzt
        Assert.That(adjusted[_teams[0]], Is.Empty);
        Assert.That(adjusted[_teams[1]], Is.Empty);
        Assert.That(adjusted[_teams[2]], Is.Empty);
        Assert.That(adjusted[_teams[3]], Is.Empty);
    }

    [Test]
    public void GetAdjustedGames_OneTeamAheadOfOthers_DropsOnlyThatTeamsExcessGame()
    {
        // Alle 4 Teams haben mindestens 1 Spiel; Team0 hat zusaetzlich ein zweites gespielt
        var g1 = AddGame(_teams[0], _teams[1], gameNumberOverAll: 1); // Team0 & Team1 je 1
        var g2 = AddGame(_teams[2], _teams[3], gameNumberOverAll: 2); // Team2 & Team3 je 1
        var g3 = AddGame(_teams[0], _teams[2], gameNumberOverAll: 3); // Team0 zweites Spiel

        foreach (var g in new[] { g1, g2, g3 })
            MarkDone(g);

        var adjusted = Paragraph610Evaluator.GetAdjustedGames(_teamBewerb, live: false);

        // Minimum = 1 -> Team0 behaelt nur sein FRUEHERES Spiel (g1), g3 wird als "letztes" Spiel verworfen
        Assert.That(adjusted[_teams[0]], Is.EquivalentTo(new[] { g1 }));
        Assert.That(adjusted[_teams[1]], Is.EquivalentTo(new[] { g1 }));
        Assert.That(adjusted[_teams[2]], Is.EquivalentTo(new[] { g2 }));
        Assert.That(adjusted[_teams[3]], Is.EquivalentTo(new[] { g2 }));
    }

    [Test]
    public void GetAdjustedGames_PauseGamesAreNeverIncluded()
    {
        var pause = AddGame(_teams[0], _teams[1], gameNumberOverAll: 1, courtNumber: 0);
        var real = AddGame(_teams[0], _teams[2], gameNumberOverAll: 2);
        MarkDone(pause);
        MarkDone(real);

        var adjusted = Paragraph610Evaluator.GetAdjustedGames(_teamBewerb, live: false);

        Assert.That(adjusted[_teams[0]], Does.Not.Contain(pause));
    }
}
