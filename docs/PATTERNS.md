# PATTERNS.md – Reusable Design Patterns

Common patterns used throughout StockAppV2 for consistent code style and maintainability.

---

## ViewModel Creation Pattern

Every ViewModel follows this structure:

```csharp
public class MyViewModel : ViewModelBase
{
    // 1. Dependencies (private readonly)
    private readonly ITurnierStore _turnierStore;
    private readonly INavigationService<OtherViewModel> _navigationService;

    // 2. Commands (public)
    public RelayCommand SaveCommand { get; }
    public NavigateCommand<OtherViewModel> NavigateCommand { get; }

    // 3. Properties (public, using SetProperty)
    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // 4. Collections (public ObservableCollection or backing field)
    public ObservableCollection<ITeam> Teams { get; } = new();

    // 5. Constructor (dependency injection)
    public MyViewModel(
        ITurnierStore turnierStore,
        INavigationService<OtherViewModel> navigationService)
    {
        _turnierStore = turnierStore;
        _navigationService = navigationService;

        // Initialize commands
        SaveCommand = new RelayCommand(
            () => _turnierStore.Save(),
            () => _turnierStore.IsDuty()
        );

        NavigateCommand = new NavigateCommand<OtherViewModel>(_navigationService);

        // Subscribe to events
        _turnierStore.CurrentTurnierChanged += OnTurnierChanged;
        _turnierStore.CurrentTeambewerb_TeamsChanged += OnTeamsChanged;

        // Load initial data
        LoadData();
    }

    // 6. Event handlers (private)
    private void OnTurnierChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(Title));
    }

    private void OnTeamsChanged(object sender, EventArgs e)
    {
        LoadData();
    }

    // 7. Private methods
    private void LoadData()
    {
        Teams.Clear();
        var turnier = _turnierStore.Turnier;
        if (turnier?.ContainerTeamBewerbe?.CurrentTeamBewerb != null)
        {
            foreach (var team in turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Teams)
            {
                Teams.Add(team);
            }
        }
    }

    // 8. Dispose (CRITICAL: unsubscribe all events)
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _turnierStore.CurrentTurnierChanged -= OnTurnierChanged;
                _turnierStore.CurrentTeambewerb_TeamsChanged -= OnTeamsChanged;
                Teams?.DisposeAndClear();
            }
            _disposed = true;
        }
    }
}
```

---

## Nested ViewModel Pattern

For ViewModels that manage collections of other ViewModels:

```csharp
public class ParentViewModel : ViewModelBase
{
    public ObservableCollection<ChildViewModel> Children { get; } = new();

    public ParentViewModel(ITurnierStore store)
    {
        LoadChildren();
    }

    private void LoadChildren()
    {
        Children.Clear();
        var teams = store.Turnier?.ContainerTeamBewerbe?.CurrentTeamBewerb?.Teams;
        
        if (teams != null)
        {
            foreach (var team in teams)
            {
                // Create child ViewModel for each team
                Children.Add(new ChildViewModel(team));
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // IMPORTANT: Call DisposeAndClear on collections of ViewModels
                Children?.DisposeAndClear();
            }
            _disposed = true;
        }
    }

    // Nested ViewModel
    public class ChildViewModel : ViewModelBase
    {
        private readonly ITeam _team;

        public string TeamName => _team.TeamName;
        public int StartNumber => _team.StartNumber;

        public ChildViewModel(ITeam team)
        {
            _team = team;
            _team.PlayersChanged += OnPlayersChanged;
        }

        private void OnPlayersChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(PlayerCount));
        }

        public int PlayerCount => _team.Players.Count();

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _team.PlayersChanged -= OnPlayersChanged;
                }
                _disposed = true;
            }
        }
    }
}
```

---

## Property Changed Pattern (SetProperty)

Standard reactive property pattern used everywhere:

```csharp
// Simple property
private string _name;
public string Name
{
    get => _name;
    set => SetProperty(ref _name, value);  // Auto-notifies if changed
}

// Property with computed value (no setter)
public string DisplayName => $"{Name} ({StartNumber})";

// Property with side effect
private ITeam _selectedTeam;
public ITeam SelectedTeam
{
    get => _selectedTeam;
    set => SetProperty(
        ref _selectedTeam,
        value,
        () => OnTeamSelected()  // Called only if value changed
    );
}

private void OnTeamSelected()
{
    // Refresh related UI
    RefreshGamesDisplay();
    RaisePropertyChanged(nameof(SelectedTeamInfo));
}

// Read-only property (use public field with private setter)
private ObservableCollection<IGame> _games = new();
public ObservableCollection<IGame> Games
{
    get => _games;
    private set => SetProperty(ref _games, value);
}
```

---

## Command Pattern

Standard command implementations:

### Simple Command
```csharp
public RelayCommand SaveCommand { get; }

public MyViewModel()
{
    SaveCommand = new RelayCommand(
        execute: () => DoWork(),
        canExecute: () => IsReady
    );
}
```

### Async Command
```csharp
public AsyncRelayCommand LoadCommand { get; }

public MyViewModel()
{
    LoadCommand = new AsyncRelayCommand(
        executeAsync: async () => await LoadDataAsync(),
        canExecute: () => !IsLoading
    );
}

private async Task LoadDataAsync()
{
    IsLoading = true;
    try
    {
        // Async work
        await Task.Delay(1000);
    }
    finally
    {
        IsLoading = false;
    }
}
```

### Navigation Command
```csharp
public NavigateCommand<TurnierViewModel> NavigateCommand { get; }

public MyViewModel(INavigationService<TurnierViewModel> navService)
{
    NavigateCommand = new NavigateCommand<TurnierViewModel>(navService);
}

// In XAML: <Button Command="{Binding NavigateCommand}" />
```

---

