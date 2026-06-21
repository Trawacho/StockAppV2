# StockApp Installer

Der Installer wird mit [NSIS](https://nsis.sourceforge.io/) erstellt. Um keine lokale NSIS-Installation zu benötigen, läuft `makensis` in einem Docker-Container.

---

## Voraussetzungen

- [Docker](https://www.docker.com/) installiert und gestartet
- .NET 8 SDK

---

## Release erstellen

Alle Befehle werden vom **Repository-Root** (`StockAppV2/`) aus ausgeführt.

> **Versionsnummer vor dem Build anpassen** — sie muss an drei Stellen geändert werden:
> - `installer.nsi` → `FULL_VERSION`
> - `StockApp.UI/StockApp.UI.csproj` → `<Version>`
> - Publish-Befehl unten (Parameter `-p:Version=`)

### 1. Release-Ordner bereinigen

```powershell
rm .\StockApp.UI\bin\Release -r
```

### 2. Executable publizieren

```powershell
dotnet publish StockApp.UI\StockApp.UI.csproj -r win-x64 -c Release -p:Version=1.14.0.0 --sc
```

### 3. Docker-Image erstellen

Nur beim ersten Mal oder nach Änderungen am `Dockerfile` nötig.

```powershell
# Windows (PowerShell)
docker build -f .\StockApp.Installer\Dockerfile -t nsisimage .

# WSL
docker build -f /mnt/c/Users/daniel/source/repos/StockAppV2/StockApp.Installer/Dockerfile -t nsisimage .
```

### 4. Lizenzdatei kopieren

```powershell
copy .\LICENSE .\StockApp.Installer\License.txt
```

### 5. Installer bauen

```powershell
# Windows (PowerShell)
docker run -it --rm -v C:\Users\daniel\source\repos\StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi

# WSL
docker run -it --rm -v /mnt/c/Users/daniel/source/repos/StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi
```

Das Ergebnis ist `StockApp.Installer/StockAppInstaller.exe`.

---

## Kurzfassung (alle Schritte)

```powershell
rm .\StockApp.UI\bin\Release -r
dotnet publish StockApp.UI\StockApp.UI.csproj -r win-x64 -c Release -p:Version=1.14.0.0 --sc
copy .\LICENSE .\StockApp.Installer\License.txt
docker run -it --rm -v C:\Users\daniel\source\repos\StockAppV2:/build nsisimage /build/StockApp.Installer/installer.nsi
```
