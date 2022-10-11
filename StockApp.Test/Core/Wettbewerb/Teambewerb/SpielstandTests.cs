using NUnit.Framework;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Collections.Generic;
using System.Linq;

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

        [Test]
        public void TestCountOfWinnningTurns()
        {
            _spielstand = Spielstand.Create();
            var kehren = new List<IKehre>()
            {
                Kehre.Create(1,0,3),
                Kehre.Create(2,5,0)
            };
            _spielstand.SetLiveValues(kehren.OrderBy(k => k.KehrenNummer));
            Assert.That(_spielstand.GetCountOfWinningTurnsTeamA(true), Is.EqualTo(_spielstand.GetCountOfWinningTurnsTeamB(true)));

            kehren.Add(Kehre.Create(3, 0, 5));
            _spielstand.SetLiveValues(kehren.OrderBy(k => k.KehrenNummer));
            Assert.That(_spielstand.GetCountOfWinningTurnsTeamB(true), Is.GreaterThan(_spielstand.GetCountOfWinningTurnsTeamA(true)));

            kehren.Add(Kehre.Create(4, 0, 0));
            kehren.Add(Kehre.Create(5, 0, 5));
            kehren.Add(Kehre.Create(5, 3, 0));
            _spielstand.SetLiveValues(kehren.OrderBy(k => k.KehrenNummer));
            Assert.That(_spielstand.GetCountOfWinningTurnsTeamA(true), Is.EqualTo(2));
            Assert.That(_spielstand.GetCountOfWinningTurnsTeamB(true), Is.EqualTo(3));

            _spielstand.CopyLiveToMasterValues();
            Assert.That(_spielstand.GetCountOfWinningTurnsTeamA(false), Is.EqualTo(2));
            Assert.That(_spielstand.GetCountOfWinningTurnsTeamB(false), Is.EqualTo(3));
        }

        [Test]
        public void TestSetMasterValue()
        {
            _spielstand = Spielstand.Create();

            _spielstand.SetMasterKehre(Kehre.Create(1, 3, 0));
            Assert.That(_spielstand.IsSetByHand, Is.True);

            _spielstand.SetMasterKehre(Kehre.Create(2, 3, 0));
            _spielstand.SetMasterKehre(Kehre.Create(3, 0, 3));
            Assert.That(_spielstand.Kehren_Master.Count(), Is.EqualTo(3));

            _spielstand.SetMasterKehre(Kehre.Create(1, 0, 5));
            Assert.That(_spielstand.Kehren_Master.Count(), Is.EqualTo(3));
            Assert.That(_spielstand.Kehren_Master.First(k => k.KehrenNummer == 1).PunkteTeamA, Is.EqualTo(0));
            Assert.That(_spielstand.Kehren_Master.First(k => k.KehrenNummer == 1).PunkteTeamB, Is.EqualTo(5));


        }
    }
}