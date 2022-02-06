using StockApp.Core.Wettbewerb.Zielbewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Wertung")]
public class SerialisableWertung : IWertung
{
    public SerialisableWertung()
    {

    }

    public SerialisableWertung(IWertung wertung)
    {
        Nummer = wertung.Nummer;
        SerialisableDisziplinen = new List<SerialisableDisziplin>();
        foreach (var disziplin in wertung.Disziplinen)
        {
            SerialisableDisziplinen.Add(new SerialisableDisziplin(disziplin));
        }
    }

    public void ToNormal(IWertung normal)
    {
        normal.Nummer = Nummer;
        foreach (var d in SerialisableDisziplinen)
        {
            var normalDisziplin = normal.Disziplinen.First(x => x.Disziplinart == d.Disziplinart);
            d.ToNormal(normalDisziplin);
        }
    }


    public int Nummer { get; set; }

    [XmlArray(ElementName = "Disziplinen")]
    public List<SerialisableDisziplin> SerialisableDisziplinen { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public IEnumerable<IDisziplin> Disziplinen => throw new NotImplementedException();

    [XmlIgnore]
    public int GesamtPunkte => throw new NotImplementedException();

    [XmlIgnore]
    public bool IsOnline { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [XmlIgnore]
    public int PunkteMassenMitte => throw new NotImplementedException();

    [XmlIgnore]
    public int PunkteSchuesse => throw new NotImplementedException();

    [XmlIgnore]
    public int PunkteMassenSeitlich => throw new NotImplementedException();

    [XmlIgnore]
    public int PunkteKombinieren => throw new NotImplementedException();


#pragma warning disable 67
    public event EventHandler OnlineStatusChanged;
#pragma warning restore 67

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public bool VersucheAllEntered()
    {
        throw new NotImplementedException();
    }

    #endregion
}
