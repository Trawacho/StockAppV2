using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Linq;

namespace StockApp.Test
{
    public class TeamTests
    {
        ITeam _team1;
        [SetUp]
        public void Setup()
        {
            _team1 = Team.Create("erstes Team", false);
            _team1.AddGame(Game.Create(1, 1, 1));
        }

        [Test]
        public void TestPublicFunctions()
        {
            Assert.IsTrue(_team1.Games.Count == 1);
            _team1.ClearGames();
            Assert.IsTrue(_team1.Games.Count == 0);

            _team1.AddPlayer();
            Assert.IsTrue(_team1.Players.Any());
            var player = _team1.Players.First();
            _team1.RemovePlayer(player);
            Assert.IsFalse(_team1.Players.Any());

        }
    }
}