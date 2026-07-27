using NUnit.Framework;
using StockApp.Test.UI.TestDoubles;
using StockApp.UI.ViewModels;
using System.Linq;

namespace StockApp.Test.UI.ViewModels;

public class TeamBewerbContainerViewModelTest
{
    private FakeTurnierStore _store;
    private TeamBewerbContainerViewModel _sut;

    [SetUp]
    public void Setup()
    {
        _store = new FakeTurnierStore();
        _sut = new TeamBewerbContainerViewModel(_store);
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Dispose();
    }

    [Test]
    public void Constructor_WrapsExistingTeamBewerbeAndSelectsFirst()
    {
        Assert.That(_sut.TeamBewerbe, Has.Count.EqualTo(1));
        Assert.That(_sut.SelectedTeamBewerb, Is.Not.Null);
        Assert.That(_sut.SelectedTeamBewerb.ID, Is.EqualTo(_sut.TeamBewerbe.First().ID));
    }

    [Test]
    public void AddNewTeamBewerbCommand_CascadesThroughContainer_UntilFourGroupsReached()
    {
        var addCommand = _sut.AddNewTeamBewerbCommand;

        Assert.That(addCommand.CanExecute(null), Is.True);
        addCommand.Execute(null);
        Assert.That(_sut.TeamBewerbe, Has.Count.EqualTo(2));

        addCommand.Execute(null);
        addCommand.Execute(null);
        Assert.That(_sut.TeamBewerbe, Has.Count.EqualTo(4));

        Assert.That(addCommand.CanExecute(null), Is.False,
            "Es dürfen maximal 4 Gruppen existieren (TeamBewerbe.Count <= 3 vor dem Hinzufügen).");
    }

    [Test]
    public void RemoveSelectedTeamBewerbCommand_CannotExecute_WhileOnlyOneGroupExists()
    {
        var removeCommand = _sut.RemoveSelectedTeamBewerbCommand;

        Assert.That(removeCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void RemoveSelectedTeamBewerbCommand_CanExecute_ForNonActiveSelectedGroup()
    {
        _sut.AddNewTeamBewerbCommand.Execute(null);
        _sut.SelectedTeamBewerb = _sut.TeamBewerbe.Last();

        var removeCommand = _sut.RemoveSelectedTeamBewerbCommand;

        Assert.That(_sut.SelectedTeamBewerb.ID, Is.Not.EqualTo(_sut.ActiveTeamBewerb.ID));
        Assert.That(removeCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void Dispose_UnsubscribesFromTeamBewerbeChanged()
    {
        _sut.Dispose();

        Assert.That(_sut.TeamBewerbe, Is.Empty, "Dispose() muss die gewrappten TeamBewerbViewModels aufräumen.");

        _store.Turnier.ContainerTeamBewerbe.AddNew();

        Assert.That(_sut.TeamBewerbe, Is.Empty,
            "Nach Dispose() darf TeamBewerbeChanged die Collection nicht mehr befüllen (Memory-Leak-Regression).");
    }
}
