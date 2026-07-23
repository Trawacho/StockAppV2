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
        public void TestInitialState_AllPointsAreZeroAndNotSetByHand()
        {
            Assert.That(_spielstand.GetSpielPunkteTeamA(false), Is.EqualTo(0));
            Assert.That(_spielstand.GetSpielPunkteTeamB(false), Is.EqualTo(0));
            Assert.That(_spielstand.GetSpielPunkteTeamA(true), Is.EqualTo(0));
            Assert.That(_spielstand.GetSpielPunkteTeamB(true), Is.EqualTo(0));

            Assert.That(_spielstand.GetStockPunkteTeamA(false), Is.EqualTo(0));
            Assert.That(_spielstand.GetStockPunkteTeamB(false), Is.EqualTo(0));
            Assert.That(_spielstand.GetStockPunkteTeamA(true), Is.EqualTo(0));
            Assert.That(_spielstand.GetStockPunkteTeamB(true), Is.EqualTo(0));

            Assert.That(_spielstand.IsSetByHand, Is.False);
        }

        [Test]
        public void TestSetLiveValues_DoesNotSetIsSetByHand()
        {
            _spielstand.SetLiveValues(3, 5);

            Assert.That(_spielstand.IsSetByHand, Is.False);
            Assert.That(_spielstand.Punkte_Live_TeamA, Is.EqualTo(3));
            Assert.That(_spielstand.Punkte_Live_TeamB, Is.EqualTo(5));
        }

        [Test]
        public void TestSetMasterTeamAValue_SetsIsSetByHandAndUpdatesDerivedPoints()
        {
            _spielstand.SetMasterTeamAValue(7);

            Assert.That(_spielstand.IsSetByHand, Is.True);
            Assert.That(_spielstand.GetStockPunkteTeamA(false), Is.EqualTo(7));
            Assert.That(_spielstand.GetStockPunkteTeamB(false), Is.EqualTo(0));

            // TeamA hat mehr Stockpunkte als TeamB -> 2 Spielpunkte (Sieg)
            Assert.That(_spielstand.GetSpielPunkteTeamA(false), Is.EqualTo(2));
            Assert.That(_spielstand.GetSpielPunkteTeamB(false), Is.EqualTo(0));
        }

        [Test]
        public void TestCountOfWinningTurns()
        {
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
        public void TestSetMasterKehre_OverwritesExistingKehrenNummer_InsteadOfDuplicating()
        {
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
