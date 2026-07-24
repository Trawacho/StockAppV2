using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Test.UI.TestDoubles;
using StockApp.UI.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StockApp.Test.UI.ViewModels;

/// <summary>
/// Zeigt das Muster für ViewModel-Tests: Store-Grenze mit <see cref="FakeTurnierStore"/> ersetzen,
/// dahinter echte Core-Objekte (<see cref="TeamBewerb"/>) verwenden, damit Event-Kaskaden und
/// Dispose-Verhalten (siehe CLAUDE.md "Ereignis-Kaskaden") gegen echtes Verhalten geprüft werden.
/// </summary>
public class GamesViewModelTest
{
    private FakeTurnierStore _store;
    private ITeamBewerb _teamBewerb;
    private GamesViewModel _sut;

    [SetUp]
    public void Setup()
    {
        _store = new FakeTurnierStore();
        _teamBewerb = _store.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

        for (int i = 0; i < 4; i++)
            _teamBewerb.AddNewTeam();

        _sut = new GamesViewModel(_store);
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Dispose();
    }

    [Test]
    public void Constructor_LoadsGameplansMatchingCurrentTeamCount()
    {
        Assert.That(_sut.Gameplans, Is.Not.Empty);
        Assert.That(_sut.Gameplans.All(g => g.Teams == 4), Is.True);
    }

    [Test]
    public void CreateGamesCommand_CannotExecute_WhenNoGameplanSelected()
    {
        Assert.That(_sut.SelectedGameplanId, Is.EqualTo(0));
        Assert.That(_sut.CreateGamesCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void CreateGamesCommand_CanExecute_OnceGameplanSelected()
    {
        _sut.SelectedGameplanId = _sut.Gameplans.First().ID;

        Assert.That(_sut.CreateGamesCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void ContainerTeamBewerbChanged_CascadesToDependentProperties()
    {
        var raisedProperties = new List<string>();
        _sut.PropertyChanged += (s, e) => raisedProperties.Add(e.PropertyName);

        var newTeamBewerb = _store.Turnier.ContainerTeamBewerbe.AddNew();
        newTeamBewerb.AddNewTeam();
        newTeamBewerb.AddNewTeam();
        _store.Turnier.ContainerTeamBewerbe.SetCurrentTeamBewerb(newTeamBewerb);

        Assert.That(raisedProperties, Does.Contain(nameof(GamesViewModel.HasNoGames)));
        Assert.That(raisedProperties, Does.Contain(nameof(GamesViewModel.CountOfGames)));
    }

    [Test]
    public void Dispose_UnsubscribesFromContainerTeamBewerbChanged()
    {
        _sut.Dispose();

        var raisedProperties = new List<string>();
        _sut.PropertyChanged += (s, e) => raisedProperties.Add(e.PropertyName);

        var newTeamBewerb = _store.Turnier.ContainerTeamBewerbe.AddNew();
        _store.Turnier.ContainerTeamBewerbe.SetCurrentTeamBewerb(newTeamBewerb);

        Assert.That(raisedProperties, Is.Empty,
            "Nach Dispose() darf das ViewModel nicht mehr auf Store-Events reagieren (Memory-Leak-Regression, siehe CLAUDE.md 'Memory Leak: Events nicht unsubscrieben').");
    }
}
