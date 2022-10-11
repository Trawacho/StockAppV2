using StockApp.Core.Wettbewerb.Teambewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Spielstand")]
public class SerialisableSpielstand : ISpielstand
{
    public SerialisableSpielstand() { }
    public SerialisableSpielstand(ISpielstand spielstand)
    {
        A = spielstand.Punkte_Master_TeamA;
        B = spielstand.Punkte_Master_TeamB;
    }

    public int A { get; set; }
    public int B { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public int Punkte_Master_TeamA => throw new NotImplementedException();

    [XmlIgnore]
    public int Punkte_Master_TeamB => throw new NotImplementedException();

    [XmlIgnore]
    public int Punkte_Live_TeamA => throw new NotImplementedException();

    [XmlIgnore]
    public int Punkte_Live_TeamB => throw new NotImplementedException();

    [XmlIgnore]
    public bool IsSetByHand => throw new NotImplementedException();
    
    [XmlIgnore]
    public IOrderedEnumerable<IKehre> Kehren_Live => throw new NotImplementedException();
    [XmlIgnore]
    public IOrderedEnumerable<IKehre> Kehren_Master => throw new NotImplementedException();


#pragma warning disable 67
    public event EventHandler SpielStandChanged;
#pragma warning restore 67

    public void CopyLiveToMasterValues()
    {
        throw new NotImplementedException();
    }

    public int GetCountOfWinningTurnsTeamA(bool live)
    {
        throw new NotImplementedException();
    }

    public int GetCountOfWinningTurnsTeamB(bool live)
    {
        throw new NotImplementedException();
    }

    public IKehre GetKehre(int kehrenNummer, bool isLive = false)
    {
        throw new NotImplementedException();
    }

    public int GetSpielPunkteTeamA(bool live)
    {
        throw new NotImplementedException();
    }

    public int GetSpielPunkteTeamB(bool live)
    {
        throw new NotImplementedException();
    }

    public int GetStockPunkteTeamA(bool live)
    {
        throw new NotImplementedException();
    }

    public int GetStockPunkteTeamB(bool live)
    {
        throw new NotImplementedException();
    }

    public void Reset(bool force = false)
    {
        throw new NotImplementedException();
    }

    public void SetMasterKehre(int kehrenNummer, int teamA = int.MinValue, int teamB = int.MinValue)
    {
        throw new NotImplementedException();
    }

    public void SetLiveValues(int teamA, int teamB)
    {
        throw new NotImplementedException();
    }

    public void SetLiveValues(IOrderedEnumerable<IKehre> kehren)
    {
        throw new NotImplementedException();
    }

    public void SetMasterTeamAValue(int punkteTeamA)
    {
        throw new NotImplementedException();
    }

    public void SetMasterTeamBValue(int punkteTeamB)
    {
        throw new NotImplementedException();
    }

    public void SetMasterKehre(IKehre kehre)
    {
        throw new NotImplementedException();
    }
    #endregion
}
