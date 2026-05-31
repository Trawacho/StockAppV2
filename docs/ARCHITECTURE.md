# ARCHITECTURE.md – Deep Dive into Patterns & Data Flow

This document explains the detailed patterns used in StockAppV2, with code examples and data flow diagrams.

---

## Event Cascade Pattern

The core pattern linking Views → ViewModels → Stores → Core.

```
User Action in View (XAML)
         ↓
ViewModel Command or Property Setter
         ↓
ViewModel updates Core object (e.g., game.Spielstand.SetPoints())
         ↓
Core raises event (e.g., IGame.SpielstandChanged)
         ↓
Store or ViewModel subscribed → fires PropertyChanged
         ↓
View binding refreshes (WPF re-renders)
```

### Example: Entering a Game Result

```csharp
// 1. View (XAML) binds to ViewModel property
<TextBox Text="{Binding TeamAPoints, Mode=TwoWay}" />

// 2. ViewModel property setter
private int _teamAPoints;
public int TeamAPoints
{
    get => _teamAPoints;
    set => SetProperty(
        ref _teamAPoints,
        value,
        () => UpdateGameResult()  // Callback when changed
    );
}

// 3. ViewModel updates Core
private void UpdateGameResult()
{
    _selectedGame.Spielstand.SetPoints(_teamAPoints, _teamBPoints);
}

// 4. Core raises event (in Game.cs)
public void SetPoints(int teamAPoints, int teamBPoints)
{
    // ... validation, update ...
    RaiseSpielstandChanged();  // Raises IGame.SpielstandChanged event
}

// 5. ViewModel subscribed (in constructor)
public MyViewModel()
{
    _selectedGame.SpielstandChanged += (s, e) => 
    {
        RaisePropertyChanged(nameof(RankingDisplay));  // Re-render ranking
    };
}

// 6. View refreshes automatically via binding
```

---

## MVVM Layer Structure

```
Layer              Responsibility              Example Classes
─────────────────────────────────────────────────────────────────
View (XAML)        Render UI, bind data        TeamBewerbView.xaml
                                               ResultInputPerTeamView.xaml

ViewModel          React to events,            TeamBewerbViewModel
                   provide commands,           ResultInputPerTeamViewModel
                   format display data         

Store              Hold shared state,          TurnierStore
                   manage subscriptions        NavigationStore
                                               DialogStore

Core               Calculate rules,            TeamBewerb, Game, Team
                   manage domain logic         Paragraph610Evaluator
                   
XML                Persist to .skmr            LoadingModule, SavingModule
```

### Data Flow: Save Tournament

```
User clicks "Save" button
         ↓
TurnierViewModel.SaveCommand executes
         ↓
TurnierStore.Save()
         ↓
XmlFileService.Save(ref _turnier)
         ↓
SavingModule: ITurnier → XML-DTOs
         ↓
XmlSerializer writes .skmr file
         ↓
XmlFileService fires FullFilePathChanged event
         ↓
TurnierStore fires FileNameChanged event
         ↓
MainViewModel re-renders title with new filename
```

---

## NavigationStore + Dispose Cascade

The **critical** pattern for memory leak prevention.

### Flow

```csharp
public class NavigationStore : INavigationStore
{
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            // ⚠️ CRITICAL: Dispose old ViewModel
            _currentViewModel?.Dispose();  
            
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }
}
```

When user navigates:
1. `NavigationService<T>.Navigate()` is called
2. It sets `_navigationStore.CurrentViewModel = newVM`
3. NavigationStore setter **calls `oldVM.Dispose()`**
4. Old ViewModel's `Dispose()` MUST unsubscribe from all events
5. GC collects old ViewModel

### Correct Implementation

```csharp
public class TeamBewerbViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;

    // Constructor: SUBSCRIBE to events
    public TeamBewerbViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        _turnierStore.CurrentTurnierChanged += OnTurnierChanged;
        _turnierStore.CurrentTeambewerb_TeamsChanged += OnTeamsChanged;
    }

    private void OnTurnierChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(Turnier));
    }

    private void OnTeamsChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(Teams));
    }

    // Dispose: UNSUBSCRIBE from events
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // ⚠️ CRITICAL: Must unsubscribe!
                _turnierStore.CurrentTurnierChanged -= OnTurnierChanged;
                _turnierStore.CurrentTeambewerb_TeamsChanged -= OnTeamsChanged;
            }
            _disposed = true;
        }
    }
}
```

### ObservableCollection Cleanup

If a ViewModel holds ObservableCollections of other ViewModels, must dispose them:

```csharp
public class TeamsViewModel : ViewModelBase
{
    public ObservableCollection<TeamViewModel> Teams { get; } = new();

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Extension method: disposes each ViewModel and clears
                Teams?.DisposeAndClear();
            }
            _disposed = true;
        }
    }
}

// Extension (in CollectionExtensions.cs)
public static void DisposeAndClear<T>(this ObservableCollection<T> collection) 
    where T : IDisposable
{
    foreach (var item in collection)
        item?.Dispose();
    collection.Clear();
}
```

---

## SetProperty Pattern (BindableBase)

Core method for reactive properties in ViewModels.

### Signature

```csharp
protected bool SetProperty<T>(
    ref T storage,
    T value,
    [CallerMemberName] string propertyName = null
)

// With callback:
protected bool SetProperty<T>(
    ref T storage,
    T value,
    Action onChanged,
    [CallerMemberName] string propertyName = null
)
```

### Usage Patterns

**Simple property:**
```csharp
private string _teamName;
public string TeamName
{
    get => _teamName;
    set => SetProperty(ref _teamName, value);  // Auto-notifies if changed
}
```

**Property with side effect:**
```csharp
private ITeam _selectedTeam;
public ITeam SelectedTeam
{
    get => _selectedTeam;
    set => SetProperty(
        ref _selectedTeam,
        value,
        () => RefreshGamesDisplay()  // Called if value actually changed
    );
}

private void RefreshGamesDisplay()
{
    Games.Clear();
    foreach (var game in _selectedTeam?.Games ?? Enumerable.Empty<IGame>())
    {
        Games.Add(new GameViewModel(game));
    }
}
```

**Key Benefits:**
- ✅ Only notifies if value **actually changed**
- ✅ `propertyName` auto-captured via `[CallerMemberName]`
- ✅ Returns `true` if changed (useful for conditions)
- ✅ Callback executes only on change

---

## Result Input ViewModels (Four Variants)

The tournament organizer can enter results in 4 different ways. Each has its own ViewModel.

### 1. ResultInputPerTeamViewModel

**Flow**: Team Selection → Games for that Team

See PATTERNS.md for full ViewModel template patterns.

---

## Ranking & Regelwerk

### IERVersion (Ranking-Standard)

`ITeamBewerb.IERVersion` defines the ranking rules. Ranking is calculated with `TeamBewerb.GetTeamsRanked(bool live)`.

### Paragraph 610 (Frühabbruch-Regel)

Wenn >50% der Spiele fertig sind, darf Turnier abgebrochen werden.

See NETWORKING.md for complete communication patterns and EVENTS.md for event details.