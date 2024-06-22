using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
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
        Is8TurnsGame = bewerb.Is8TurnsGame;
        StartingTeamChange = bewerb.StartingTeamChange;
        SpielGruppe = bewerb.SpielGruppe;
        IERVersion = bewerb.IERVersion;
        ID = bewerb.ID;
        Gruppenname = bewerb.Gruppenname;
        GameplanId = bewerb.GameplanId;
        IsSplitGruppe = bewerb.IsSplitGruppe;
        NumberOfTeamsWithNamedPlayerOnResult = bewerb.NumberOfTeamsWithNamedPlayerOnResult;
        AnzahlAufsteiger = bewerb.AnzahlAufsteiger;
        AnzahlAbsteiger = bewerb.AnzahlAbsteiger;
        Endtext = bewerb.Endtext;
        VorText = bewerb.VorText;
        TeamNameWithStartnumber = bewerb.TeamNameWithStartnumber;
        ImageTopLeftFilename = bewerb.ImageTopLeftFilename;
        ImageTopRightFilename = bewerb.ImageTopRightFilename;
        ImageHeaderFilename = bewerb.ImageHeaderFilename;
        RowSpace = bewerb.RowSpace;
        FontSize = bewerb.FontSize;
        FontSizeVorText = bewerb.FontSizeVorText;
        FontSizeEndText = bewerb.FontSizeEndText;
        PageBreakSplitGroup = bewerb.PageBreakSplitGroup;
    }


    internal void ToNormal(ITeamBewerb teamBewerb)
    {

        teamBewerb.NumberOfGameRounds = NumberOfGameRounds;
        teamBewerb.Is8TurnsGame = Is8TurnsGame;
        teamBewerb.StartingTeamChange = StartingTeamChange;
        teamBewerb.SpielGruppe = SpielGruppe;
        teamBewerb.IERVersion = IERVersion;
        teamBewerb.Gruppenname = Gruppenname;
        teamBewerb.IsSplitGruppe = IsSplitGruppe;
        teamBewerb.NumberOfTeamsWithNamedPlayerOnResult = NumberOfTeamsWithNamedPlayerOnResult;
        teamBewerb.AnzahlAufsteiger = AnzahlAufsteiger;
        teamBewerb.AnzahlAbsteiger = AnzahlAbsteiger;
        teamBewerb.Endtext = Endtext;
        teamBewerb.VorText = VorText;
        teamBewerb.PageBreakSplitGroup = PageBreakSplitGroup;
        teamBewerb.TeamNameWithStartnumber = TeamNameWithStartnumber;
        teamBewerb.ImageTopLeftFilename = ImageTopLeftFilename;
        teamBewerb.ImageTopRightFilename = ImageTopRightFilename;
        teamBewerb.ImageHeaderFilename = ImageHeaderFilename;
        if (RowSpace >= 0 && RowSpace <= 99)
            teamBewerb.RowSpace = RowSpace;
        if (FontSize >= 12 && FontSize <= 24)
            teamBewerb.FontSize = FontSize;
        if(FontSizeVorText >= 12 && FontSizeVorText <= 24)
            teamBewerb.FontSizeVorText = FontSizeVorText;
        if (FontSizeEndText >= 12 && FontSizeEndText <= 24)
            teamBewerb.FontSizeEndText = FontSizeEndText;

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

            game.SerialisableSpielstand.ToNormal(newGame.Spielstand);

            teamBewerb.Teams.First(t => t.StartNumber == game.StartnumberTeamA).AddGame(newGame);

            if (game.CourtNumber != 0) //bei einem Aussetzer, nicht doppelt zuweisen, da in TeamA und TeamB die gleiche Mannschaft steckt
                teamBewerb.Teams.First(t => t.StartNumber == game.StartnumberTeamB).AddGame(newGame);
        }

        teamBewerb.GameplanId = GameplanId;

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

    [XmlElement(ElementName = "Splitgruppe")]
    public bool IsSplitGruppe { get; set; }

    [XmlElement(ElementName = "IERVersion")]
    public IERVersion IERVersion { get; set; }

    [XmlElement(ElementName = "Spielplan")]
    public int GameplanId { get; set; }

    [XmlArray(ElementName = "Mannschaften")]
    public List<SerialisableTeam> SerialisableTeams { get; set; }

    [XmlArray(ElementName = "Spiele")]
    public List<SerialisableGame> SerialisableGames { get; set; }

    [XmlElement(ElementName = "Aufsteiger")]
    public int AnzahlAufsteiger { get; set; } = 0;

    [XmlElement(ElementName = "Absteiger")]
    public int AnzahlAbsteiger { get; set; } = 0;

    [XmlElement(ElementName = "Endtext")]
    public string Endtext { get; set; }

    [XmlElement(ElementName = "VorText")]
    public string VorText { get; set; }

    [XmlElement(ElementName = "TeamNameWithStarnumber")]
    public bool TeamNameWithStartnumber { get; set; }

    [XmlElement(ElementName = "ImageTopLeft")]
    public string ImageTopLeftFilename { get; set; }

    [XmlElement(ElementName = "ImageTopRight")]
    public string ImageTopRightFilename { get; set; }

    [XmlElement(ElementName = "ImageHeader")]
    public string ImageHeaderFilename { get; set; }

    [XmlElement(ElementName = "RowSpace")]
    public int RowSpace { get; set; }

    [XmlElement(ElementName = "FontSize")]
    public int FontSize { get; set; }

    [XmlElement(ElementName = "FontSizeVorText")]
    public int FontSizeVorText { get; set; }

    [XmlElement(ElementName = "FontSizeEndText")]
    public int FontSizeEndText { get; set; }

    [XmlElement(ElementName = "PageBreakSplitGroup")]
    public bool PageBreakSplitGroup { get; set; }

    [XmlElement(ElementName = "PlayerNames")]
    public int NumberOfTeamsWithNamedPlayerOnResult { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public IEnumerable<ITeam> Teams => throw new NotImplementedException();
    [XmlIgnore]
    public IEnumerable<IGame> Games => throw new NotImplementedException();
    [XmlIgnore]
    public int NumberOfCourts => throw new NotImplementedException();




#pragma warning disable 67
    public event EventHandler TeamsChanged;
    public event EventHandler GamesChanged;
    public event EventHandler Is8TurnsGameChanged;
#pragma warning restore 67

    #region Methods

    public void AddTeam(ITeam team) => throw new NotImplementedException();
    public void AddNewTeam() => throw new NotImplementedException();


    public void CreateGames() => throw new NotImplementedException();


    public IEnumerable<IGame> GetAllGames(bool withBreaks = true) => throw new NotImplementedException();


    public int GetCountOfGames() => throw new NotImplementedException();


    public int GetCountOfGamesPerCourt() => throw new NotImplementedException();


    public IOrderedEnumerable<ITeam> GetSplitTeamsRanked(bool groupOne, bool live) => throw new NotImplementedException();


    public IOrderedEnumerable<ITeam> GetTeamsRanked(bool live = false) => throw new NotImplementedException();

    public void RemoveTeam(ITeam team) => throw new NotImplementedException();

    public void RemoveAllTeams() => throw new NotImplementedException();

    public void SetBroadcastData(IBroadCastTelegram telegram) => throw new NotImplementedException();

    public void SetStockTVResult(IStockTVResult tVResult) => throw new NotImplementedException();

    public void Reset() { }

    public void AddVirtualTeams(int count) => throw new NotImplementedException();

    public void RemoveAllVirtualTeams() => throw new NotImplementedException();
    public bool IsEachGameDone(bool live) => throw new NotImplementedException();
    #endregion

    #endregion
}