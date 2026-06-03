# TODO – Critical Issues to Fix

This file tracks critical issues found during codebase analysis. See `docs/EVENTS.md` for details.

---

## Memory Leaks (High Priority)

### ✅ 1. ContainerTeamBewerbe – Lambda Capture Leak
- **Status**: ✅ COMPLETED (2026-06-03)
- **Priority**: CRITICAL (was: happens 100x per session)
- **File**: `StockAppV2.Core/Wettbewerb/Teambewerb/ContainerTeamBewerbe.cs`
- **Commit**: `99f397f` (branch: `fix/container-teambewerbe-lambda-leak`)
- **Solution**: Replaced inline lambdas with dedicated event handler methods
  - `OnCurrentTeamBewerb_GamesChanged()` (line 94-95)
  - `OnCurrentTeamBewerb_TeamsChanged()` (line 97-98)
  - `UnsubscribeFromCurrentTeamBewerb()` for centralized cleanup (line 87-93)
- **Tests**: All 22 NUnit tests passed ✅
- **Impact**: Eliminates ~1 orphaned subscription per tournament load

---

### ✅ 2. GamesViewModel – GamesPrintsViewModel Not Disposed
- **Status**: ✅ COMPLETED (2026-06-03)
- **Priority**: CRITICAL (was: each tournament load)
- **File**: `StockApp.UI/ViewModels/GamesViewModel.cs`
- **Commit**: `6887489` (branch: `fix/games-viewmodel-dispose-leak`)
- **Solution**: Dispose old GamesPrintsViewModel before assigning new one
  - Line 44: `_gamesPrintsViewModel?.Dispose()` in property setter
- **Tests**: All 22 NUnit tests passed ✅
- **Impact**: Eliminates orphaned child ViewModel per tournament load

---

### ✅ 3. LiveResultsTeamViewModel – Closure References & Conditional Cleanup
- **Status**: ✅ COMPLETED (2026-06-03)
- **Priority**: MEDIUM-HIGH (was: affects live results)
- **File**: `StockApp.UI/ViewModels/LiveResultsTeamViewModel.cs`
- **Commit**: `b9074fb` (branch: `fix/live-results-team-viewmodel-closure`)
- **Solution**: Store Game references at subscription time
  - Add `_subscribedGames` field (line 17)
  - Populate in constructor with `AddRange()` (line 39)
  - Unsubscribe from stored Games in Dispose (line 84)
- **Tests**: All 22 NUnit tests passed ✅
- **Impact**: Guarantees correct unsubscription even if Games change between subscription and disposal

---

### ✅ 4. Review Other ViewModels for Similar Issues
- **Status**: Pending
- **Priority**: MEDIUM (systematic review)
- **Scope**: All ViewModels in `StockApp.UI/ViewModels/`
- **Check For**:
  - Event subscriptions without Dispose
  - Inline lambdas (can't unsubscribe)
  - Child ViewModels not disposed
  - Conditional cleanup
- **Docs**: See `docs/EVENTS.md` → "Best Practices"

---

### ✅ 5. Test Memory Leaks with Profiler
- **Status**: Pending
- **Priority**: MEDIUM (after fixes)
- **Test Plan**:
  1. Open profiler (Visual Studio Performance Profiler)
  2. Start app, take baseline heap snapshot
  3. Load tournament
  4. Unload tournament
  5. Repeat 10x
  6. Force GC.Collect()
  7. Take final snapshot
  8. Compare: should see minimal growth
- **Success Criteria**: <5MB growth after 10 load/unload cycles

---

## Documentation Created

These files document the architecture and issues found:

| File | Purpose | Location |
|------|---------|----------|
| `docs/ARCHITECTURE.md` | Deep MVVM patterns, Event Cascade, Dispose | `/docs/` |
| `docs/PATTERNS.md` | Copy-paste ViewModel templates | `/docs/` |
| `docs/TROUBLESHOOTING.md` | Common bugs and fixes | `/docs/` |
| `docs/NETWORKING.md` | NetMQ + mDNS critical patterns | `/docs/` |
| `docs/EVENTS.md` | 70+ events + 3 critical leaks found | `/docs/` |
| `CLAUDE.md` | Project instructions (root) | `/.` |

---

## Related Documentation

- **CLAUDE.md** – Project overview, structure, commands
- **ARCHITECTURE.md** – Patterns (in `docs/`)
- **EVENTS.md** – Event system + critical leaks (in `docs/`)
- **NETWORKING.md** – NetMQ + mDNS (in `docs/`)

---

## How to Track

1. **In IDE**: Todo panel shows pending/in_progress/completed
2. **In Git**: This TODO.md is version-controlled
3. **For Session**: Copy one of the bug numbers and say "work on bug #1" to start

---

*Last Updated: 2026-05-31*
*Created during comprehensive architecture analysis*