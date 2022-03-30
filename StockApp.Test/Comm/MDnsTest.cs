using NUnit.Framework;
using StockApp.Comm.MDns;

namespace StockApp.Test.Comm;

internal class MDnsTest
{
    MDnsService _service;
    bool _found;

    [SetUp]
    public void Setup() { }

    [Test]
    public void MdnsTest()
    {
        _found = false;
        _service = new MDnsService();
        _service.StockTVDiscovered += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine(s.GetType());
            _found = true;
        };
        _service.Discover();

        var constraint = Is.True.After(delayInMilliseconds: 100000, pollingInterval: 100);
        Assert.That(() => _found == true, constraint);
        _service.Dispose();

    }

}
