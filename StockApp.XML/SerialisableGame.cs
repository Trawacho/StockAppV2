using StockApp.Core.Wettbewerb.Teambewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Spiel")]
public class SerialisableGame : IGame
{
    public SerialisableGame() { }
    public SerialisableGame(IGame game)
    {
        RoundOfGame = game.RoundOfGame;
        CourtNumber = game.CourtNumber;
        GameNumber = game.GameNumber;
        GameNumberOverAll = game.GameNumberOverAll;
        IsTeamA_Starting = game.IsTeamA_Starting;
        StartnumberTeamA = game.TeamA.StartNumber;
        StartnumberTeamB = game.TeamB.StartNumber;
        SerialisableSpielstand = new SerialisableSpielstand(game.Spielstand);
    }

    public int RoundOfGame { get; set; }

    public int CourtNumber { get; set; }

    public int GameNumber { get; set; }

    public int GameNumberOverAll { get; set; }

    public bool IsTeamA_Starting { get; set; }

    [XmlElement(ElementName = "StartnummerA")]
    public int StartnumberTeamA { get; set; }

    [XmlElement(ElementName = "StartnummerB")]
    public int StartnumberTeamB { get; set; }


    [XmlElement(ElementName = "Spielstand")]
    public SerialisableSpielstand SerialisableSpielstand { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public ITeam TeamA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [XmlIgnore]
    public ITeam TeamB { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [XmlIgnore]
    public ISpielstand Spielstand => throw new NotImplementedException();
#pragma warning disable 67
    public event EventHandler SpielstandChanged;
#pragma warning restore 67

    public bool Equals(IGame other)
    {
        throw new NotImplementedException();
    }

    public ITeam GetNotStartingTeam()
    {
        throw new NotImplementedException();
    }

    public ITeam GetStartingTeam()
    {
        throw new NotImplementedException();
    }

    public bool IsPauseGame()
    {
        throw new NotImplementedException();
    }

    public string ToStringExtended()
    {
        throw new NotImplementedException();
    }

    public bool IsGameDone(bool live)
    {
        throw new NotImplementedException();
    }

    public ITeam GetOpponent(ITeam team)
    {
        throw new NotImplementedException();
    }
    #endregion
}
