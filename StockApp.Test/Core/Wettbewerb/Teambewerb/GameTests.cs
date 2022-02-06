using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;

namespace StockApp.Test
{
    public class GameTests
    {
        private IGame _game;
        private ITeam _team1;
        private ITeam _team2;

        [SetUp]
        public void Setup()
        {
            _team1 = Team.Create("erstes Team", false);
            _team2 = Team.Create("zweites Team", false);

            _game = Game.Create(2, 1, 2);

            _game.CourtNumber = 1;
            _game.IsTeamA_Starting = true;
            _game.TeamA = _team1;
            _game.TeamB = _team2;

        }

        [Test]
        public void TestPublicFunctions()
        {
            Assert.IsTrue(_game.GetStartingTeam() == _team1);
            Assert.IsFalse(_game.GetStartingTeam() == _game.GetNotStartingTeam());


            //Assert.IsTrue(_game.GetOpponent(_team1) == _team2);
            //Assert.IsFalse(_game.GetOpponent(_team1) == _team1);


            Assert.IsTrue(_game.GetNotStartingTeam() == _team2);
            Assert.IsFalse(_game.GetNotStartingTeam() == _team1);

            Assert.IsFalse(_game.IsPauseGame());
            Assert.IsFalse(_game.IsPauseGame() == !_game.IsPauseGame());

            _team1.IsVirtual = true;
            Assert.IsTrue(_game.IsPauseGame());
        }
    }
}