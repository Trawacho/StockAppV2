using System.Xml;

namespace StockApp.UI.Settings;

public interface ISettingsSerializer
{
    string Name { get; }
    void WriteXML(XmlWriter writer);
    void ReadXML(XmlReader reader);
}
