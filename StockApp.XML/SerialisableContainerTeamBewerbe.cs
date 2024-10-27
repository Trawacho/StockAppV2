using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

public class SerialisableContainerTeamBewerbe : IContainerTeamBewerbe
{
    [XmlArray(ElementName = "TeamBewerbe")]
    public List<SerialisableTeamBewerb> SerialisableTeamBewerbe { get; set; }

    #region Constructor

    public SerialisableContainerTeamBewerbe()
    {
        SerialisableTeamBewerbe = new List<SerialisableTeamBewerb>();
    }

    public SerialisableContainerTeamBewerbe(IContainerTeamBewerbe containerTeamBewerbe) : this()
    {
        if (containerTeamBewerbe.TeamBewerbe?.Any() == true)
        {
            foreach (var teamBewerb in containerTeamBewerbe.TeamBewerbe)
            {
                SerialisableTeamBewerbe.Add(new SerialisableTeamBewerb(teamBewerb));
            }
        }
    }

    #endregion

    public void ToNormal(IContainerTeamBewerbe containerTeamBewerbe)
    {

        containerTeamBewerbe.RemoveAll();

        SerialisableTeamBewerbe[0].ToNormal(containerTeamBewerbe.TeamBewerbe.ToList()[0]);
        int i = 1;
        while (SerialisableTeamBewerbe.Count > containerTeamBewerbe.TeamBewerbe.Count())
        {
            SerialisableTeamBewerbe[i].ToNormal(containerTeamBewerbe.AddNew());
            i++;
        }
    }

    #region Methods
    public TeamBewerb AddNew()
    {
        throw new NotImplementedException();
    }

    public void Remove(ITeamBewerb teamBewerb)
    {
        throw new NotImplementedException();
    }

    public void Remove(int teamBewerbId)
    {
        throw new NotImplementedException();
    }

    public void RemoveAll()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void SetBroadcastData(IBroadCastTelegram telegram)
    {
        throw new NotImplementedException();
    }

    public void SetCurrentTeamBewerb(ITeamBewerb teamBewerb)
    {
        throw new NotImplementedException();
    }

    public void SetStockTVResult(IStockTVResult tVResult)
    {
        throw new NotImplementedException();
    }
    #endregion

    [XmlIgnore]
    public ITeamBewerb CurrentTeamBewerb => throw new NotImplementedException();

    [XmlIgnore]
    public IOrderedEnumerable<ITeamBewerb> TeamBewerbe => throw new NotImplementedException();

    [XmlIgnore]
    public IEnumerable<IGameplan> Gameplans => throw new NotImplementedException();

#pragma warning disable 67
    public event EventHandler TeamBewerbeChanged;
    public event EventHandler CurrentTeamBewerbChanged;
    public event EventHandler CurrentTeambewerb_TeamsChanged;
    public event EventHandler CurrentTeambewerb_GamesChanged;
#pragma warning restore 67


}