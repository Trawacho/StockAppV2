using NUnit.Framework;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Test.UI.ViewModels;

public class LiveResultsTeamViewModelTest
{
    private ITeamBewerb _teamBewerb;

    [SetUp]
    public void Setup()
    {
        _teamBewerb = TeamBewerb.Create(1);
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();

        GamePlanFactory.MatchTeamAndGames(
            GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 2), _teamBewerb.Teams);
    }

    [Test]
    public void RankedTeamList_ReflectsCurrentTeamCount()
    {
        using var sut = new LiveResultsTeamViewModel(_teamBewerb);

        Assert.That(sut.RankedTeamList, Has.Count.EqualTo(2));
    }

    [Test]
    public void ResultChangeOnSubscribedGame_RaisesRankedTeamListChanged()
    {
        using var sut = new LiveResultsTeamViewModel(_teamBewerb);
        var raisedProperties = new List<string>();
        sut.PropertyChanged += (s, e) => raisedProperties.Add(e.PropertyName);

        var game = _teamBewerb.GetAllGames().First();
        game.Spielstand.SetLiveValues(10, 3);

        Assert.That(raisedProperties, Does.Contain(nameof(LiveResultsTeamViewModel.RankedTeamList)));
    }

    [Test]
    public void GamesChangedOnTeamBewerb_AutoDisposesAndRequestsClose()
    {
        var sut = new LiveResultsTeamViewModel(_teamBewerb);
        bool closeRequested = false;
        sut.DialogCloseRequested += (s, e) => closeRequested = true;

        // Ein neu erstellter Spielplan (z.B. weil der Nutzer die Runden ändert) macht die Live-Ansicht
        // ungültig - das ViewModel muss sich selbst schließen (siehe LiveResultsTeamViewModel.TeamBewerb_GamesChanged).
        _teamBewerb.Teams.First().ClearGames();

        Assert.That(closeRequested, Is.True);
    }

    [Test]
    public void Dispose_WithoutAnyGames_DoesNotThrow()
    {
        // Regression: früher wurde beim Dispose() nur unsubscribed, wenn eine (zur Konstruktionszeit
        // ermittelte) Spieleliste vorhanden war - bei einem TeamBewerb ohne Spiele konnte das
        // Cleanup dadurch übersprungen werden. _subscribedGames wird jetzt immer beim Konstruieren
        // befüllt (ggf. leer), Dispose muss also auch ohne Spiele sicher funktionieren.
        var emptyTeamBewerb = TeamBewerb.Create(2);

        var sut = new LiveResultsTeamViewModel(emptyTeamBewerb);

        sut.Dispose();
    }
}
