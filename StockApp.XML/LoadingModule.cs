using StockApp.Core.Turnier;
using System.Xml;
using System.Xml.Serialization;

namespace StockApp.XML;

public static class LoadingModule
{
    public static void Load(ref ITurnier turnier, string filename)
    {
        var serializer = new XmlSerializer(typeof(SerialisableTurnier));
        SerialisableTurnier serialisableTurnier;
        using (XmlReader reader = XmlReader.Create(filename))
        {
            serialisableTurnier = (SerialisableTurnier)serializer.Deserialize(reader);
        }

        serialisableTurnier.ToNormal(turnier);
    }

    public static string Load(string fileName)
    {
        using var reader = new StreamReader(fileName, System.Text.Encoding.UTF8);
        return reader.ReadToEnd();
    }
}