## Event Subscription Pattern

Always pair Subscribe (constructor) with Unsubscribe (Dispose):

```csharp
public class MyViewModel : ViewModelBase
{
    private readonly ITurnierStore _store;

    public MyViewModel(ITurnierStore store)
    {
        _store = store;

        // Subscribe (use += and exact same method reference!)
        _store.CurrentTurnierChanged += OnTurnierChanged;
        _store.Turnier.WettbewerbChanged += OnWettbewerbChanged;
    }

    private void OnTurnierChanged(object sender, EventArgs e) { }
    private void OnWettbewerbChanged(object sender, CancelEventArgs e) { }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unsubscribe (use -= and exact same method reference!)
                _store.CurrentTurnierChanged -= OnTurnierChanged;
                _store.Turnier.WettbewerbChanged -= OnWettbewerbChanged;
            }
            _disposed = true;
        }
    }
}
```

**Important**: The method reference must be **exactly the same** for `-=` to work.

---

## Observable Collection Filtering Pattern

Don't create filtered collections in properties (recreates every time). Cache them:

```csharp
// ❌ WRONG: Recreates on every PropertyChanged
public IEnumerable<ITeam> FilteredTeams => 
    _teams.Where(t => t.StartNumber > 0);

// ✅ CORRECT: Cache in backing field, update on demand
private ObservableCollection<ITeam> _filteredTeams = new();
public ObservableCollection<ITeam> FilteredTeams => _filteredTeams;

private void RefreshFilter()
{
    _filteredTeams.Clear();
    foreach (var team in _teams.Where(t => t.StartNumber > 0))
    {
        _filteredTeams.Add(team);
    }
}
```

---

## Property Binding to ObservableCollection Pattern

For collections that bind to UI (ListBox, DataGrid, etc.):

```csharp
// ✅ Always use ObservableCollection for UI binding
public ObservableCollection<ITeam> Teams { get; } = new();

private void LoadTeams()
{
    Teams.Clear();
    foreach (var team in _source.Teams)
    {
        Teams.Add(team);
    }
}

// In XAML:
// <ListBox ItemsSource="{Binding Teams}" />
```

---

## Setting Property with Callback Pattern

Use when change needs side effects:

```csharp
private ITeam _selectedTeam;
public ITeam SelectedTeam
{
    get => _selectedTeam;
    set => SetProperty(
        ref _selectedTeam,
        value,
        () => OnSelectedTeamChanged()  // Only called if value changed!
    );
}

private void OnSelectedTeamChanged()
{
    // Refresh dependent collections
    Games.Clear();
    foreach (var game in _selectedTeam?.Games ?? Enumerable.Empty<IGame>())
    {
        Games.Add(game);
    }
    
    // Raise other property changes
    RaisePropertyChanged(nameof(SelectedTeamInfo));
}
```

---

## Dialog Pattern

Opening modal dialogs in MVVM:

```csharp
// In App.xaml.cs (initialization)
_dialogStore.Register<LiveResultsTeamViewModel, LiveResultTeamView>();

// In ViewModel
private readonly IDialogService<LiveResultsTeamViewModel> _liveResultsDialog;

public MyViewModel()
{
    _liveResultsDialog = new DialogService<LiveResultsTeamViewModel>(
        dialogStore,
        () => new LiveResultsTeamViewModel(_turnierStore),
        isModal: true
    );
}

public RelayCommand ShowLiveResultsCommand { get; }

public MyViewModel()
{
    ShowLiveResultsCommand = new RelayCommand(
        () => _liveResultsDialog.ShowDialog()
    );
}
```

---

## Factory Method Pattern (Core)

Core objects are created via static factory methods:

```csharp
// ✅ Usage pattern
public static ITurnier Create() => new Turnier();
public static ITeamBewerb Create(int id) => new TeamBewerb(id);

// In initialization code
var turnier = Turnier.Create();
var teamBewerb = TeamBewerb.Create(1);
```

---

## Validation Pattern

Input validation should happen in ViewModel, not View:

```csharp
public class GameResultInputViewModel : ViewModelBase
{
    private int _teamAPoints;
    public int TeamAPoints
    {
        get => _teamAPoints;
        set => SetProperty(ref _teamAPoints, Math.Max(0, value));  // Prevent negative
    }

    private string _validationError;
    public string ValidationError
    {
        get => _validationError;
        private set => SetProperty(ref _validationError, value);
    }

    public RelayCommand SaveCommand { get; }

    public GameResultInputViewModel()
    {
        SaveCommand = new RelayCommand(
            () => SaveResult(),
            () => IsValid
        );
    }

    private bool IsValid => 
        TeamAPoints >= 0 && TeamBPoints >= 0 && _selectedGame != null;

    private void SaveResult()
    {
        if (!IsValid)
        {
            ValidationError = "Invalid points";
            return;
        }

        _selectedGame.Spielstand.SetPoints(TeamAPoints, TeamBPoints);
        ValidationError = null;
    }
}
```

---

## Empty State Pattern

Handle empty collections gracefully:

```csharp
private bool _hasTeams;
public bool HasTeams
{
    get => _hasTeams;
    private set => SetProperty(ref _hasTeams, value);
}

public string EmptyMessage => "No teams added yet. Click 'Add Team' to begin.";

private void RefreshTeams()
{
    var teams = _teamBewerb?.Teams;
    HasTeams = teams?.Any() == true;
    
    Teams.Clear();
    if (teams != null)
    {
        foreach (var team in teams)
        {
            Teams.Add(team);
        }
    }
}

// In XAML:
// <TextBlock Text="{Binding EmptyMessage}" Visibility="{Binding HasTeams, Converter={...}}" />
// <ListBox ItemsSource="{Binding Teams}" Visibility="{Binding HasTeams, Converter={...}}" />
```

