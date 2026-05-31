# CLAUDE.md – StockAppV2 Architektur & Entwicklungsrichtlinien

Dies ist ein erweiterte Anleitung für Claude Code. Sie ersetzt vage Beschreibungen durch konkrete Muster, Fehler-Häufungen und klare Regeln.

---

## Repository-Überblick

**StockAppV2** ist eine WPF-Desktopanwendung zur Verwaltung von Stocksport-Turnieren (Stocktennis/Boccia).

- **Lösung**: `StockAppV2.sln`
- **.NET-Ziel**: `net8.0` (plattformunabhängig) + `net8.0-windows7.0` (WPF/Druck)
- **Nullable-Referenztypen**: Global deaktiviert
- **Turnier-Format**: `.skmr` (XML-basiert)

---

## Projekte & Verantwortlichkeiten

| Projekt | Ordner | Namespaces | Zuständigkeit |
|---------|--------|-----------|---------------|
| **Core** | `StockAppV2.Core` | `StockApp.Core.*` | Domänenlogik, Regelwerk-Implementierung, Spielpläne |
| **UI** | `StockApp.UI` | `StockApp.UI.*` | WPF-Views, ViewModels, Navigation, Services |
| **Lib** | `StockApp.Lib` | `StockApp.Lib.*` | Basisklassen (`BindableBase`, `ViewModelBase`), Converter, Utilities |
| **XML** | `StockApp.XML` | `StockApp.XML.*` | Serialisierung, `.skmr` Lade-/Speicher-Module |
| **Comm** | `StockApp.Comm` | `StockApp.Comm.*` | Netzwerk: StockTV (mDNS + NetMQ), UDP-Broadcasting |
| **Prints** | `StockApp.Prints` | `StockApp.Prints.*` | Druckvorlagen und Seitenlayout |
| **Test** | `StockApp.Test` | `StockApp.Test.*` | NUnit-Tests (Fokus: Core + XML) |

**Wichtig**: Der Ordner heißt `StockAppV2.Core`, aber der Namespace ist `StockApp.Core`!

---

## Domänenmodell (StockAppV2.Core)

```
ITurnier (Turnier.cs)
├── IOrgaDaten (Schiedsrichter, Veranstalter, Startgebühren)
├── IContainerTeamBewerbe (Team-Wettbewerbe)
│   └── ITeamBewerb[] (mehrere möglich, eine ist aktuell)
│       ├── ITeam[] (Teams mit Spieler, Spiele, Rankings)
│       └── IGame[] (zwei Teams pro Spiel, Spielstand)
└── IZielBewerb (Ziel-Wettbewerbe)
    ├── IDisziplin[] (Spielklassen: Herren, Damen, Jugend, ...)
    └── ITeilnehmer[] (Einzelne Teilnehmer mit Wertungen)
```

### Kritische Klassen

**Teambewerb (`StockAppV2.Core/Wettbewerb/Teambewerb/`)**
- `ITeamBewerb` – Container für Teams + Spiele einer Gruppe
- `ITeam` – Name, StartNumber, Spieler, Spiele, Rankings
- `IGame` – zwei Teams, Spielstand (Spielpunkte + Stockpunkte)
- `ISpielstand` – Ergebnisse der einzelnen Runden
- `TeamBewerb.GetTeamsRanked(bool live)` – Ranking nach Regelwerk (IERVersion)
- `Paragraph610Evaluator` – Frühabbruch-Logik wenn >50% Spiele fertig

**Zielbewerb (`StockAppV2.Core/Wettbewerb/Zielbewerb/`)**
- `IZielBewerb` – Container für Disziplinen + Teilnehmer
- `IDisziplin` – Spielklasse
- `ITeilnehmer` – Einzelner Teilnehmer
- `IWertung` – Ergebnis pro Runde

**Spielpläne (GamePlanFactory)**
- Aus `StockAppV2.Core/Factories/gpf.json` laden
- Unterstützt 2–22 Teams
- Konfigurierbar: Bahn-Anzahl, Runden, Start-Positionen

---

## MVVM-Architektur (StockApp.UI)

### Schichtenmodell

```
Views (.xaml)
    ↓ DataBinding
ViewModels (erben von ViewModelBase)
    ↓ (kein direkter Zugriff auf Core)
Stores (TurnierStore, NavigationStore, DialogStore)
    ↓ (Domän-Abonnement)
Core (ITurnier, ITeamBewerb, ITeam, ...)
```

**Regel**: Views nehmen nur Bindings vor. **Keine Code-behind-Logik** außer Navigation und Dialoge.

### NavigationStore + NavigationService

