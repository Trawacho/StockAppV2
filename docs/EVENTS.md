# EVENTS.md – Event System & Cascade Architecture

This document maps the 70+ events across StockAppV2 and their cascades. **Critical for understanding memory leaks and state synchronization.**

---

## Event Overview

The application uses events as the primary communication mechanism connecting Core domain logic to UI updates to network broadcasts.

### Event Categories

```
Core (Domain Events)
├── Game.SpielstandChanged → Score updated
├── Team.PlayersChanged → Player added/removed
├── TeamBewerb.GamesChanged, TeamsChanged → Structure changed
├── Turnier.WettbewerbChanged → Mode switched
└── Spielstand.SpielStandChanged → Score details updated

UI (ViewModel Events)
├── BindableBase.PropertyChanged (for all ViewModels)
├── TurnierStore.CurrentTurnierChanged → Tournament loaded
├── NavigationStore.CurrentViewModelChanged → Screen changed
└── XmlFileService.FullFilePathChanged → File saved

Network (Communication Events)
├── StockTVService.StockTVCollectionChanged → Display discovered
├── StockTVService.StockTVResultChanged → Result sent to display
├── StockTVAppClient.ConnectedChanged → Network connected
└── MDnsService.StockTVDiscovered → mDNS found display

Persistence (XML Events)
└── Serializable*.SpielstandChanged, TeamsChanged, etc.
```

---

## Critical Event Cascades

### Cascade 1: Game Result Entry → Display Update

```
User enters score in ResultInputPerTeamView
    ↓
ResultInputPerTeamViewModel.StockPunkte property setter
    ↓
_game.Spielstand.SetPoints(teamA, teamB)
    ↓
Spielstand.SpielStandChanged fired
    ↓
Game.SpielstandChanged fired
    ↓
Three cascades happen:
    ├─→ ResultInputPerTeamViewModel → RaisePropertyChanged("StockPunkte")
    │   ↓ WPF binding updates UI
    │
    ├─→ LiveResultsTeamViewModel → OnGameResultChanged()
    │   ↓ Updates RankedTeams, refreshes display
    │
    └─→ ResultsViewModel → OnGameResultChanged()
        ↓ Triggers TurnierNetworkManager.SendGameResult()
        ↓ Sends to all StockTV displays via NetMQ
```

**Thread Context**: Main/UI thread (WPF data binding)

**Memory Risk**: ⚠️ **MEDIUM** — See Leak #3 in Known Issues

---

### Cascade 2: Tournament Load → Complete State Reset

```
User loads .skmr file
    ↓
TurnierStore.Load() reads XML
    ↓
TurnierStore.Turnier = newTurnier
    ↓
TurnierStore.CurrentTurnierChanged fired
    ↓
Cascade through ALL ViewModels:
    ├─→ MainViewModel.OnCurrentTurnierChanged()
    │   ├─→ Unsubscribe from old Turnier events
    │   └─→ Subscribe to new Turnier events
    │
    ├─→ ResultsViewModel.OnCurrentTurnierChanged()
    │   ├─→ Unsubscribe from old TeamBewerb games ⚠️
    │   └─→ Subscribe to new TeamBewerb games
    │
    ├─→ GamesViewModel.OnCurrentTurnierChanged()
    │   └─→ Create new GamesPrintsViewModel (old one NOT disposed!) 🚨
    │
    └─→ TeamsViewModel, NavigationTeamViewModel, etc.
        └─→ Repeat: unsubscribe old, subscribe new
```

**Thread Context**: Main/UI thread

**Memory Risk**: 🚨 **CRITICAL** — Multiple cascading leaks here. See Known Issues.

---

### Cascade 3: StockTV Discovery → UI Update (Cross-Thread!)

```
mDNS service runs on background thread
    ↓
ZeroconfResolver finds _stockTV._tcp.local. service
    ↓
MDnsService.RaiseStockTVDiscoverd() (background thread!)
    ↓
StockTVService.AddNewStockTV() (still background thread!)
    ↓
RaiseStockTVCollectionChanged()
    ↓
StockTVCollectionViewModel handler fires
    ↓
App.Current.Dispatcher.Invoke( ← MARSHAL TO UI THREAD
    () => FillStockTvViewModelsCollection()
)
    ↓
Create StockTVViewModel for each display (now on UI thread)
```

**Thread Context**: **BACKGROUND thread (mDNS) → UI thread (Dispatcher)**

**Memory Risk**: 🚨 **CRITICAL SHUTDOWN RACE** — If app closes during discovery, background thread may try to invoke dead Dispatcher.

---

## Known Memory Leak Issues 🚨

### Issue 1: ContainerTeamBewerbe Lambda Capture 🚨🚨🚨

**File**: `StockAppV2.Core/Wettbewerb/Teambewerb/ContainerTeamBewerbe.cs:91-92, 98-99`

**Problem**: Lambda unsubscription doesn't work because each lambda is a different object.

