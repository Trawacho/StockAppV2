using NUnit.Framework;
using StockApp.Test.UI.TestDoubles;
using StockApp.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Test.UI.ViewModels;

/// <summary>
/// Regression für einen echten (gefixten) Memory-Leak: <see cref="StockTVCollectionViewModel"/> hat
/// beim Neuaufbau seiner <see cref="StockTVCollectionViewModel.StockTvViewModels"/>-Collection früher
/// nur <c>.Clear()</c> statt <c>.DisposeAndClear()</c> aufgerufen - die alten <see cref="StockTVViewModel"/>
/// blieben dadurch auf ihrem <see cref="StockApp.Comm.NetMqStockTV.IStockTV"/> abonniert.
/// </summary>
public class StockTVCollectionViewModelTest
{
    private FakeStockTVService _service;
    private FakeStockTVCommandStore _commandStore;
    private FakeTurnierStore _turnierStore;

    [SetUp]
    public void Setup()
    {
        _service = new FakeStockTVService();
        _commandStore = new FakeStockTVCommandStore();
        _turnierStore = new FakeTurnierStore();
    }

    [Test]
    public void Constructor_CreatesOneViewModelPerStockTv()
    {
        _service.AddStockTv(new FakeStockTV());
        _service.AddStockTv(new FakeStockTV());

        using var sut = new StockTVCollectionViewModel(_service, _commandStore, _turnierStore);

        Assert.That(sut.StockTvViewModels, Has.Count.EqualTo(2));
    }

    [Test]
    public void RecreateViewsCommand_DisposesPreviousChildViewModels()
    {
        var tv = new FakeStockTV();
        _service.AddStockTv(tv);
        using var sut = new StockTVCollectionViewModel(_service, _commandStore, _turnierStore);

        var oldViewModel = sut.StockTvViewModels.Single();
        var raisedProperties = new List<string>();
        oldViewModel.PropertyChanged += (s, e) => raisedProperties.Add(e.PropertyName);

        sut.RecreateViewsCommand.Execute(null);

        tv.RaiseOnlineChanged(true);

        Assert.That(raisedProperties, Is.Empty,
            "Nach dem Neuaufbau darf das alte StockTVViewModel nicht mehr auf IStockTV-Events reagieren " +
            "(Memory-Leak-Regression: FillStockTvViewModelsCollection muss DisposeAndClear() statt Clear() nutzen).");
    }

    [Test]
    public void Dispose_UnsubscribesFromStockTVCollectionChanged()
    {
        var sut = new StockTVCollectionViewModel(_service, _commandStore, _turnierStore);

        sut.Dispose();

        // Der echte Event-Handler nutzt App.Current.Dispatcher.Invoke(...), was ohne laufende
        // WPF-Application (wie hier im Testprozess) eine NullReferenceException wirft. Bleibt das
        // ViewModel nach Dispose() fälschlich abonniert, würde dieser Aufruf hier crashen - läuft er
        // durch, ist die Abmeldung nachgewiesen.
        _service.AddStockTv(new FakeStockTV());
    }
}
