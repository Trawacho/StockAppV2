using StockApp.Core.Wettbewerb.Teambewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Mannschaft")]
public class SerialisableTeam : ITeam
{
    public SerialisableTeam() { }

    public SerialisableTeam(ITeam team)
    {
        IsVirtual = team.IsVirtual;
        StartNumber = team.StartNumber;
        TeamName = team.TeamName;
        Nation = team.Nation;
        SerialisablePlayers = new List<SerialisablePlayer>();
        foreach (var player in team.Players)
        {
            SerialisablePlayers.Add(new SerialisablePlayer(player));
        }
    }
    internal void ToNormal(ITeam normal)
    {
        normal.Nation = Nation;
        normal.TeamName = TeamName;
        normal.IsVirtual = IsVirtual;
        foreach (var player in SerialisablePlayers)
        {
            normal.AddPlayer(player);
        }
    }



    public bool IsVirtual { get; set; }
    public int StartNumber { get; set; }
    public string TeamName { get; set; }

    [XmlIgnore]
    public string TeamNameShort { get; }
    public string Nation { get; set; }


    [XmlArray(ElementName = "Spieler")]
    public List<SerialisablePlayer> SerialisablePlayers { get; set; }




    #region XMLIgnore

    [XmlIgnore]
    public IEnumerable<IPlayer> Players => throw new NotImplementedException();

    [XmlIgnore]
    public IReadOnlyCollection<IGame> Games => throw new NotImplementedException();

    public IEnumerable<int> SpieleMitAnspiel() => throw new NotImplementedException();

    public IEnumerable<int> SpieleAufStartSeite() => throw new NotImplementedException();

#pragma warning disable 67
    public event EventHandler GamesChanged;
    public event EventHandler PlayersChanged;
#pragma warning restore 67

    public void AddGame(IGame game) => throw new NotImplementedException();

    public void AddPlayer() => throw new NotImplementedException();
    public void AddPlayer(IPlayer player) => throw new NotImplementedException();

    public void ClearGames() => throw new NotImplementedException();

    public bool Equals(ITeam other) => throw new NotImplementedException();

    public IOrderedEnumerable<IGrouping<int, IGame>> GetGamesGroupedByRound() => throw new NotImplementedException();

    public (int positiv, int negativ) GetSpielPunkte(bool live = false) => throw new NotImplementedException();

    public double GetStockNote(bool live = false) => throw new NotImplementedException();

    public (int positiv, int negativ) GetStockPunkte(bool live = false) => throw new NotImplementedException();

    public int GetStockPunkteDifferenz(bool live = false) => throw new NotImplementedException();

    public void RemovePlayer(IPlayer selectedPlayer) => throw new NotImplementedException();



    #endregion
}
