using NUnit.Framework;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb;
using StockApp.Core.Wettbewerb.Teambewerb;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace StockApp.Test;

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

public class TeamComparerTest
{
    ITeamBewerb _teamBewerb;
    [SetUp]
    public void Setup()
    {
        _teamBewerb = TeamBewerb.Create();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddNewTeam();
        _teamBewerb.AddVirtualTeams(1);

        

        var facGames = GameFactory.CreateGames(5, 1, 1, false);
        GameFactoryWrapper.MatchTeamAndGames(facGames , _teamBewerb.Teams);

        var games = _teamBewerb.GetAllGames(false);

        var g1b1 = games.First(t => t.GameNumber == 1 && t.CourtNumber == 1);
        g1b1.Spielstand.SetMasterTeamAValue(25);    //1
        g1b1.Spielstand.SetMasterTeamBValue(0);     //4

        var g1b2 = games.First(t => t.GameNumber == 1 && t.CourtNumber == 2);
        g1b2.Spielstand.SetMasterTeamAValue(14);    //2
        g1b2.Spielstand.SetMasterTeamBValue(9);     //3


        var g2b1 = games.First(t => t.GameNumber == 2 && t.CourtNumber == 1);
        g2b1.Spielstand.SetMasterTeamAValue(5);     //5
        g2b1.Spielstand.SetMasterTeamBValue(12);    //3

        var g2b2 = games.First(t => t.GameNumber == 2 && t.CourtNumber == 2);
        g2b2.Spielstand.SetMasterTeamAValue(13);    //1
        g2b2.Spielstand.SetMasterTeamBValue(1);     //2


        var g3b1 = games.First(t => t.GameNumber == 3 && t.CourtNumber == 1);
        g3b1.Spielstand.SetMasterTeamAValue(6);     //4
        g3b1.Spielstand.SetMasterTeamBValue(15);    //2

        var g3b2 = games.First(t => t.GameNumber == 3 && t.CourtNumber == 2);
        g3b2.Spielstand.SetMasterTeamAValue(7);     //5
        g3b2.Spielstand.SetMasterTeamBValue(11);    //1


        var g4b1 = games.First(t => t.GameNumber == 4 && t.CourtNumber == 1);
        g4b1.Spielstand.SetMasterTeamAValue(15);    //3
        g4b1.Spielstand.SetMasterTeamBValue(3);     //1

        var g4b2 = games.First(t => t.GameNumber == 4 && t.CourtNumber == 2);
        g4b2.Spielstand.SetMasterTeamAValue(9);     //4
        g4b2.Spielstand.SetMasterTeamBValue(31);    //5


        var g5b1 = games.First(t => t.GameNumber == 5 && t.CourtNumber == 1);
        g5b1.Spielstand.SetMasterTeamAValue(12);    //2
        g5b1.Spielstand.SetMasterTeamBValue(5);     //5

        var g5b2 = games.First(t => t.GameNumber == 5 && t.CourtNumber == 2);
        g5b2.Spielstand.SetMasterTeamAValue(11);    //3
        g5b2.Spielstand.SetMasterTeamBValue(13);    //4
    }

    


    [Test]
    public void TestComparer()
    {
        var comparer = new TeamRankingComparer(false, IERVersion.v2022);
        var teamListe = _teamBewerb.Teams.Where(t =>!t.IsVirtual).ToList();
        teamListe.Sort(comparer);
    }
    
}
