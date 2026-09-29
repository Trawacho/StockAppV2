using NUnit.Framework;
using StockApp.Comm.NetMqStockTV;
using System.Linq;

namespace StockApp.Test.Comm;

public class StockTVBegegnungTest
{
    [TestCase(true, true, "3:A »:B;")]
    [TestCase(false, true, "3:A:« B;")]
    [TestCase(true, false, "3:« A:B;")]
    [TestCase(false, false, "3:A:B »;")]
    public void GetStockTVString_PlacesAnspielMarker(bool anspielA, bool nextCourtLeft, string expected)
    {
        var sut = new StockTVBegegnung(3, "A", "B", anspielA);

        Assert.That(sut.GetStockTVString(nextCourtLeft), Is.EqualTo(expected));
    }

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void GetStockTVString_NamesWithSeparators_KeepFormatIntact(bool anspielA, bool nextCourtLeft)
    {
        var sut = new StockTVBegegnung(7, "SV Ebensee: Damen", "ESC A; B\r\nC", anspielA);

        var result = sut.GetStockTVString(nextCourtLeft);

        Assert.That(result.Count(c => c == ':'), Is.EqualTo(2));
        Assert.That(result.Count(c => c == ';'), Is.EqualTo(1));
        Assert.That(result, Does.EndWith(";"));
        Assert.That(result, Does.StartWith("7:"));
        Assert.That(result, Does.Contain("SV Ebensee Damen"));
        Assert.That(result, Does.Contain("ESC A B C"));
    }

    [Test]
    public void GetStockTVString_NullNames_AreSentAsEmpty()
    {
        var sut = new StockTVBegegnung(1, null, null, true);

        Assert.That(sut.GetStockTVString(false), Is.EqualTo("1:« :;"));
    }

    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("  a   b  ", "a b")]
    [TestCase("a:b;c", "a b c")]
    [TestCase("a\tb\nc", "a b c")]
    public void SanitizeName_ReplacesSeparatorsAndControlChars(string input, string expected)
    {
        Assert.That(StockTVBegegnung.SanitizeName(input), Is.EqualTo(expected));
    }
}
