using System;
using System.IO;
using System.Xml;

namespace StockApp.UI.Settings;


public class PreferencesManager
{
    public const string FORMAT_VERSION = "1.0";
    private readonly GeneralAppSettings _generalAppSettings = new();
    private readonly TeamBewerbSettings _teamBewerbSettings = new();
    private static PreferencesManager _instance = null;
    private static readonly object _locker = new();
    private readonly ILogger _log = null;

    public static GeneralAppSettings GeneralAppSettings
    {
        get
        {
            _instance ??= new PreferencesManager();
            return _instance._generalAppSettings;
        }
    }

    public static TeamBewerbSettings TeamBewerbSettings
    {
        get
        {
            _instance ??= new PreferencesManager();
            return _instance._teamBewerbSettings;
        }
    }



    public static void Initialize()
    {
        _instance = new PreferencesManager();
    }

    public static void Save()
    {
        if (_instance == null)
            return;

        lock (_locker)
            _instance.Export();
    }

    private PreferencesManager()
    {
        Import();
    }

    private void Export()
    {
        _log?.DebugFormat("Exporting {0}", Path.GetFileName(Software.PreferencesFile));

        try
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                CloseOutput = true
            };

            using XmlWriter w = XmlWriter.Create(Software.PreferencesFile, settings);
            WriteXML(w);
        }
        catch (Exception e)
        {
            _log?.Error("An error happened during the writing of the preferences file");
            _log?.Error(e);
        }
    }

    private void Import()
    {
        if (!File.Exists(Software.PreferencesFile))
            return;

        _log?.DebugFormat("Importing {0}", Path.GetFileName(Software.PreferencesFile));

        var preferencesFile = ConvertIfNeeded(Software.PreferencesFile);

        var settings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            CloseInput = true
        };

        var reader = XmlReader.Create(preferencesFile, settings);

        try
        {
            ReadXML(reader);
        }
        catch (Exception e)
        {
            _log?.Error("An error happened during the parsing of the preferences file");
            _log?.Error(e);
        }
        finally
        {
            reader?.Close();
        }
    }

    private void WriteXML(XmlWriter writer)
    {
        writer.WriteStartElement("StockAppSettings");
        writer.WriteElementString("FormatVersion", FORMAT_VERSION);
        WritePreference(writer, _generalAppSettings);
        WritePreference(writer, _teamBewerbSettings);

    }
    private void ReadXML(XmlReader reader)
    {
        reader.MoveToContent();

        if (!(reader.Name == "StockAppSettings"))
            return;

        reader.ReadStartElement();
        reader.ReadElementContentAsString("FormatVersion", "");

        while (reader.NodeType == XmlNodeType.Element)
        {
            switch (reader.Name)
            {
                case "General":
                    _generalAppSettings.ReadXML(reader);
                    break;
                case "TeamBewerb":
                    _teamBewerbSettings.ReadXML(reader);
                    break;
                default:
                    reader.ReadOuterXml();
                    break;
            }
        }

        reader.ReadEndElement();
    }

    private static void WritePreference(XmlWriter writer, ISettingsSerializer serializer)
    {
        writer.WriteStartElement(serializer.Name);
        serializer.WriteXML(writer);
        writer.WriteEndElement();
    }

    private static string ConvertIfNeeded(string preferencesFile)
    {
        return preferencesFile;
    }
}
