using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Wettbewerb.Zielbewerb;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Disziplin")]
public class SerialisableDisziplin : IDisziplin
{
    public SerialisableDisziplin()
    {

    }

    public SerialisableDisziplin(IDisziplin disziplin)
    {
        Disziplinart = disziplin.Disziplinart;
        Versuch1 = disziplin.Versuch1;
        Versuch2 = disziplin.Versuch2;
        Versuch3 = disziplin.Versuch3;
        Versuch4 = disziplin.Versuch4;
        Versuch5 = disziplin.Versuch5;
        Versuch6 = disziplin.Versuch6;
    }

    public void ToNormal(IDisziplin normal)
    {
        normal.Versuch1 = Versuch1;
        normal.Versuch2 = Versuch2;
        normal.Versuch3 = Versuch3;
        normal.Versuch4 = Versuch4;
        normal.Versuch5 = Versuch5;
        normal.Versuch6 = Versuch6;
    }


    public StockTVZielDisziplinName Disziplinart { get; set; }

    public int Versuch1 { get; set; }
    public int Versuch2 { get; set; }
    public int Versuch3 { get; set; }
    public int Versuch4 { get; set; }
    public int Versuch5 { get; set; }
    public int Versuch6 { get; set; }

    #region XMLIgnore

    [XmlIgnore]
    public string Name => throw new NotImplementedException();

    [XmlIgnore]
    public int Summe => throw new NotImplementedException();
#pragma warning disable 67
    public event EventHandler ValuesChanged;
#pragma warning restore 67

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public int VersucheCount()
    {
        throw new NotImplementedException();
    }

    public void SetVersuch(int nr, int value)
    {
        throw new NotImplementedException();
    }

    #endregion
}
