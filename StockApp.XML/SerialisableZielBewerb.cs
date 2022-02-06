using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Wettbewerb.Zielbewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

public class SerialisableZielBewerb : IZielBewerb
{
    public SerialisableZielBewerb() { }
    public SerialisableZielBewerb(IZielBewerb zielBewerb) : this()
    {
        SerialisableTeilnehmerListe = new List<SerialisableTeilnehmer>();
        foreach (var teilnehmer in zielBewerb.Teilnehmerliste)
        {
            SerialisableTeilnehmerListe.Add(new SerialisableTeilnehmer(teilnehmer));
        }
    }
    public void ToNormal(IZielBewerb normal)
    {
        for (int i = 1; i < SerialisableTeilnehmerListe.Count; i++)
        {
            normal.AddTeilnehmer(Teilnehmer.Create());
        }

        foreach (var teilnehmer in SerialisableTeilnehmerListe)
        {
            var tln = normal.Teilnehmerliste.First(t => t.Startnummer == teilnehmer.Startnummer);
            teilnehmer.ToNormal(tln);
        }
    }

    [XmlArray(ElementName = "Teilnehmerliste")]
    public List<SerialisableTeilnehmer> SerialisableTeilnehmerListe { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public IOrderedEnumerable<ITeilnehmer> Teilnehmerliste => throw new NotImplementedException();

    [XmlIgnore]
    public IOrderedEnumerable<int> Bahnen => throw new NotImplementedException();
    [XmlIgnore]

    public IOrderedEnumerable<int> FreieBahnen => throw new NotImplementedException();

#pragma warning disable 67
    public event EventHandler TeilnehmerCollectionChanged;
#pragma warning restore 67

    public void AddNewTeilnehmer()
    {
        throw new NotImplementedException();
    }

    public void AddTeilnehmer(ITeilnehmer teilnehmer)
    {
        throw new NotImplementedException();
    }

    public bool CanAddTeilnehmer()
    {
        throw new NotImplementedException();
    }

    public bool CanRemoveTeilnehmer()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ITeilnehmer> GetTeilnehmerRanked()
    {
        throw new NotImplementedException();
    }

    public void MoveTeilnehmer(int oldIndex, int newIndex)
    {
        throw new NotImplementedException();
    }

    public void RemoveTeilnehmer(ITeilnehmer teilnehmer)
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

    #endregion
}
