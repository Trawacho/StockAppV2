using System;
using System.Diagnostics;
using System.IO;

namespace StockApp.UI.Settings;

public class Software
{
    private static bool _instanceConfigured;
    private static readonly ILogger _log = null; //TODO: Logger fehlt

    public static string PreferencesFile
    {
        get
        {
            if (!_instanceConfigured || string.IsNullOrEmpty(InstanceName) || !PreferencesManager.GeneralAppSettings.InstancesOwnPreferences)
                return SettingsDirectory + "Preferences.xml";
            else
                return SettingsDirectory + string.Format("Preferences.{0}.xml", InstanceName);
        }
    }

    public static string SettingsDirectory { get; private set; }
    public static string InstanceName { get; private set; }
    public static string ApplicationName { get { return "StockApp"; } }
    public static string Version { get; private set; }
    public static bool Is32bit { get; private set; }

    public static void Initialize(Version version)
    {
        Version = version.Build == 0 ?
            string.Format("{0}.{1}", version.Major, version.Minor) :
            string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

        string applicationDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

        Is32bit = IntPtr.Size == 4;

        string portableSettings = Path.Combine(applicationDirectory, "AppData");
        if (Directory.Exists(portableSettings))
            SettingsDirectory = portableSettings + "\\";
        else
            SettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName) + "\\";
    }

    /// <summary>
    /// Setup the name of the instance. Used for the window title and to select a preferences profile.
    /// </summary>
    public static void ConfigureInstance()
    {
        Process[] instances = Process.GetProcessesByName("StockApp.UI");
        int instanceNumber = instances.Length;
        if (instanceNumber == 1)
            InstanceName = null;
        else
            InstanceName = instanceNumber.ToString();

        _instanceConfigured = true;
    }

    public static void SanityCheckDirectories()
    {
        CreateDirectory(SettingsDirectory);
    }

    private static void CreateDirectory(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public static void LogInfo()
    {
        _log?.Info("--------------------------------------------------");
        _log?.InfoFormat("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        _log?.InfoFormat("{0} {1}, {2}.", ApplicationName, Version.ToString(), (IntPtr.Size == 8) ? "x64" : "x86");
        _log?.InfoFormat("{0}", Environment.OSVersion.ToString());
        _log?.InfoFormat(".NET Framework {0}", Environment.Version.ToString());
        _log?.Info("--------------------------------------------------");
    }
}
