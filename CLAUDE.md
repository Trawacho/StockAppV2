# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository-Überblick

**StockAppV2** ist eine WPF-Desktopanwendung zur Verwaltung und Anzeige von Stocksport-Turnieren. Sie unterstützt Team- und Zielbewerbe, Live-Anzeige über StockTV-Hardware, Ergebnis-Broadcasting und Druckfunktionen. Turnierdateien werden im Format `.skmr` gespeichert.

- Lösung: `StockAppV2.sln`
- .NET-Ziel: `net8.0` (plattformunabhängige Projekte) und `net8.0-windows7.0` (WPF/Druck)
- Nullable-Referenztypen sind **deaktiviert** (global)

### Projektverantwortlichkeiten

| Projekt | Zuständigkeit |
|---|---|
| `StockApp.Core` | Domänenlogik, Turnier- und Bewerbsregeln, GamePlan-Factory |
| `StockApp.UI` | WPF-Anwendung: Views, ViewModels, Services, Navigation |
| `StockApp.Lib` | Gemeinsame UI-Hilfsmittel: `BindableBase`, `ViewModelBase`, Converter, Druckkomponenten |
| `StockApp.Prints` | Druckvorlagen und Seitenlayout |
| `StockApp.XML` | XML-Serialisierung (`XmlSerializer`), Lade-/Speichermodule, `.skmr`-Format |
| `StockApp.Comm` | Netzwerkkommunikation: NetMQ (ZeroMQ), mDNS, StockTV-Integration, UDP-Broadcasting |
| `StockApp.Test` | NUnit-Tests für Core und XML |

## Häufig verwendete Befehle

```bash
# Abhängigkeiten wiederherstellen
dotnet restore .\StockAppV2.sln -r win-x86
dotnet restore .\StockAppV2.sln -r win-x64

# Bauen
dotnet build .\StockAppV2.sln

# Alle Tests ausführen
dotnet test .\StockApp.Test\StockApp.Test.csproj

# Einzelnen Test ausführen
dotnet test .\StockApp.Test\StockApp.Test.csproj --filter "FullyQualifiedName~TestMethodName"

# Packaging (Release, x64+x86, Store-Upload)
msbuild ".\StockApp.Packaging\StockApp.Packaging.wapproj" /t:"Restore;Publish" /p:Configuration=Release /p:Platform=x64 /p:AppxBundle=Always /p:AppxBundlePlatforms="x86|x64" /p:UapAppxPackageBuildMode=StoreUpload /p:AppxPackageDir="C:\Users\daniel\source\repos\StockAppV2\StockApp.Packaging\AppPackages\" /p:AppxSymbolPackageEnabled=True /p:AppxPackageSigningEnabled=false
```

## Architektur

### Domänenmodell (`StockApp.Core`)

```
ITurnier
├── IOrgaDaten          (Schiedsrichter, Veranstalter, Startgebühren)
├── IContainerTeamBewerbe
│   └── ITeamBewerb[]
│       ├── ITeam[]     (StartNumber, TeamName, Nation, Region, Spieler, Spiele)
│       └── IGame[]     (zwei Teams, ISpielstand: Spielpunkte + Stockpunkte)
└── IZielBewerb
    ├── IDisziplin[]    (Spielklassen z.B. Herren, Damen, Jugend)
    └── ITeilnehmer[]   (Startnummer, Vereinsname, Nation, IWertung[])
```

Spielpläne für 2–22 Teams auf verschiedenen Bahnkonfigurationen werden aus `StockApp.Core/Factories/gpf.json` geladen (GamePlanFactory).

### Architekturmuster in `StockApp.UI`

**MVVM ohne IoC-Container** – alle Abhängigkeiten werden manuell in `App.xaml.cs` per Konstruktorinjektion verdrahtet.

**Navigation**
- `NavigationStore`: Hält das aktuelle ViewModel, feuert `CurrentViewModelChanged`
- `NavigationService<TViewModel>`: Generischer Service, der beim Aufruf das ViewModel im Store ersetzt
- Pro Haupt-View existiert eine eigene `NavigationService`-Instanz (erzeugt in `App.xaml.cs`)

**Dialoge**
- `DialogStore`: Registriert ViewModel-Typen → View-Typen
- `DialogService<TViewModel>`: Öffnet das gemappte View als eigenes Fenster

**Stores (reaktiver Zustand)**
- `TurnierStore`: Kapselt `ITurnier`, verwaltet Laden/Speichern/Speichern-unter, informiert über `CurrentTurnierChanged` und `FileNameChanged`
- ViewModels abonnieren Store-Events für reaktive UI-Updates

**ViewModel-Basisklassen** (in `StockApp.Lib`)
- `BindableBase` → `INotifyPropertyChanged` mit `SetProperty<T>`
- `ViewModelBase` → erbt `BindableBase`, implementiert `IDisposable`

**Commands**: `RelayCommand`, `AsyncRelayCommand`, `NavigateCommand`, `DialogCommand`

### Kommunikation (`StockApp.Comm`)

- **StockTVService**: Erkennt StockTV-Displays via mDNS (Zeroconf), sendet Spielergebnisse und Einstellungen per NetMQ (ZeroMQ)
- **BroadcastService**: UDP-Broadcasting von Spielergebnissen (unabhängig von StockTV)
- `TurnierNetworkManager` in `StockApp.UI` vermittelt zwischen diesen Services und dem `TurnierStore`

### Persistenz (`StockApp.XML`)

- `SavingModule`: Domänenobjekte → XML-DTOs → `.skmr`-Datei
- `LoadingModule`: `.skmr`-Datei → XML-DTOs → Domänenobjekte
- `XmlFileService` in `StockApp.UI` delegiert an diese Module

## Wichtige Arbeitsregeln

- Änderungen gehören in das Projekt, das der Zuständigkeit entspricht (s. Tabelle oben).
- UI-Logik bleibt in ViewModels/Services, nicht in Views (Code-behind vermeiden).
- Programmlogik muss mit dem Regelbuch konform sein: `.rulebook/ifi-rules.pdf` (lokal, nicht im Repository).
- Das Projekt hat keine `global.json`; nutze eine installierte .NET-SDK, die `net8.0` unterstützt.
- Für Packaging ist `<RuntimeIdentifiers>win-x86;win-x64</RuntimeIdentifiers>` im `.wapproj` entscheidend.

## Nützliche Dokumente

- `README.md`
- `StockApp.Packaging/readme.md`
- `.rulebook/ifi-rules.pdf`

> Bei Unklarheiten frage den Nutzer nach den genauen Anforderungen oder dem gewünschten Änderungsschwerpunkt.