```csharp
// In App.xaml.cs: Wird einmalig verdrahtet
var navigationStore = new NavigationStore();
var turnierNavService = new NavigationService<TurnierViewModel>(
    navigationStore,
    () => new TurnierViewModel(turnierStore)
);

// Im ViewModel: Navigation triggern
turnierNavService.Navigate();  // Setzt CurrentViewModel im Store
```

**Dispose-Kette**: `NavigationStore.CurrentViewModel = newVM` ruft `oldVM.Dispose()` auf → **Wichtig für Event-Unsubscription!**

### DialogStore + DialogService

```csharp
// In App.xaml.cs: View-Typen registrieren
_dialogStore.Register<LiveResultsTeamViewModel, LiveResultTeamView>();

// Im ViewModel: Dialog öffnen
var dialogService = new DialogService<LiveResultsTeamViewModel>(
    _dialogStore,
    () => new LiveResultsTeamViewModel(...)
);
dialogService.ShowDialog();
```

Dialoge öffnen als separate Windows (keine Navigation).

### Stores (Reactive State)

**TurnierStore** (`Stores/TurnierStore.cs`)
- Hält aktuelle `ITurnier` Instanz
- Events: `CurrentTurnierChanged`, `FileNameChanged`
- Delegiert Lade/Speicher an `XmlFileService`
- Wird in ViewModels abonniert für reaktive UI

```csharp
public class TeamBewerbViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;

    public TeamBewerbViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        _turnierStore.CurrentTurnierChanged += OnTurnierChanged;  // Subscribe
    }

    private void OnTurnierChanged(object sender, EventArgs e)
    {
        // UI-Bindings aktualisieren
        RaisePropertyChanged(nameof(Teams));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                _turnierStore.CurrentTurnierChanged -= OnTurnierChanged;  // Unsubscribe!
            _disposed = true;
        }
    }
}
```

**Regel**: Jedes `+=` muss ein entsprechendes `-=` in `Dispose()` haben!

### ViewModel-Basisklassen

**BindableBase** (`StockApp.Lib/ViewModels/BindableBase.cs`)
- `INotifyPropertyChanged` Implementierung
- `SetProperty<T>(ref storage, T value)` – Nur bei Änderung Benachrichtigung
- `SetProperty<T>(ref storage, T value, Action onChanged)` – Mit Callback

```csharp
private ITeam _selectedTeam;
public ITeam SelectedTeam
{
    get => _selectedTeam;
    set => SetProperty(
        ref _selectedTeam,
        value,
        () => RefreshGamesDisplay()  // Callback wenn geändert
    );
}
```

**ViewModelBase** (`StockApp.Lib/ViewModels/ViewModelBase.cs`)
- Erbt `BindableBase`
- `IDisposable` Pattern
- Destruktor (nur DEBUG) zur Kontrolle von Memory Leaks

**Wichtig**: ViewModels müssen `Dispose()` implementieren und **alle Events unsubscriben**!

### Commands

- **RelayCommand** – Synchron, Action ohne Parameter
- **AsyncRelayCommand** – Async, Task ohne Parameter
- **NavigateCommand** – Löst Navigation aus
- **DialogCommand** – Öffnet Dialog

```csharp
public RelayCommand SaveCommand { get; }

public MyViewModel(ITurnierStore store)
{
    SaveCommand = new RelayCommand(
        () => store.Save(),
        () => store.IsDuty()  // CanExecute
    );
}
```

---

## Ereignis-Kaskaden (Critical Pattern)

Die Architektur nutzt **Event-Cascading**: Core → Store → ViewModel → View

### Beispiel: Team-Ergebnis eingeben

1. **Nutzer ändert Spielstand in View** (ResultInputPerTeamView.xaml)
2. **ViewModel fangt Change ab** (ResultInputPerTeamViewModel.SetProperty)
3. **ViewModel aktualisiert Core-Objekt** (`game.Spielstand.SetPoints(...)`)
4. **Core feuert Event** (`IGame.SpielstandChanged`)
5. **Store / ViewModel subscribed, feuert PropertyChanged**
6. **View bindet neu** (Ranking, Status, etc.)

**Fehlerhafte Muster**:
- ❌ View direkt auf Core zugreifen
- ❌ Events abonnieren, aber nicht unsubscriben (Memory Leak)
- ❌ Binding zu IEnumerable statt ObservableCollection (keine Refr.)

---

## Result-Input ViewModels

Es gibt mehrere Varianten je nach Turnier-Typ:

### ResultInputPerTeamViewModel
- Pro Team: Alle Spiele anzeigen
- Nutzer wählt Team → Spiele werden gefiltert angezeigt
- `SetPointsPerTeam()` aktualisiert ObservableCollection

