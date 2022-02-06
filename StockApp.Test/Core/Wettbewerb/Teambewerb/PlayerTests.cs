using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Test
{
    public class PlayerTests
    {
        IPlayer _player1;
        IPlayer _player2;

        [SetUp]
        public void Setup()
        {
            _player1 = Player.Create();
            _player2 = Player.Create("Meier", "Josef");
        }

        [Test]
        public void TestPublicFunctions()
        {
            Assert.IsTrue(_player1.LicenseNumber == string.Empty);
            _player1.LicenseNumber = "321654";
            Assert.IsTrue(_player1.LicenseNumber == "321654");
            Assert.IsTrue(_player2.LastName == "Meier");
            Assert.IsTrue(_player2.FirstName == "Josef");
        }
    }
}