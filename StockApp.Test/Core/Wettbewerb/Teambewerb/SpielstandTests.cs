using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Test
{
    public class SpielstandTests
    {
        ISpielstand _spielstand;

        [SetUp]
        public void Setup()
        {
            _spielstand = Spielstand.Create();
        }

        [Test]
        public void TestPublicFunctions()
        {
            Assert.IsTrue(_spielstand.GetSpielPunkteTeamA(false) == 0);
            Assert.IsTrue(_spielstand.GetSpielPunkteTeamB(false) == 0);

            Assert.IsTrue(_spielstand.GetSpielPunkteTeamA(true) == 0);
            Assert.IsTrue(_spielstand.GetSpielPunkteTeamB(true) == 0);



            Assert.IsTrue(_spielstand.GetStockPunkteTeamA(false) == 0);
            Assert.IsTrue(_spielstand.GetStockPunkteTeamB(false) == 0);

            Assert.IsTrue(_spielstand.GetStockPunkteTeamA(true) == 0);
            Assert.IsTrue(_spielstand.GetStockPunkteTeamB(true) == 0);



            Assert.IsFalse(_spielstand.IsSetByHand);

            _spielstand.SetLiveValues(3, 5);
            Assert.IsFalse(_spielstand.IsSetByHand);

            Assert.IsTrue(_spielstand.Punkte_Live_TeamA == 3);
            Assert.IsTrue(_spielstand.Punkte_Live_TeamB == 5);

            _spielstand.SetMasterTeamAValue(7);
            Assert.IsTrue(_spielstand.IsSetByHand);

            Assert.IsTrue(_spielstand.GetStockPunkteTeamA(false) == 7);
            Assert.IsTrue(_spielstand.GetStockPunkteTeamB(false) == 0);

            Assert.IsTrue(_spielstand.GetSpielPunkteTeamA(false) == 2);
            Assert.IsTrue(_spielstand.GetSpielPunkteTeamB(false) == 0);
        }
    }
}