### ResultInputAfterGameViewModel
- Pro Spiel: Direkt das Ergebnis eingeben
- Sortierung: `GameNumber` oder `GameNumberOverAll`

### ResultInputPerTeamAndKehreViewModel / ResultInputAfterGameWithKehreViewModel
- Erweiterte Versionen für "Kehre" (Runden)
- Zusätzliche UI für Kehre-Auswahl

**Regel**: Nutze den ViewModle-Typ, der zur aktuellen Eingabe-Methode passt. Nicht vermischen!

---

## Ranking & Regelwerk

### IERVersion (Ranking-Standard)

`ITeamBewerb.IERVersion` definiert die Ranking-Regeln:
- Spielpunkte (0, 2, 3 je nach Sieg/Unentschieden/Niederlage)
- Stockpunkte (Differenz)
- Strafpunkte

Ranking wird mit `TeamBewerb.GetTeamsRanked(bool live)` berechnet.

### Paragraph 610 (Frühabbruch-Regel)

Wenn >50% der Spiele fertig sind, darf Turnier abgebrochen werden:
- Alle Teams spielen gleichviele Spiele
- Letzte Spiele von Teams mit mehr Spielen werden ignoriert

```csharp
// Vor Ranking-Berechnung prüfen:
if (Paragraph610Evaluator.IsApplicable(teamBewerb, live: false))
{
    var adjustedGames = Paragraph610Evaluator.GetAdjustedGames(teamBewerb, live: false);
    // Ranking nur mit adjustedGames berechnen
}
```

---

## XML-Persistenz (StockApp.XML)

### Lade-Speicher-Flow

```
.skmr-Datei (XML)
    ↓ LoadingModule.Load()
XML-DTOs (z.B. TurnierDTO, TeamDTO, GameDTO)
    ↓ Mapper
Domänenobjekte (ITurnier, ITeam, IGame, ...)
```

Umgekehrt: Domänen → XML-DTOs → `.skmr`

### Wichtige Klassen

- **XmlFileService** (`StockApp.UI/Services/XmlFileService.cs`)
  - `Load(ref ITurnier)` – Standard-Datei laden
  - `Load(ref ITurnier, string path)` – Benutzerdatei laden
  - `Save(ref ITurnier)` – Aktuelle Datei speichern
  - `SaveAs(ref ITurnier)` – Neue Datei speichern
  - Events: `FullFilePathChanged`

- **LoadingModule**, **SavingModule** (`StockApp.XML/`)
  - Detaillierte Umwandlung zwischen DTOs ↔ Domain

**Regel**: Änderungen an Domänenobjekten gehören in **StockAppV2.Core**, nicht XML!

---

## Netzwerk & Broadcasting (StockApp.Comm)

### StockTVService
- Findet StockTV-Displays via **mDNS** (Zeroconf)
- Sendet Spielergebnisse + Einstellungen via **NetMQ** (ZeroMQ)
- `_stockTVService.Discover()` – Suche starten
- Events: `DisplayDiscovered`, `DisplayLost`

### BroadcastService
- **UDP-Broadcasting** unabhängig von StockTV
- Alternative Übertragung für Anzeigetafeln

### TurnierNetworkManager
- Koordinator zwischen Services + TurnierStore
- Subscribed auf Store-Events, sendet Updates an Netzwerk

---

## Häufige Fehler & Lösungen

### ❌ Memory Leak: Events nicht unsubscrieben

**Fehler**: ViewModel wird disposed, aber Event bleibt registriert
```csharp
// FALSCH:
public class MyViewModel : ViewModelBase
{
    public MyViewModel(ITurnierStore store)
    {
        store.CurrentTurnierChanged += OnTurnierChanged;  // Nie unsubscribed!
    }
}

// RICHTIG:
protected override void Dispose(bool disposing)
{
    if (!_disposed)
    {
        if (disposing)
            _turnierStore.CurrentTurnierChanged -= OnTurnierChanged;
        _disposed = true;
    }
}
```

### ❌ UI-Logik in Views statt ViewModels

**Fehler**: Code-behind mit Spiellogik
```csharp
// FALSCH (Code-behind):
private void Button_Click(object sender, RoutedEventArgs e)
{
    var teams = myTeamBewerb.Teams.OrderBy(t => t.StartNumber);
    //...
}

// RICHTIG (ViewModel):
public ObservableCollection<ITeam> SortedTeams
{
    get => new ObservableCollection<ITeam>(
        _teamBewerb.Teams.OrderBy(t => t.StartNumber)
    );
}
// View: ItemsSource="{Binding SortedTeams}"
```

