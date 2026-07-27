# Release-Anleitung – StockAppV2

Diese Datei ist die **einzige** vollständige Anleitung für den Release-Ablauf.
Sie fasst zusammen, was in `CONTRIBUTING.md` (Branch-Modell/Versionierung),
`StockApp.Packaging/readme.md` (MS-Store-Build) und
`StockApp.Installer/README.md` (Setup.exe-Build) bisher getrennt beschrieben
war. Bei Änderungen am Release-Ablauf **nur hier** editieren – die anderen
Dokumente verweisen auf diese Datei.

Ein Release erzeugt zwei Artefakte:
1. Ein **AppBundle für den Microsoft Store** (`.appxbundle`/`.appxupload`).
2. Eine **`StockAppInstaller.exe`** (NSIS-Setup) als GitHub-Release-Anhang.

---

## Voraussetzungen

- Visual Studio 2022 mit Workload **"Universal Windows Platform development"**
- Docker (installiert und gestartet) – für den NSIS-Installer-Build
- .NET 8 SDK
- [GitHub CLI](https://cli.github.com/) (`gh`), bereits authentifiziert

---

## Ablauf

Alle Befehle werden vom **Repository-Root** (`StockAppV2/`) aus ausgeführt.

### 1. `develop` releasefähig prüfen

```powershell
dotnet test .\StockApp.Test\StockApp.Test.csproj
```

Alle Tests müssen grün sein. Prüfen, ob `develop` tatsächlich released werden
soll (`git log main..develop`).

### 2. Versionsnummer anheben

SemVer (`MAJOR.MINOR.PATCH`, siehe `CONTRIBUTING.md`). Die Version muss an
**drei Stellen** identisch geändert werden:

| Datei | Feld |
|---|---|
| `StockApp.UI/StockApp.UI.csproj` | `<Version>` |
| `StockApp.Packaging/Package.appxmanifest` | `Identity Version="..."` |
| `StockApp.Installer/installer.nsi` | `FULL_VERSION` |

Commit auf `develop`:

```powershell
git commit -am "chore: Version auf X.Y.Z angehoben"
```

### 3. `develop` nach `main` mergen und taggen

```powershell
git checkout main
git merge --no-ff develop
git tag vX.Y.Z
git push origin main
git push origin vX.Y.Z
```

### 4. MS-Store-AppBundle bauen

Das Publishing über die Visual Studio GUI kann zu Fehlern führen, daher über
die Kommandozeile:

```powershell
# Dependencies für beide Architekturen vorbereiten
dotnet publish StockApp.UI/StockApp.UI.csproj -c Release -r win-x86 --self-contained
dotnet publish StockApp.UI/StockApp.UI.csproj -c Release -r win-x64 --self-contained

# AppBundle erstellen
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

Ergebnis liegt unter
`.\StockApp.Packaging\AppPackages\StockApp.Packaging_<VERSION>_Test\`:
- `*.appxbundle`/`*.appxupload` – zum Hochladen in den [Partner Center](https://partner.microsoft.com/dashboard) (manuell, kein CLI-Schritt).
- `*.appxsym` – Debug-Symbole (optional mit hochladen).

Voraussetzungen in den `.csproj`-Dateien (sollten bereits gesetzt sein):
```xml
<RuntimeIdentifiers>win-x86;win-x64</RuntimeIdentifiers>
<Platforms>x86;x64</Platforms>
```

**Troubleshooting**

| Problem | Lösung |
|---------|--------|
| `msbuild.exe nicht gefunden` | Vollständigen Pfad wie oben nutzen (Visual-Studio-Version anpassen) |
| `DesktopBridge.props nicht gefunden` | Workload "Universal Windows Platform development" installieren |
| `project.assets.json nicht gefunden` | `obj/`- und `bin/`-Ordner löschen, erneut versuchen |

### 5. Installer (`StockAppInstaller.exe`) bauen

Der Installer wird mit [NSIS](https://nsis.sourceforge.io/) gebaut, über
`makensis` in einem Docker-Container (keine lokale NSIS-Installation nötig).

```powershell
# Release-Ordner bereinigen
rm .\StockApp.UI\bin\Release -r

# Executable publizieren (Version anpassen!)
dotnet publish StockApp.UI\StockApp.UI.csproj -r win-x64 -c Release -p:Version=X.Y.Z.0 --sc

# Docker-Image bauen (nur beim ersten Mal / nach Dockerfile-Änderung nötig)
docker build -f .\StockApp.Installer\Dockerfile -t nsisimage .

# Lizenzdatei kopieren
copy .\LICENSE .\StockApp.Installer\License.txt

# Installer bauen
docker run -it --rm -v C:\Users\daniel\source\repos\StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi
```

Ergebnis: `StockApp.Installer/StockAppInstaller.exe`.

### 6. GitHub-Release erstellen

```powershell
gh release create vX.Y.Z .\StockApp.Installer\StockAppInstaller.exe `
  --title "vX.Y.Z" `
  --notes "- ..."
```

Release-Notes sollten – wie bei bisherigen Releases – auf den Microsoft-Store-Link
verweisen und ggf. auf kompatible StockTV-Versionen hinweisen, z.B.:

```
Um die Updates automatisch zu erhalten, empfiehlt sich die Installation über
den Microsoft Store. [Link](https://apps.microsoft.com/detail/9nmn65mfdzzh?hl=de-DE&gl=DE)

kompatibel mit StockTV >= vX.Y.Z
```

### 7. MS-Store-Upload

Manuell im [Partner Center](https://partner.microsoft.com/dashboard):
`.appxupload` aus Schritt 4 hochladen, Store-Freigabe anstoßen.

### 8. `main` zurück nach `develop` mergen

Damit der Versionsbump-Commit auch in `develop` landet:

```powershell
git checkout develop
git merge main
git push origin develop
```

---

## Referenzen

- `CONTRIBUTING.md` – Branch-Modell, Versionierung (SemVer), Commit-Konventionen
- `StockApp.Packaging/README.md` – Kurzverweis auf diese Datei
- `StockApp.Installer/README.md` – Kurzverweis auf diese Datei
