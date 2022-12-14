using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Teambewerb")]
public class SerialisableTeamBewerb : ITeamBewerb
{
    public SerialisableTeamBewerb() 
    {
        ID = 0;
        Gruppenname = "default 1";
    }

    public SerialisableTeamBewerb(ITeamBewerb bewerb) : this()
    {
        SerialisableTeams = new List<SerialisableTeam>();
        foreach (var team in bewerb.Teams)
        {
            SerialisableTeams.Add(new SerialisableTeam(team));
        }

        SerialisableGames = new List<SerialisableGame>();
        foreach (var game in bewerb.GetAllGames())
        {
            SerialisableGames.Add(new SerialisableGame(game));
        }

        NumberOfGameRounds = bewerb.NumberOfGameRounds;
        BreaksCount = bewerb.BreaksCount;
        Is8TurnsGame = bewerb.Is8TurnsGame;
        StartingTeamChange = bewerb.StartingTeamChange;
        SpielGruppe = bewerb.SpielGruppe;
        IERVersion = bewerb.IERVersion;
        ID = bewerb.ID;
        Gruppenname = bewerb.Gruppenname;

        GamePlan = new List<IFactoryGame>();
        foreach (var game in bewerb.Games)
        {
            GamePlan.Add(new FactoryGame(game.RoundOfGame,
                                         game.GameNumberOverAll,
                                         game.GameNumber,
                                         game.CourtNumber,
                                         game.TeamA.StartNumber,
                                         game.TeamB.StartNumber,
                                         game.IsTeamA_Starting,
                                         game.IsPauseGame(),
                                         bewerb.StartingTeamChange));
        }
    }

    private List<IFactoryGame> GamePlan { get; set; }



    internal void ToNormal(ITeamBewerb teamBewerb)
    {

        teamBewerb.NumberOfGameRounds = NumberOfGameRounds;
        teamBewerb.Is8TurnsGame = Is8TurnsGame;
        teamBewerb.StartingTeamChange = StartingTeamChange;
        teamBewerb.SpielGruppe = SpielGruppe;
        teamBewerb.BreaksCount = BreaksCount;
        teamBewerb.IERVersion = IERVersion;
        teamBewerb.Gruppenname = Gruppenname;
        

        //alle Teams löschen
        teamBewerb.RemoveAllTeams();

        // Alle Teams erzeugen
        for (int i = 0; i < SerialisableTeams.Count; i++)
        {
            teamBewerb.AddNewTeam();
        }
        // den erzeugten Teams die Eigenschaften zuweisen
        foreach (var team in SerialisableTeams)
        {
            var normalTeam = teamBewerb.Teams.First(t => t.StartNumber == team.StartNumber);
            team.ToNormal(normalTeam);
        }

        // jedem Spiel die Mannschaften zuweisen und dann jeder Mannschaft die Spiele zuweisen.
        foreach (var game in SerialisableGames)
        {
            var teamA = teamBewerb.Teams.First(t => t.StartNumber == game.StartnumberTeamA);
            var teamB = teamBewerb.Teams.First(t => t.StartNumber == game.StartnumberTeamB);
            var newGame = Game.Create(teamA, teamB, game.GameNumber, game.RoundOfGame, game.GameNumberOverAll);
            newGame.CourtNumber = game.CourtNumber;
            newGame.IsTeamA_Starting = game.IsTeamA_Starting;
            newGame.Spielstand.SetMasterTeamAValue(game.SerialisableSpielstand.A);
            newGame.Spielstand.SetMasterTeamBValue(game.SerialisableSpielstand.B);
            teamBewerb.Teams.First(t => t.StartNumber == game.StartnumberTeamA).AddGame(newGame);
            teamBewerb.Teams.First(t => t.StartNumber == game.StartnumberTeamB).AddGame(newGame);
        }

        //Wenn alle Spielstände 0:0 sind, dann jeden Spielstand Resetten, damit IsSetByHand auf false steht
        if (!teamBewerb.Teams.Any(t => t.GetStockPunkteDifferenz() != 0))
        {
            foreach (var game in teamBewerb.Games)
            {
                game.Spielstand.Reset(true);
            }
        }


    }

    [XmlElement(ElementName = "ID")]
    public int ID { get; set; }

    [XmlElement(ElementName = "SpielRunden")]
    public int NumberOfGameRounds { get; set; }

    [XmlElement(ElementName = "AnzahlAussetzer")]
    public int BreaksCount { get; set; }

    [XmlElement(ElementName = "AchtKehren")]
    public bool Is8TurnsGame { get; set; }

    [XmlElement(ElementName = "AnspielWechsel")]
    public bool StartingTeamChange { get; set; }

    [XmlElement(ElementName = "SpielGruppe")]
    public int SpielGruppe { get; set; }

    [XmlElement(ElementName = "Gruppenname")]
    public string Gruppenname { get; set; }

    [XmlElement(ElementName = "IERVersion")]
    public IERVersion IERVersion { get; set; }

    [XmlArray(ElementName = "Mannschaften")]
    public List<SerialisableTeam> SerialisableTeams { get; set; }

    [XmlArray(ElementName = "Spiele")]
    public List<SerialisableGame> SerialisableGames { get; set; }



    #region XMLIgnore

    [XmlIgnore]
    public IEnumerable<ITeam> Teams => throw new NotImplementedException();
    [XmlIgnore]
    public IEnumerable<IGame> Games => throw new NotImplementedException();
    [XmlIgnore]
    public int NumberOfCourts => throw new NotImplementedException();
    [XmlIgnore]
    public int NumberOfTeamsWithNamedPlayerOnResult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


#pragma warning disable 67
    public event EventHandler TeamsChanged;
    public event EventHandler GamesChanged;
    public event EventHandler Is8TurnsGameChanged;
#pragma warning restore 67

    #region Methods

    public void AddTeam(ITeam team) => throw new NotImplementedException();
    public void AddNewTeam()
    {
        throw new NotImplementedException();
    }

    public void CreateGames()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IGame> GetAllGames(bool withBreaks = true)
    {
        throw new NotImplementedException();
    }

    public int GetCountOfGames()
    {
        throw new NotImplementedException();
    }

    public int GetCountOfGamesPerCourt()
    {
        throw new NotImplementedException();
    }

    public IOrderedEnumerable<ITeam> GetTeamsRanked(bool live = false)
    {
        throw new NotImplementedException();
    }

    public void RemoveTeam(ITeam team)
    {
        throw new NotImplementedException();
    }

    public void RemoveAllTeams()
    {
        throw new NotImplementedException();
    }

    public void SetBroadcastData(IBroadCastTelegram telegram)
    {
        throw new NotImplementedException();
    }

    public void SetStockTVResult(IStockTVResult tVResult)
    {
        throw new NotImplementedException();
    }

    public void Reset() { }

    public void AddVirtualTeams(int count)
    {
        throw new NotImplementedException();
    }

    public void RemoveAllVirtualTeams()
    {
        throw new NotImplementedException();
    }
    #endregion

    #endregion
}