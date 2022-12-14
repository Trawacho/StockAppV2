using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Core.Wettbewerb.Zielbewerb;
using System.ComponentModel;
using System.Xml.Serialization;

namespace StockApp.XML;



[XmlType(TypeName = "Turnier")]
[Serializable]
public class SerialisableTurnier : ITurnier
{
    public SerialisableTurnier()
    {

    }
    public SerialisableTurnier(ITurnier turnier)
    {
        this.Organisation = new SerialisableOrganisation(turnier.OrgaDaten);

        if (turnier.Wettbewerb is ITeamBewerb teamBewerb)
        {
            TeamBewerb = new SerialisableTeamBewerb(teamBewerb);
        }
        else if (turnier.Wettbewerb is IZielBewerb zielBewerb)
        {
            ZielBewerb = new SerialisableZielBewerb(zielBewerb);
        }
        else if (turnier.Wettbewerb is IContainerTeamBewerbe containerTeamBewerbe)
        {
            TeamBewerbContainer = new SerialisableContainerTeamBewerbe(containerTeamBewerbe);
        }
    }

    public void ToNormal(ITurnier normal)
    {
        Organisation.ToNormal(normal.OrgaDaten);
        if (TeamBewerb != null)
        {
            normal.SetBewerb(Wettbewerbsart.Team);
            normal.ContainerTeamBewerbe.RemoveAll();
            TeamBewerb.ToNormal(normal.ContainerTeamBewerbe.CurrentTeamBewerb as ITeamBewerb);
        }
        if (ZielBewerb != null)
        {
            normal.SetBewerb(Wettbewerbsart.Ziel);
            ZielBewerb.ToNormal(normal.Wettbewerb as IZielBewerb);
        }
        if(TeamBewerbContainer is not null)
        {
            normal.SetBewerb(Wettbewerbsart.Team);
            TeamBewerbContainer.ToNormal(normal.ContainerTeamBewerbe as IContainerTeamBewerbe);
        }
    }



    public SerialisableOrganisation Organisation { get; set; }
    public SerialisableTeamBewerb TeamBewerb { get; set; }
    public SerialisableZielBewerb ZielBewerb { get; set; }
    public SerialisableContainerTeamBewerbe TeamBewerbContainer { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public IOrgaDaten OrgaDaten { get; set; }


    [XmlIgnore]
    public IBewerb Wettbewerb => throw new NotImplementedException();
    

    [XmlIgnore]
    public ITeamBewerb ActiveTeamBewerb => throw new NotImplementedException();

    [XmlIgnore]
    public IContainerTeamBewerbe ContainerTeamBewerbe { get; set; }



#pragma warning disable 67
    public event CancelEventHandler WettbewerbChanging;
    public event Action WettbewerbChanged;
#pragma warning restore 67

    public void SetBewerb(Wettbewerbsart team)
    {
        throw new NotImplementedException();
    }

    public void Reset() { }


    #endregion
}