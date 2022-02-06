using StockApp.Core.Turnier;
using System.Xml.Serialization;

namespace StockApp.XML;

public static class SavingModule
{
    public static void Save(ref ITurnier turnier, string filePath)
    {
        var x = new SerialisableTurnier(turnier);

        var xmlSerializer = new XmlSerializer(typeof(SerialisableTurnier));

        using var writer = new StreamWriter(filePath, false);
        xmlSerializer.Serialize(writer, x);
    }

    public static string ConvertToXml(ref ITurnier turnier)
    {
        var x = new SerialisableTurnier(turnier);
        var serializer = new XmlSerializer(typeof(SerialisableTurnier));
        using var writer = new StringWriter();
        serializer.Serialize(writer, x);
        return writer.ToString();
    }

}
