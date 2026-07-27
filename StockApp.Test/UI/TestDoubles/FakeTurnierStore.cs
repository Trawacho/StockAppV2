using StockApp.Core.Factories;
using StockApp.Core.Turnier;
using StockApp.UI.com;
using StockApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Test.UI.TestDoubles;

/// <summary>
/// Test-Double für <see cref="ITurnierStore"/>. Nutzt bewusst ein echtes <see cref="ITurnier"/>
/// (Core-Objekte sind ohne externe Abhängigkeiten konstruierbar), damit ViewModel-Tests keine
/// Annahmen über die Core-internen Event-Kaskaden mocken müssen.
/// </summary>
public class FakeTurnierStore : ITurnierStore
{
    public ITurnier Turnier { get; set; } = StockApp.Core.Turnier.Turnier.Create();

    public event EventHandler CurrentTurnierChanged;
    public event EventHandler FileNameChanged;

    public string FileName { get; set; } = string.Empty;
    public bool IsDutyValue { get; set; } = false;
    public int MaxCountOfTeams { get; set; } = GamePlanFactory.LoadAllGameplans().Select(t => t.Teams).Max();
    public IEnumerable<IVerein> TemplateVereine { get; set; } = Enumerable.Empty<IVerein>();

    public int LoadCallCount { get; private set; }
    public int SaveCallCount { get; private set; }
    public int SaveAsCallCount { get; private set; }

    public void RaiseCurrentTurnierChanged() => CurrentTurnierChanged?.Invoke(this, EventArgs.Empty);
    public void RaiseFileNameChanged() => FileNameChanged?.Invoke(this, EventArgs.Empty);

    public void Load() => LoadCallCount++;
    public void Load(string fullFileName) => LoadCallCount++;
    public void Save() => SaveCallCount++;
    public void SaveAs() => SaveAsCallCount++;
    public bool IsDuty() => IsDutyValue;
}
