using NUnit.Framework;
using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Wettbewerb.Zielbewerb;

namespace StockApp.Test
{
    public class DisziplinTests
    {
        IDisziplin _disziplin;
        [SetUp]
        public void Setup()
        {
            _disziplin = Disziplin.Create(StockTVZielDisziplinName.Schiessen);
        }

        [Test]
        public void TestPublicFunctions()
        {
            Assert.That(_disziplin.Summe == 0, Is.True);
            _disziplin.Reset();
            _disziplin.Versuch1 = 2;
            _disziplin.Versuch2 = 5;
            _disziplin.Versuch3 = 0;
            //disziplin.AddVersuch(2);
            //disziplin.AddVersuch(5);
            //disziplin.AddVersuch(0);
            Assert.That(_disziplin.VersucheCount() == 3, Is.True);

            Assert.That(_disziplin.Summe == 7, Is.True);
        }
    }
}