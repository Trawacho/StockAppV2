using NUnit.Framework;
using StockApp.Test.UI.TestDoubles;
using StockApp.UI.ViewModels;

namespace StockApp.Test.UI.ViewModels;

public class TurnierViewModelTest
{
    [Test]
    public void Dispose_DisposesChildViewModels()
    {
        var store = new FakeTurnierStore();
        var sut = new TurnierViewModel(store);

        sut.Dispose();

        // TurnierViewModel hatte bisher gar kein Dispose(bool)-Override und disponierte die vier
        // erzeugten Kind-ViewModels (EntryFeeViewModel, 3x ExecutiveViewModel) nie.
        Assert.That(sut.EntryFeeViewModel._disposed, Is.True);
        Assert.That(sut.SchiedsrichterViewModel._disposed, Is.True);
        Assert.That(sut.RechenbueroViewModel._disposed, Is.True);
        Assert.That(sut.WettbewerbsleiterViewModel._disposed, Is.True);
    }
}
