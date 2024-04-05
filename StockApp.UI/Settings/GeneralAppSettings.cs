using System.Xml;

namespace StockApp.UI.Settings;

public class GeneralAppSettings : ISettingsSerializer
{
    public string Name => "General";

    private WindowPlaceManager _windowPlaceManager;

    public WindowPlaceManager WindowPlaceManager { get => _windowPlaceManager; set => _windowPlaceManager = value; }

    public GeneralAppSettings()
    {
        WindowPlaceManager = new WindowPlaceManager();
    }

    public bool InstancesOwnPreferences { get; internal set; }

    public void ReadXML(XmlReader reader)
    {
        reader.ReadStartElement();

        while (reader.NodeType == XmlNodeType.Element)
        {
            switch (reader.Name)
            {
                case "WindowPlace":
                    WindowPlaceManager.Add(WindowPlaceManager.Convert(reader.ReadElementContentAsString()));
                    break;

                default:
                    reader.ReadOuterXml();
                    break;
            }
        }

        reader.ReadEndElement();
    }

    public void WriteXML(XmlWriter writer)
    {
        foreach (IWindowPlace item in WindowPlaceManager.WindowPlaces)
        {
            writer.WriteElementString(nameof(WindowPlace), item.ToString());
        }
    }
}
