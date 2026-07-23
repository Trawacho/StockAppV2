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
        public void TestInitialState_SummeIsZero()
        {
            Assert.That(_disziplin.Summe, Is.EqualTo(0));
        }

        [Test]
        public void TestVersucheCountAndSumme_ReflectSetVersuche()
        {
            _disziplin.Reset();
            _disziplin.Versuch1 = 2;
            _disziplin.Versuch2 = 5;
            _disziplin.Versuch3 = 0;

            Assert.That(_disziplin.VersucheCount(), Is.EqualTo(3));
            Assert.That(_disziplin.Summe, Is.EqualTo(7));
        }
    }
}
