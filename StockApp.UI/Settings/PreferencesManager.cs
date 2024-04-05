using System;
using System.IO;
using System.Xml;

namespace StockApp.UI.Settings;


public class PreferencesManager
{
    public const string FORMAT_VERSION = "1.0";
    private GeneralAppSettings _generalAppSettings = new GeneralAppSettings();
    private static PreferencesManager instance = null;
    private static object locker = new object();
    private readonly ILogger _log;

    public static GeneralAppSettings GeneralAppSettings
    {
        get
        {
            if (instance == null)
                instance = new PreferencesManager();

            return instance._generalAppSettings;
        }
    }



    public static void Initialize()
    {
        instance = new PreferencesManager();
    }

    public static void Save()
    {
        if (instance == null)
            return;

        lock (locker)
            instance.Export();
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
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;

            using (XmlWriter w = XmlWriter.Create(Software.PreferencesFile, settings))
            {
                WriteXML(w);
            }
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

        string preferencesFile = ConvertIfNeeded(Software.PreferencesFile);

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        settings.IgnoreProcessingInstructions = true;
        settings.IgnoreWhitespace = true;
        settings.CloseInput = true;

        XmlReader reader = null;
        reader = XmlReader.Create(preferencesFile, settings);

        try
        {
            ReadXML(reader);
        }
        catch (Exception e)
        {
            _log.Error("An error happened during the parsing of the preferences file");
            _log.Error(e);
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
                default:
                    reader.ReadOuterXml();
                    break;
            }
        }

        reader.ReadEndElement();
    }

    private void WritePreference(XmlWriter writer, ISettingsSerializer serializer)
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
