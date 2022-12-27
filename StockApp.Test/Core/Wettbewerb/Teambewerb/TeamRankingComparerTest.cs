using NUnit.Framework;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Linq;

namespace StockApp.Test;

public class TeamRankingComparerTest
{
    ITeamBewerb _teamBewerb;

    [SetUp]
    public void Setup()
    {

        _teamBewerb = TeamBewerb.Create(1);
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();

        var allGameplans = GamePlanFactory.LoadAllGameplans();

        GamePlanFactory.MatchTeamAndGames(
            gameplan: allGameplans.First(p => p.Teams == _teamBewerb.Teams.Count()), 
            teams: _teamBewerb.Teams, 
            rounds: 2);

        var games = _teamBewerb.GetAllGames(false);

        /*
         * 
         *      Bahn 1      Bahn 2
         * R1 1-4  1:15    3-2  9:3   
         * R1 5-3  11:15   2-1  31:3
         * R1 4-2  15:5    1-5  6:15
         * R1 3-1  10:5    5-4  15:9
         * R1 2-5  7:11    4-3  15:5
         * 
         * R2 1-4  15:5    3-2  3:15
         * R2 5-3  5:21    2-1  5:25
         * R2 4-2  5:15    1-5  15:10
         * R2 3-1  6:5     5-4  6:21    
         * R2 2-5  10:9    4:3  6:18
         * 
         * 
         * --> Ergebnis: 
         * 1. Team3 P 12:4  D 22   P 87:65
         * 2. Team4 P 8:8   D 11   P 91:80
         * 3. Team2 P 8:8   D 11   P 91:80
         * 4. Team5 P 6:10  D -22  P 82:104
         * 5. Team1 P 6:10  D -22  P 75:97
         * 
         */

        #region Runde 1
        var g1b1 = games.First(t => t.GameNumberOverAll == 1 && t.CourtNumber == 1);
        g1b1.Spielstand.SetMasterTeamAValue(1);      //1
        g1b1.Spielstand.SetMasterTeamBValue(15);     //4

        var g1b2 = games.First(t => t.GameNumberOverAll == 1 && t.CourtNumber == 2);
        g1b2.Spielstand.SetMasterTeamAValue(3);     //2
        g1b2.Spielstand.SetMasterTeamBValue(9);     //3


        var g2b1 = games.First(t => t.GameNumberOverAll == 2 && t.CourtNumber == 1);
        g2b1.Spielstand.SetMasterTeamAValue(11);     //5
        g2b1.Spielstand.SetMasterTeamBValue(15);    //3

        var g2b2 = games.First(t => t.GameNumberOverAll == 2 && t.CourtNumber == 2);
        g2b2.Spielstand.SetMasterTeamAValue(3);    //1
        g2b2.Spielstand.SetMasterTeamBValue(31);     //2


        var g3b1 = games.First(t => t.GameNumberOverAll == 3 && t.CourtNumber == 1);
        g3b1.Spielstand.SetMasterTeamAValue(15);     //4
        g3b1.Spielstand.SetMasterTeamBValue(5);    //2

        var g3b2 = games.First(t => t.GameNumberOverAll == 3 && t.CourtNumber == 2);
        g3b2.Spielstand.SetMasterTeamAValue(15);     //5
        g3b2.Spielstand.SetMasterTeamBValue(6);    //1


        var g4b1 = games.First(t => t.GameNumberOverAll == 4 && t.CourtNumber == 1);
        g4b1.Spielstand.SetMasterTeamAValue(10);    //3
        g4b1.Spielstand.SetMasterTeamBValue(5);     //1

        var g4b2 = games.First(t => t.GameNumberOverAll == 4 && t.CourtNumber == 2);
        g4b2.Spielstand.SetMasterTeamAValue(9);     //4
        g4b2.Spielstand.SetMasterTeamBValue(15);    //5


        var g5b1 = games.First(t => t.GameNumberOverAll == 5 && t.CourtNumber == 1);
        g5b1.Spielstand.SetMasterTeamAValue(7);    //2
        g5b1.Spielstand.SetMasterTeamBValue(11);     //5

        var g5b2 = games.First(t => t.GameNumberOverAll == 5 && t.CourtNumber == 2);
        g5b2.Spielstand.SetMasterTeamAValue(5);    //3
        g5b2.Spielstand.SetMasterTeamBValue(15);    //4

        #endregion


        #region Runde 2

        var g6b1 = games.First(t => t.GameNumberOverAll == 6 && t.CourtNumber == 1);
        g6b1.Spielstand.SetMasterTeamAValue(15);    //1
        g6b1.Spielstand.SetMasterTeamBValue(5);     //4

        var g6b2 = games.First(t => t.GameNumberOverAll == 6 && t.CourtNumber == 2);
        g6b2.Spielstand.SetMasterTeamAValue(15);    //2
        g6b2.Spielstand.SetMasterTeamBValue(3);     //3


        var g7b1 = games.First(t => t.GameNumberOverAll == 7 && t.CourtNumber == 1);
        g7b1.Spielstand.SetMasterTeamAValue(5);     //5
        g7b1.Spielstand.SetMasterTeamBValue(21);    //3

        var g7b2 = games.First(t => t.GameNumberOverAll == 7 && t.CourtNumber == 2);
        g7b2.Spielstand.SetMasterTeamAValue(25);    //1
        g7b2.Spielstand.SetMasterTeamBValue(5);     //2


        var g8b1 = games.First(t => t.GameNumberOverAll == 8 && t.CourtNumber == 1);
        g8b1.Spielstand.SetMasterTeamAValue(5);     //4
        g8b1.Spielstand.SetMasterTeamBValue(15);    //2

        var g8b2 = games.First(t => t.GameNumberOverAll == 8 && t.CourtNumber == 2);
        g8b2.Spielstand.SetMasterTeamAValue(10);     //5
        g8b2.Spielstand.SetMasterTeamBValue(15);    //1


        var g9b1 = games.First(t => t.GameNumberOverAll == 9 && t.CourtNumber == 1);
        g9b1.Spielstand.SetMasterTeamAValue(6);    //3
        g9b1.Spielstand.SetMasterTeamBValue(5);     //1

        var g9b2 = games.First(t => t.GameNumberOverAll == 9 && t.CourtNumber == 2);
        g9b2.Spielstand.SetMasterTeamAValue(21);     //4
        g9b2.Spielstand.SetMasterTeamBValue(6);    //5


        var g10b1 = games.First(t => t.GameNumberOverAll == 10 && t.CourtNumber == 1);
        g10b1.Spielstand.SetMasterTeamAValue(10);    //2
        g10b1.Spielstand.SetMasterTeamBValue(9);     //5

        var g10b2 = games.First(t => t.GameNumberOverAll == 10 && t.CourtNumber == 2);
        g10b2.Spielstand.SetMasterTeamAValue(18);    //3
        g10b2.Spielstand.SetMasterTeamBValue(6);    //4

        #endregion
    }


