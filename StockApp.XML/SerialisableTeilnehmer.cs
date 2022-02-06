using StockApp.Core.Wettbewerb.Zielbewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Teilnehmer")]
public class SerialisableTeilnehmer : ITeilnehmer
{
    public SerialisableTeilnehmer() { }
    public SerialisableTeilnehmer(ITeilnehmer teilnehmer)
    {
        LastName = teilnehmer.LastName;
        FirstName = teilnehmer.FirstName;
        LicenseNumber = teilnehmer.LicenseNumber;
        Startnummer = teilnehmer.Startnummer;
        Vereinsname = teilnehmer.Vereinsname;
        Nation = teilnehmer.Nation;
        SerialisableWertungen = new List<SerialisableWertung>();
        foreach (var wertung in teilnehmer.Wertungen)
        {
            SerialisableWertungen.Add(new SerialisableWertung(wertung));
        }
    }
    public void ToNormal(ITeilnehmer normal)
    {
        normal.LastName = LastName;
        normal.FirstName = FirstName;
        normal.LicenseNumber = LicenseNumber;
        normal.Startnummer = Startnummer;
        normal.Vereinsname = Vereinsname;
        normal.Nation = Nation;
        for (int i = 1; i < SerialisableWertungen.Count; i++)
        {
            normal.AddNewWertung();
        }

        foreach (var wrt in SerialisableWertungen)
        {
            var wertung = normal.Wertungen.First(w => w.Nummer == wrt.Nummer);
            wrt.ToNormal(wertung);
        }
    }

    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string LicenseNumber { get; set; }
    public int Startnummer { get; set; }
    public string Vereinsname { get; set; }
    public string Nation { get; set; }

    [XmlArray(ElementName = "Wertungen")]
    public List<SerialisableWertung> SerialisableWertungen { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public string Name => throw new NotImplementedException();

    [XmlIgnore]
    public int AktuelleBahn => throw new NotImplementedException();

    [XmlIgnore]
    public bool HasOnlineWertung => throw new NotImplementedException();

    [XmlIgnore]
    public IWertung OnlineWertung => throw new NotImplementedException();

    [XmlIgnore]
    public IEnumerable<IWertung> Wertungen => throw new NotImplementedException();

    [XmlIgnore]
    public int GesamtPunkte => throw new NotImplementedException();

#pragma warning disable 67
    public event EventHandler WertungenChanged;
    public event EventHandler OnlineStatusChanged;
    public event EventHandler StartNumberChanged;
#pragma warning restore 67

    public IWertung AddNewWertung()
    {
        throw new NotImplementedException();
    }

    public bool CanAddWertung()
    {
        throw new NotImplementedException();
    }

    public bool CanRemoveWertung()
    {
        throw new NotImplementedException();
    }

    public void RemoveWertung(IWertung wertung)
    {
        throw new NotImplementedException();
    }

    public void SetOffline()
    {
        throw new NotImplementedException();
    }

    public void SetOnline(int bahnNummer, int wertungsNummer)
    {
        throw new NotImplementedException();
    }
    #endregion
}
