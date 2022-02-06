using NUnit.Framework;
using StockApp.Core.Wettbewerb.Zielbewerb;

namespace StockApp.Test
{
    public class DisziplinTests
    {
        IDisziplin _disziplin;
        [SetUp]
        public void Setup()
        {
            _disziplin = Disziplin.Create(Disziplinart.Schiessen);
        }

        [Test]
        public void TestPublicFunctions()
        {
            Assert.IsTrue(_disziplin.Summe == 0);
            _disziplin.Reset();
            _disziplin.Versuch1 = 2;
            _disziplin.Versuch2 = 5;
            _disziplin.Versuch3 = 0;
            //disziplin.AddVersuch(2);
            //disziplin.AddVersuch(5);
            //disziplin.AddVersuch(0);
            Assert.IsTrue(_disziplin.VersucheCount() == 3);

            Assert.IsTrue(_disziplin.Summe == 7);
        }
    }
}