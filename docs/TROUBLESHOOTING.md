# TROUBLESHOOTING.md – Common Issues & Solutions

Quick reference for debugging and fixing common problems in StockAppV2.

---

## Memory Leaks / ViewModel Not Disposed

### Symptom
- App memory usage grows over time
- ViewModels still referenced after navigation
- Events still firing for disposed ViewModels

### Root Cause
ViewModel subscribes to events but doesn't unsubscribe in `Dispose()`.

### Solution

```csharp
// Check all public ViewModel properties and constructor subscriptions

public class MyViewModel : ViewModelBase
{
    public MyViewModel(ITurnierStore store)
    {
        store.CurrentTurnierChanged += OnTurnierChanged;  // ✅ Subscribe
        // ... but where is the unsubscribe?
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // ✅ ADD THIS:
                store.CurrentTurnierChanged -= OnTurnierChanged;  // Unsubscribe!
            }
            _disposed = true;
        }
    }
}
```

See TODO.md for 3 critical leaks found in the codebase.

---

## UI Not Updating After Core Change

### Symptom
- User changes value in UI
- Core object updates
- View doesn't refresh

### Root Causes & Solutions

#### 1. Not using ObservableCollection

```csharp
// ❌ WRONG
public IEnumerable<ITeam> Teams { get; set; }  // No change notifications!

// ✅ CORRECT
public ObservableCollection<ITeam> Teams { get; } = new();
```

#### 2. Not calling RaisePropertyChanged

```csharp
private void OnCoreEventFired(object sender, EventArgs e)
{
    // ❌ WRONG: Just updating data, not notifying view
    _teams = newTeamList;

    // ✅ CORRECT: Notify view
    RaisePropertyChanged(nameof(Teams));
    
    // Or rebuild collection:
    Teams.Clear();
    foreach (var team in newTeamList)
        Teams.Add(team);
}
```

#### 3. Property not bound correctly in XAML

```xaml
<!-- ❌ WRONG: Binding path doesn't match ViewModel property -->
<ListBox ItemsSource="{Binding team}" />  <!-- Property is "Teams" not "team" -->

<!-- ✅ CORRECT -->
<ListBox ItemsSource="{Binding Teams}" />
```

---

## Null Reference Exception in ViewModel

### Solution

```csharp
// Check all public ViewModel properties and constructor subscriptions

public class MyViewModel : ViewModelBase
{
    // ❌ WRONG: Property can be null
    public ObservableCollection<ITeam> Teams { get; set; }

    // ✅ CORRECT: Always initialized
    public ObservableCollection<ITeam> Teams { get; } = new();
}
```

---

## Namespace Not Found / Import Error

### Common Mistakes

1. **Using `StockAppV2.Core` instead of `StockApp.Core`**
   ```csharp
   // ❌ WRONG: Folder name
   using StockAppV2.Core.Wettbewerb;
   
   // ✅ CORRECT: Namespace
   using StockApp.Core.Wettbewerb;
   ```

See CLAUDE.md for full namespace documentation.

---

## Tests Failing After Code Change

### Debugging Steps

1. **Run specific test with output**
   ```bash
   dotnet test .\StockApp.Test\StockApp.Test.csproj \
     --filter "FullyQualifiedName~MyTest" \
     --logger "console;verbosity=detailed"
   ```

2. **Check if Core logic changed**
   - Did you modify `Paragraph610Evaluator`, ranking, or point calculation?
   - Tests verify the OLD behavior – update tests if behavior intentionally changed

3. **Rebuild solution**
   ```bash
   dotnet clean .\StockAppV2.sln
   dotnet build .\StockAppV2.sln
   ```

---

## Question: Where Do I Look First?

| Symptom | Check First |
|---------|-----------|
| UI doesn't update | Binding, ObservableCollection, RaisePropertyChanged |
| App crashes | Null references, NullReferenceException details |
| App slow | Memory leaks (event subscriptions), UI virtualization |
| Wrong calculation | Core logic (Core project), not ViewModel |
| Memory leak | See TODO.md — 3 critical leaks documented |

---

See **docs/EVENTS.md** for detailed event patterns and memory leak prevention.
See **docs/ARCHITECTURE.md** for MVVM pattern details.