```csharp
// WRONG: This creates an orphaned subscription!
public void SetCurrentTeamBewerb(ITeamBewerb teamBewerb)
{
    if (_currentTeamBewerb != null)
    {
        _currentTeamBewerb.GamesChanged -= (s, e) => RaiseCurrentTeam_GamesChanged();  // ❌
        _currentTeamBewerb.TeamsChanged -= (s, e) => RaiseCurrentTeam_TeamsChanged();  // ❌
    }

    // ... set new team bewerb ...

    if (teamBewerb != null)
    {
        _currentTeamBewerb.GamesChanged += (s, e) => RaiseCurrentTeam_GamesChanged();   // New lambda!
        _currentTeamBewerb.TeamsChanged += (s, e) => RaiseCurrentTeam_TeamsChanged();   // New lambda!
    }
}
```

**Why It Leaks**: Each lambda is a **different object** (different closure instance). `-=` can't find the original lambda to unregister. **Every TeamBewerb change accumulates one more orphaned subscription.**

**Status**: See TODO.md — Issue #1

---

### Issue 2: GamesViewModel Doesn't Dispose Old GamesPrintsViewModel 🚨🚨

**File**: `StockApp.UI/ViewModels/GamesViewModel.cs:32-36`

**Problem**: GamesPrintsViewModel is replaced without calling Dispose().

```csharp
public GamesPrintsViewModel GamesPrintsViewModel
{
    get => _gamesPrintsViewModel;
    set => SetProperty(ref _gamesPrintsViewModel, value);  // ❌ Old one not disposed!
}

// When CurrentTeamBewerb changes (line 33):
private void CurrentTeambewerb_GamesChanged(object sender, EventArgs e)
{
    GamesPrintsViewModel = new GamesPrintsViewModel(CurrentTeamBewerb, _turnierStore);
    // ❌ Old GamesPrintsViewModel is just overwritten, not disposed!
}
```

**Why It Leaks**: GamesPrintsViewModel is a ViewModel (implements IDisposable) that holds event subscriptions. Old instance is replaced without calling Dispose(). **Each tournament load = accumulation of disposed ViewModels in memory.**

**Status**: See TODO.md — Issue #2

---

### Issue 3: LiveResultsTeamViewModel Closure References 🚨

**File**: `StockApp.UI/ViewModels/LiveResultsTeamViewModel.cs:37-39, 81-84`

**Problem**: Subscribes to all games' events. If TeamBewerb is replaced, old games stay referenced in closure.

```csharp
public LiveResultsTeamViewModel(ITurnierStore turnierStore)
{
    _turnierStore = turnierStore;
    var games = CurrentTeamBewerb.GetAllGames();
    
    foreach (var game in games)
    {
        game.Spielstand.SpielStandChanged += OnSpielstandChanged;  // ❌ Closure!
    }
}

protected override void Dispose(bool disposing)
{
    if (!_disposed)
    {
        if (disposing)
        {
            var games = CurrentTeamBewerb?.GetAllGames();  // ⚠️ Conditional!
            if (games != null)
                foreach (var game in games)
                {
                    game.Spielstand.SpielStandChanged -= OnSpielstandChanged;
                }
        }
        _disposed = true;
    }
}
```

**Why It Leaks**: Unsubscribe is conditional. If `CurrentTeamBewerb?.GetAllGames()` returns null, cleanup is **skipped**. Old games stay referenced.

**Status**: See TODO.md — Issue #3

---

## Best Practices for Events in This Project

### 1. Always Store Lambda References (if needed)

```csharp
// ❌ DON'T: Inline lambda (can't unsubscribe reliably)
_source.MyEvent += (s, e) => HandleEvent();

// ✅ DO: Store and use reference
_handler = (s, e) => HandleEvent();
_source.MyEvent += _handler;
// Later:
_source.MyEvent -= _handler;
```

### 2. Unsubscribe in Dispose — Always

```csharp
public class MyViewModel : ViewModelBase
{
    private readonly IStore _store;
    private EventHandler _handler;

    public MyViewModel(IStore store)
    {
        _store = store;
        _handler = (s, e) => OnStoreChanged();
        _store.Changed += _handler;  // Subscribe
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _store.Changed -= _handler;  // ALWAYS unsubscribe
            }
            _disposed = true;
        }
    }
}
```

### 3. Dispose Child ViewModels

```csharp
// When replacing a ViewModel
private MyViewModel _viewModel;

public MyViewModel ViewModel
{
    get => _viewModel;
    set
    {
        _viewModel?.Dispose();  // Dispose old BEFORE replacing
        SetProperty(ref _viewModel, value);
    }
}
```

### 4. Handle Cross-Thread Events

```csharp
// If event fires on background thread but handler updates UI:
public void OnBackgroundEvent(object sender, EventArgs e)
{
    if (sender is BackgroundTask bg)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            // Safe: now on UI thread
            UpdateUIFromBackgroundEvent();
        });
    }
}
```

---

## Event Testing Checklist

When modifying events, verify:

- [ ] Event handler subscribes in constructor
- [ ] Event handler unsubscribes in Dispose()
- [ ] Lambda references are stored (if used)
- [ ] Child ViewModels are disposed (if collections)
- [ ] Thread context is handled (Dispatcher.Invoke if needed)
- [ ] No events remain after Dispose()
- [ ] Cascade works: change → event fires → UI updates → network sends

---

See TODO.md for critical issues requiring fixes.
See ARCHITECTURE.md for detailed MVVM patterns.