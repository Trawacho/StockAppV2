# StockApp.Packaging - MS Store AppBundle Erstellen

Das Publishing über die Visual Studio GUI kann zu Fehlern führen. Diese Anleitung zeigt, wie du das AppBundle über die Kommandozeile korrekt erstellst.

## 1. Vorbereitung - Dependencies für beide Architekturen

Stelle sicher, dass die Dependencies für x86 und x64 verfügbar sind:

```powershell
dotnet publish StockApp.UI/StockApp.UI.csproj -c Release -r win-x86 --self-contained
dotnet publish StockApp.UI/StockApp.UI.csproj -c Release -r win-x64 --self-contained
```

**Warum?** Dies bereitet die plattformspezifischen Binaries für Windows 32-Bit (x86) und 64-Bit (x64) vor, die das AppBundle benötigt.

---

## 2. AppBundle erstellen (MS Store Release)

Führe folgenden Befehl aus (als PowerShell):

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe" `
  ".\StockApp.Packaging\StockApp.Packaging.wapproj" `
  /t:Rebuild `
  /p:Configuration=Release `
  /p:Platform=x64 `
  /p:AppxBundle=Always `
  /p:AppxBundlePlatforms="x86|x64" `
  /p:UapAppxPackageBuildMode=StoreUpload `
  /p:AppxPackageDir="$(Get-Location)\StockApp.Packaging\AppPackages\" `
  /p:AppxSymbolPackageEnabled=True `
  /p:AppxPackageSigningEnabled=false
```

**Was dieser Befehl bewirkt:**

| Parameter | Bedeutung |
|-----------|-----------|
| `/t:Rebuild` | Vollständiger Neuaufbau des Projekts |
| `/p:Configuration=Release` | Produktive Optimierung (nicht Debug) |
| `/p:Platform=x64` | Primary Build Platform (x64) |
| `/p:AppxBundle=Always` | Erstellt ein .appxbundle (unterstützt beide Architekturen) |
| `/p:AppxBundlePlatforms="x86\|x64"` | Beide Architekturen im Bundle |
| `/p:UapAppxPackageBuildMode=StoreUpload` | Generiert .appxupload für MS Store |
| `/p:AppxSymbolPackageEnabled=True` | Symbol-Pakete für Debugging |
| `/p:AppxPackageSigningEnabled=false` | Signierung erfolgt im Store |

---

## 3. Output

Das fertige AppBundle findest du unter:
```
.\StockApp.Packaging\AppPackages\StockApp.Packaging_<VERSION>_Test\
```

Wichtige Dateien:
- `*.appxbundle` - Das Package für den MS Store
- `*.appxsym` - Debug-Symbole (optional zum Store hochladen)

---

## 4. Wichtige Konfigurationen

### RuntimeIdentifiers in .csproj

Alle Projekte müssen folgende Zeile haben:

```xml
<RuntimeIdentifiers>win-x86;win-x64</RuntimeIdentifiers>
```

### Platforms in .csproj

Alle Projekte müssen nur x86 und x64 unterstützen:

```xml
<Platforms>x86;x64</Platforms>
```

---

## Troubleshooting

| Problem | Lösung |
|---------|--------|
| `msbuild.exe nicht gefunden` | Nutze den vollständigen Pfad wie oben (pass die Visual Studio Version an) |
| `DesktopBridge.props nicht gefunden` | Installiere "Universal Windows Platform development" in Visual Studio |
| `project.assets.json nicht gefunden` | Lösche `obj/` und `bin/` Ordner, versuche erneut |

---

## Referenzen

- [Microsoft Docs: Desktop to UWP Packaging](https://docs.microsoft.com/windows/msix/desktop/desktop-to-uwp-packaging-dot-net)
- [AppxBundle Format](https://docs.microsoft.com/windows/msix/overview)