### ❌ Binding zu IEnumerable statt ObservableCollection

**Fehler**: Neue Items werden nicht angezeigt
```csharp
// FALSCH:
public IEnumerable<ITeam> Teams { get; set; }  // Keine Refresh-Events!

// RICHTIG:
public ObservableCollection<ITeam> Teams { get; set; }
// Oder: public IEnumerable<ITeam> Teams => _teams;  // Aber sammeln in _teams (ObservableCollection)
```

### ❌ Namespace-Verwechslung StockApp vs StockAppV2

Der Ordner `StockAppV2.Core` hat Namespace `StockApp.Core`! → Immer `using StockApp.Core.*;` schreiben!

### ❌ Spiellogik in UI statt Core

**Fehler**: Ranking oder Regel-Berechnung in ViewModel
```csharp
// FALSCH (ViewModel):
public void CalculateRanking() { /* ... */ }

// RICHTIG (Core):
// TeamBewerb.GetTeamsRanked(bool live) existiert bereits!
var ranked = _currentTeamBewerb.GetTeamsRanked(live: false);
```

### ❌ Dispose in Finalizer aufrufen (wenn nicht DEBUG)

**Fehler**: `Dispose()` wird aufgerufen, obwohl VM noch genutzt wird
```csharp
// Destruktor nur in DEBUG! ViewModels in Release nicht finalizen.
#if DEBUG
    ~ViewModelBase() { ... }
#endif
```

---

## Testing (StockApp.Test)

Tests sind **NUnit**-basiert, Fokus auf **Core + XML**:

```bash
# Alle Tests
dotnet test .\StockApp.Test\StockApp.Test.csproj

# Einzelnen Test
dotnet test .\StockApp.Test\StockApp.Test.csproj --filter "FullyQualifiedName~TeamBewerbTest"
```

### Test-Struktur

```
StockApp.Test/
├── Core/
│   ├── Factories/
│   │   ├── GameplanFactoryTest.cs
│   │   └── GameFactoryTest.cs
│   └── Wettbewerb/
│       ├── Teambewerb/
│       │   ├── TeamRankingComparerTest.cs
│       │   └── TeamBewerbTest.cs
│       └── ...
├── XML/
│   ├── SaveITurnierTest.cs
│   └── LoadITurnierTest.cs
└── Comm/
    ├── MDnsTest.cs
    └── StockTvServiceTest.cs
```

**Regel**: Ändere Core-Logik → Schreib Test. UI-Changes brauchen Tests nur bei komplexer ViewModel-Logik.

---

## Build & Packaging

```bash
# Debug build
dotnet build .\StockAppV2.sln

# Release (beide Architekturen: x86 + x64)
msbuild ".\StockApp.Packaging\StockApp.Packaging.wapproj" \
  /t:"Restore;Publish" \
  /p:Configuration=Release \
  /p:Platform=x64 \
  /p:AppxBundle=Always \
  /p:AppxBundlePlatforms="x86|x64" \
  /p:UapAppxPackageBuildMode=StoreUpload \
  /p:AppxPackageDir="C:\Users\daniel\source\repos\StockAppV2\StockApp.Packaging\AppPackages\" \
  /p:AppxSymbolPackageEnabled=True \
  /p:AppxPackageSigningEnabled=false
```

**Wichtig**: `.wapproj` muss `<RuntimeIdentifiers>win-x86;win-x64</RuntimeIdentifiers>` haben!

---

## Wichtige Dateien & Ressourcen

- `.rulebook/ifi-rules.pdf` – Offizielle Regelwerk (lokal)
- `StockAppV2.Core/Factories/gpf.json` – Spielplan-Konfiguration
- `README.md` – Projekt-Übersicht
- `StockApp.Packaging/readme.md` – Packaging-Details

---

## Checkliste für Claude-Änderungen

Vor Implementierung eines Features:

- [ ] Gehört die Logik in **Core** (Spielregeln) oder **UI** (Anzeige)?
- [ ] Sind alle ViewModels mit Store-Events richtig **disposed**?
- [ ] Nutze `ObservableCollection` statt `IEnumerable` für UI-Binding?
- [ ] Sind neue Core-Klassen **testbar** (keine externe Abhängigkeiten)?
- [ ] Existiert bereits ähnliche Logik (z.B. `Paragraph610Evaluator`)?
- [ ] Wurden alle **neuen Tests geschrieben** (Core-Logik)?
- [ ] Funktioniert die Änderung in **Debug + Release**?
- [ ] Namespace richtig? (`StockApp.*` nicht `StockAppV2.*`)

