using StockApp.Core.Turnier;
using System.Xml.Serialization;

namespace StockApp.XML;

[XmlType(TypeName = "Startgebuehr")]
public class SerialisableStartGebuehr : IStartgebuehr
{
    public SerialisableStartGebuehr()
    {

    }
    public SerialisableStartGebuehr(IStartgebuehr startgebuehr)
    {
        Value = startgebuehr.Value;
        Verbal = startgebuehr.Verbal;
    }

    public double Value { get; set; }
    public string Verbal { get; set; }
}

public static class SerialisableStartGebuehrExtension
{
    public static void ToNormal(this SerialisableStartGebuehr value, IStartgebuehr normal)
    {
        normal.Value = value.Value;
        normal.Verbal = value.Verbal;
    }
}
