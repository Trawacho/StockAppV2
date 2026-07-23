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
        public void TestCreate_Default_HasEmptyLicenseNumber()
        {
            Assert.That(_player1.LicenseNumber, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestLicenseNumber_CanBeSet()
        {
            _player1.LicenseNumber = "321654";
            Assert.That(_player1.LicenseNumber, Is.EqualTo("321654"));
        }

        [Test]
        public void TestCreate_WithNames_AssignsLastNameAndFirstNameInCorrectOrder()
        {
            Assert.That(_player2.LastName, Is.EqualTo("Meier"));
            Assert.That(_player2.FirstName, Is.EqualTo("Josef"));
        }
    }
}
