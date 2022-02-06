using NUnit.Framework;
using StockApp.Core.Turnier;
using StockApp.XML;

namespace StockApp.Test.XML;

public class LoadITurnierTest
{
    [SetUp]
    public void Setuo()
    {

    }

    [Test]
    public void TestLoadMethod()
    {
        ITurnier turnier = Turnier.Create();
        LoadingModule.Load(ref turnier, $@"C:\Temp\test.xml");
        _ = LoadingModule.Load($@"C:\Temp\test.xml");
    }
}