    [Test]
    public void TestCompareLastGame()
    {
        var comparer = new TeamRankingComparer(false, IERVersion.v2022);

        var teamListe = _teamBewerb.Teams;

        Assert.That(TeamRankingComparer.CompareLastGame(teamListe.First(t => t.StartNumber == 2)
                                                            .Games.Where(g => g.RoundOfGame == 1), 2, 4, false), Is.EqualTo(1));
        Assert.That(TeamRankingComparer.CompareLastGame(teamListe.First(t => t.StartNumber == 2)
                                                            .Games.Where(g => g.RoundOfGame == 2), 2, 4, false), Is.EqualTo(-1));
        Assert.That(TeamRankingComparer.CompareLastGame(teamListe.First(t => t.StartNumber == 2)
                                                            .Games, 2, 4, false), Is.EqualTo(-1));


    }

    [Test]
    public void TestComparer()
    {
        /*
        *  * --> Ergebnis: 
        * 1. Team3 P 12:4  D 22   P 87:65
        * 2. Team4 P 8:8   D 11   P 91:80
        * 3. Team2 P 8:8   D 11   P 91:80
        * 4. Team5 P 6:10  D -22  P 82:104 -> 0,788
        * 5. Team1 P 6:10  D -22  P 75:97  -> 0,773
        */
        var comparer = new TeamRankingComparer(false, IERVersion.v2022);
        var teamListe = _teamBewerb.Teams
                            .OrderBy(t => t, comparer)
                            .ToList();

        Assert.Multiple(() =>
        {
            Assert.That(teamListe[0].GetSpielPunkte() == (12, 4), Is.True);
            Assert.That(teamListe[0].GetStockPunkte() == (87, 65), Is.True);
            Assert.That(teamListe[0].GetStockPunkteDifferenz() == 22, Is.True);

            Assert.That(teamListe[1].GetSpielPunkte() == (8, 8), Is.True);
            Assert.That(teamListe[1].GetStockPunkte() == (91, 80), Is.True);
            Assert.That(teamListe[1].GetStockPunkteDifferenz() == 11, Is.True);

            Assert.That(teamListe[2].GetSpielPunkte() == (8, 8), Is.True);
            Assert.That(teamListe[2].GetStockPunkte() == (91, 80), Is.True);
            Assert.That(teamListe[2].GetStockPunkteDifferenz() == 11, Is.True);

            Assert.That(teamListe[3].GetSpielPunkte() == (6, 10), Is.True);
            Assert.That(teamListe[3].GetStockPunkte() == (82, 104), Is.True);
            Assert.That(teamListe[3].GetStockPunkteDifferenz() == -22, Is.True);

            Assert.That(teamListe[4].GetSpielPunkte() == (6, 10), Is.True);
            Assert.That(teamListe[4].GetStockPunkte() == (75, 97), Is.True);
            Assert.That(teamListe[4].GetStockPunkteDifferenz() == -22, Is.True);

            Assert.That(teamListe[0].StartNumber == 3, Is.True);
            Assert.That(teamListe[1].StartNumber == 4 || teamListe[1].StartNumber == 2, Is.True);
            Assert.That(teamListe[3].StartNumber == 5, Is.True);
            Assert.That(teamListe[4].StartNumber == 1, Is.True);

            Assert.That(comparer.Compare(teamListe[0], teamListe[1]), Is.EqualTo(-1));
            Assert.That(comparer.Compare(teamListe[4], teamListe[1]), Is.EqualTo(1));
        });
    }

}
