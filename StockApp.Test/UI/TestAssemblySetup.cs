using NUnit.Framework;
using StockApp.UI.Settings;
using System;
using System.IO;

namespace StockApp.Test.UI;

/// <summary>
/// ViewModels, die <see cref="PreferencesManager"/> anfassen (z.B. <see cref="StockApp.UI.ViewModels.GamesPrintsViewModel"/>),
/// verlangen, dass <see cref="Software.Initialize"/> einmal gelaufen ist - sonst ist <see cref="Software.SettingsDirectory"/>
/// null. Läuft normalerweise in App.xaml.cs beim Programmstart; hier für die Testassembly nachgeholt.
/// Zeigt bewusst auf einen Ordner unterhalb des Testbuilds ("AppData" neben der Test-DLL, siehe
/// <see cref="Software.Initialize"/>), damit Tests nie die echten Preferences des Anwenders lesen/schreiben.
/// </summary>
[SetUpFixture]
public class TestAssemblySetup
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        string testAppDataDir = Path.Combine(AppContext.BaseDirectory, "AppData");
        Directory.CreateDirectory(testAppDataDir);

        Software.Initialize(new Version(0, 0, 0));
    }
}
