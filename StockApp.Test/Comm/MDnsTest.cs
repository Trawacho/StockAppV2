using NUnit.Framework;
using StockApp.Comm.MDns;

namespace StockApp.Test.Comm;

/// <summary>
/// Requires a real StockTV device broadcasting on the LAN via mDNS.
/// Excluded from normal `dotnet test` runs; run explicitly by name when hardware is available.
/// </summary>
[Category("Manual")]
internal class MDnsTest
{
    MDnsService _service;
    bool _found;

    [SetUp]
    public void Setup() { }

    [Test]
    [Explicit("Requires a real StockTV device on the LAN broadcasting via mDNS.")]
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
        Assert.That((System.Func<bool>)(() => _found == true), constraint);
        _service.Dispose();

    }

}
