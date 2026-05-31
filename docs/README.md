# Documentation – StockAppV2 Architecture & Patterns

Comprehensive documentation of StockAppV2's architecture, event system, and known issues.

---

## 📚 Documentation Files

### [ARCHITECTURE.md](ARCHITECTURE.md)
**Deep dive into MVVM patterns and data flow**

- Event Cascade Pattern (User action → Core → ViewModel → UI)
- MVVM Layer Structure (View, ViewModel, Store, Core)
- NavigationStore + Dispose Cascade (memory leak prevention)
- SetProperty Pattern (BindableBase)
- Result Input ViewModel variants (4 types)
- Ranking & Regelwerk (§610, IERVersion)

**Read this when**: Adding new ViewModels, understanding state flow, debugging binding issues

---

### [PATTERNS.md](PATTERNS.md)
**Copy-paste ready code templates**

- ViewModel Creation Pattern (standard structure)
- Nested ViewModel Pattern (parent-child ViewModels)
- Property Changed Pattern (SetProperty usage)
- Command Pattern (RelayCommand, AsyncRelayCommand, NavigateCommand)
- Event Subscription Pattern (subscribe/unsubscribe)
- Observable Collection Filtering Pattern
- Property Binding Pattern
- Setting Property with Callback Pattern
- Dialog Pattern (modal windows)
- Factory Method Pattern
- Validation Pattern
- Empty State Pattern

**Read this when**: Building a new ViewModel, needing a code template

---

### [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
**Common bugs and fixes**

- Memory Leaks / ViewModel Not Disposed
- UI Not Updating After Core Change
- ObservableCollection Items Not Updating
- Binding Not Working / Converter Issue
- Null Reference Exception
- Namespace Not Found / Import Error
- Collection Modified While Iterating
- UI Freezes / Blocking Thread
- Tests Failing After Code Change
- Game Order / Round Numbering Incorrect
- Performance Issues / Slow UI
- **Quick lookup table**: Where to look first for each symptom

**Read this when**: Debugging a problem, something doesn't work as expected

---

### [NETWORKING.md](NETWORKING.md)
**StockTV communication (mission-critical)**

- Three Layers: mDNS Discovery → StockTVService → NetMQ
- Layer 1: Discovery (Zeroconf protocol, port 5353)
- Layer 2: StockTVService (orchestration, thread-safe lists, director pattern)
- Layer 3: StockTVAppClient (NetMQ DEALER socket, poller-based, queues)
- Message Flow (async request-reply, auto-reconnection)
- Critical Failure Modes (debugging guide)
- Disposal & Cleanup (resource freigabe)
- Configuration (ports, timeouts, message topics)
- Testing & Debugging (logging, network sniffing)

**Read this when**: Modifying network code (StockApp.Comm/*), hardware sync issues

---

### [EVENTS.md](EVENTS.md)
**Event system & 3 critical memory leaks 🚨**

- Event Overview (70+ events categorized)
- Critical Event Cascades (Game entry, Tournament load, StockTV discovery)
- **Known Memory Leak Issues** (3 leaks found and documented):
  1. ContainerTeamBewerbe lambda capture (happens 100x per session)
  2. GamesViewModel doesn't dispose old GamesPrintsViewModel
  3. LiveResultsTeamViewModel closure references
- Best Practices (subscribe/unsubscribe patterns)
- Event Testing Checklist

**Read this when**: Modifying ViewModel event subscriptions, fixing memory leaks

---

## 🎯 Quick Navigation

**What's my task?**

| Task | Read This |
|------|-----------|
| Add a new ViewModel | PATTERNS.md |
| Modify event handling | EVENTS.md |
| Debug UI not updating | TROUBLESHOOTING.md |
| Modify StockApp.Comm | NETWORKING.md |
| Understand state flow | ARCHITECTURE.md |
| Something broken? | TROUBLESHOOTING.md |

---

## 🚨 Critical Issues Found

**3 memory leaks discovered during analysis** — see [../TODO.md](../TODO.md) for fixes required.

1. **ContainerTeamBewerbe**: Lambda subscriptions accumulate
2. **GamesViewModel**: Old GamesPrintsViewModel not disposed
3. **LiveResultsTeamViewModel**: Closure references leak on tournament reload

These are documented in [EVENTS.md](EVENTS.md) with code examples and fixes.

---

## 📋 Reference Documents (Root)

- **CLAUDE.md** – Project overview, responsibilities, commands (root)
- **TODO.md** – Critical fixes needed (root)
- **README.md** – Project readme (root)

---

## 🔗 Hooks Configuration

Three automated hooks are configured in `.claude/settings.json`:

1. **ARCHITECTURE.md hook** – Runs before ANY code change
   - Reminds you of key MVVM patterns
   
2. **NETWORKING.md hook** – Runs before StockApp.Comm/* changes
   - Reminds you of critical network patterns
   
3. **EVENTS.md hook** – Runs before *ViewModel.cs changes
   - Reminds you of event disposal patterns and 3 known leaks

---

## 📊 Statistics

- **70+ events** documented
- **3 critical leaks** found
- **5 ViewModel patterns** with templates
- **20+ common issues** with solutions
- **3 event cascades** explained with diagrams

---

*Last Updated: 2026-05-31*
*Created during comprehensive architecture analysis*