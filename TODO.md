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
- **Status**: ✅ COMPLETED (2026-06-03)
- **Priority**: MEDIUM (systematic review)
- **Scope**: All 34 ViewModels in `StockApp.UI/ViewModels/` reviewed
- **Commits**: 
  - `2e22eb5` – Fixed 3 CRITICAL issues
  - `64366cd` – Fixed 4 remaining HIGH issues
- **Total Issues Fixed**: 7 out of 7 additional issues
  - ✅ **LiveResultsZielViewModel**: Dispose child ViewModels in RefreshRanking()
  - ✅ **ZielBewerbViewModel**: Dispose old WertungenViewModel
  - ✅ **NavigationViewModel**: Added missing CurrentTeamBewerbChanged unsubscription
  - ✅ **TeamsViewModel**: Dispose _modalOkCommand before reassignment
  - ✅ **ResultsViewModel**: Property with disposal for ShowLiveResultCommand
  - ✅ **TeilnehmerViewModel**: Lazy initialization for VereinSelectedEnterCommand
  - ✅ **LogViewerViewModel & MainViewModel**: Already correct
- **Tests**: All 22 NUnit tests passed ✅

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

---

# Testsuite-Review 2026-07-23: Produktivcode-Änderungen für bessere Testbarkeit

Entstanden bei der Überarbeitung von `StockApp.Test` (Comm-Tests CI-fähig gemacht,
`Paragraph610Evaluator`-Tests ergänzt, XML-Round-Trip-Tests mit echten Assertions ausgestattet,
Ranking-Lücken v2018/TeamStatus geschlossen, GamePlanFactory-Grenzwerte 2/22 Teams getestet).
Diese Punkte erfordern Änderungen **außerhalb** des Testprojekts und wurden deshalb NICHT
direkt umgesetzt.

## 1. ✅ Geklärt: `SerialisableTurnier` persistiert nur den gerade aktiven Wettbewerb (Team ODER Ziel) – ist Absicht, kein Bug

**Datei:** `StockApp.XML/SerialisableTurnier.cs:20-36`

**Status:** Vom Nutzer bestätigt (2026-07-23): Es ist so gewollt, dass immer nur der jeweils
aktive Bewerb (Ziel ODER Mannschaft) persistiert wird. Kein Handlungsbedarf.

<details>
<summary>Ursprüngliche Beobachtung (zur Referenz)</summary>

Der Konstruktor prüft `turnier.Wettbewerb` (den *aktuell aktiven* Bewerb) und befüllt je nach
Typ entweder `TeamBewerbContainer` **oder** `ZielBewerb`:

```csharp
if (turnier.Wettbewerb is ITeamBewerb teamBewerb) { TeamBewerb = ...; }
else if (turnier.Wettbewerb is IZielBewerb zielBewerb) { ZielBewerb = ...; }
else if (turnier.Wettbewerb is IContainerTeamBewerbe containerTeamBewerbe) { TeamBewerbContainer = ...; }
```

Da `ITurnier.Wettbewerb` beim Team-Modus tatsächlich die `IContainerTeamBewerbe`-Instanz liefert
(siehe `Turnier.cs:101-106`), greift immer entweder der zweite oder der dritte Zweig. Das war beim
Schreiben der Round-Trip-Tests (`StockApp.Test/XML/LoadITurnierTest.cs`) nicht anders testbar,
deshalb wurden dort zwei GETRENNTE Tests geschrieben (Team-Save mit Team aktiv, Ziel-Save mit Ziel
aktiv) statt eines gemeinsamen Tests – das bleibt so bestehen, da es das tatsächliche,
gewollte Verhalten korrekt abbildet.

</details>

## 2. `ZielBewerb.GetTeilnehmerRanked()` verletzt §531 der IFI-Spielordnung (bestätigt, kein Verdacht mehr)

**Datei:** `StockAppV2.Core/Wettbewerb/Zielbewerb/ZielBewerb.cs:245-262`

Gegen `.rulebook/ifi-rules.md` geprüft (§531, Zeile ~828-832) sowie die offizielle Startkarte
(Abb. 17, Zeile ~1702), die die Zuordnung Durchgang↔Disziplin eindeutig belegt:

| Durchgang | Disziplin im Code |
|---|---|
| 1. Durchgang | `MassenMitte` |
| 2. Durchgang | `Schiessen` |
| 3. Durchgang | `MassenSeite` |
| 4. Durchgang | `Kombinieren` |

**Regel §531:** Gesamtpunkte → 4. Durchgang (Kombinieren, bei mehreren Wertungen **zusammengezählt**)
→ 3. Durchgang (MassenSeite) → 2. Durchgang (Schiessen) → danach **kein weiteres Kriterium mehr**,
bei Gleichstand in allen 4 Durchgängen gilt **gleicher Rang**.

**Aktueller Code:**
```csharp
return _teilnehmerliste.OrderByDescending(a => a.GesamtPunkte)
                        .ThenByDescending(b => b.Wertungen.Sum(x => x.PunkteKombinieren))
                        .ThenByDescending(c => c.Wertungen.Max(x => x.PunkteMassenSeitlich))
                        .ThenByDescending(d => d.Wertungen.Max(x => x.PunkteSchuesse))
                        .ThenByDescending(e => e.Wertungen.Max(x => x.PunkteMassenMitte));
```

**Zwei konkrete Abweichungen von §531:**

1. **`.Max()` statt `.Sum()`** bei 3. und 2. Durchgang (`PunkteMassenSeitlich`, `PunkteSchuesse`).
   Für den 4. Durchgang (`PunkteKombinieren`) wird korrekt `.Sum()` verwendet ("zusammengezählt"
   laut Regel) – bei 3./2. Durchgang wechselt der Code inkonsistent zu `.Max()`. Bei Teilnehmern
   mit nur **einer** Wertung ist das unsichtbar (Max == Sum bei einem einzigen Wert), weshalb es
   in der Praxis nie aufgefallen ist. Bei Teilnehmern mit **mehreren** Wertungen (Vergleichsturniere)
   liefert es falsche Platzierungen.
2. **Ein 5. Kriterium, das die Regel nicht kennt:** `PunkteMassenMitte` (1. Durchgang) wird als
   letzter Tie-Break herangezogen. §531 sieht nach 4./3./2. Durchgang explizit **keinen** weiteren
   Vergleich vor – stattdessen "gleicher Rang". Der Code bricht also Gleichstände künstlich auf,
   die laut Regelwerk gleichrangig sein müssten.

**Vorgeschlagene Korrektur:**
```csharp
return _teilnehmerliste.OrderByDescending(a => a.GesamtPunkte)
                        .ThenByDescending(b => b.Wertungen.Sum(x => x.PunkteKombinieren))     // 4. Durchgang
                        .ThenByDescending(c => c.Wertungen.Sum(x => x.PunkteMassenSeitlich))  // 3. Durchgang
                        .ThenByDescending(d => d.Wertungen.Sum(x => x.PunkteSchuesse));       // 2. Durchgang
                        // kein weiteres Kriterium -> §531: Gleichstand in allen 4 Durchgängen = gleicher Rang
```

**Offene Punkte für die Umsetzung (auf später verschoben):**
- Das ist ein Produktivcode-Fix, keine reine Teständerung – ändert die tatsächliche
  Platzierungsreihenfolge bei Gleichstand + mehreren Wertungen. Braucht eigene Regressionstests
  (ein Szenario, in dem `Max` und `Sum` unterschiedliche Ergebnisse liefern; eins, das zeigt, dass
  `MassenMitte` keine Rolle mehr spielt).
- `OrderByDescending`/`ThenByDescending` ist stabil – bei echtem Vollgleichstand bestimmt die
  ursprüngliche Listenreihenfolge die Ausgabe. Das erfüllt "gleicher Rang" nur für die Sortierung
  selbst; falls irgendwo eine Rang**nummer** (1., 2., 2., 4. …) angezeigt wird, müsste diese
  separat aus der sortierten Liste berechnet werden. Noch zu klären, ob es so eine Anzeige gibt.
- Der bestehende Test `StockApp.Test/Core/Wettbewerb/Zielbewerb/RankingTests.cs::TestRanking`
  deckt nur den Volltreffer-Gleichstand (alle Kriterien identisch → stabile Ausgangsreihenfolge)
  ab, nicht die eigentliche Durchgangs-Regel.

## 3. UI/ViewModel-Schicht hat weiterhin keinerlei Testabdeckung

**Projekt:** `StockApp.UI`

Die oben in diesem Dokument als "COMPLETED" markierten Event-Cascade-Memory-Leak-Fixes (Punkte
1–4) haben **keine** Regressionstests – ein künftiges Refactoring könnte sie unbemerkt wieder
einführen. Um das testbar zu machen, wäre nötig:
- Sicherstellen, dass ViewModels ausschließlich über Interfaces (`ITurnierStore`,
  `INavigationService<T>`, `IDialogService<T>` etc.) von ihren Abhängigkeiten abhängen (laut
  CLAUDE.md größtenteils schon der Fall) – ggf. verbleibende direkte Konkret-Typ-Abhängigkeiten
  identifizieren.
- Ein minimales Set an Test-Doubles/Fakes für die Stores (`TurnierStore`, `NavigationStore`,
  `DialogStore`) bereitstellen, damit ViewModel-Konstruktion ohne echtes WPF/Dispatcher-Setup
  möglich ist.
- Prüfen, ob `StockApp.Test` aktuell überhaupt WPF-fähig ist (STA-Thread, `System.Windows`-Referenz)
  – falls nicht, wäre ein zusätzliches Test-Setup (z. B. `[Apartment(ApartmentState.STA)]` oder ein
  eigenes WPF-Testprojekt) nötig.

## 4. NetMQ/mDNS-Kommunikationsschicht ist nicht ohne echte Hardware testbar

**Projekt:** `StockApp.Comm`

`MDnsService` und `StockTVService` greifen direkt auf echte Sockets/mDNS-Resolver zu, ohne
Seam für Testdoubles. Die bestehenden Tests (`MDnsTest.cs`, `StockTvServiceTest.cs`) wurden daher
mit `[Explicit]`/`[Category("Manual")]` versehen und laufen nicht mehr automatisch mit
`dotnet test` mit – sie sind aber weiterhin nur mit echtem StockTV-Gerät im LAN sinnvoll
ausführbar. Für echte CI-taugliche Tests wäre erforderlich:
- Eine Abstraktion für den mDNS-Resolver (`IMdnsResolver` o. ä.), die sich durch einen Fake mit
  simulierten `ServiceAnnouncement`-Events ersetzen lässt.
- Eine Abstraktion für den NetMQ-Transport (Publish/Subscribe-Socket), damit
  `StockTVAppClient`/`StockTV` ohne echtes Netzwerk getestet werden können (ähnlich wie es
  `NetMqStockTvFixesTest.cs` bereits für die reine Nachrichtenverarbeitung tut – dort wird kein
  echtes Netzwerk gebraucht, weil die Klassen dort schon auf Objektebene testbar sind).

## Bereits erledigt (Testsuite, kein Handlungsbedarf mehr)

- Comm-Tests (`MDnsTest`, `StockTvServiceTest`) mit `[Explicit]` + `[Category("Manual")]` markiert,
  laufen nicht mehr automatisch mit `dotnet test`.
- `Paragraph610Evaluator` hat jetzt volle Testabdeckung (Grenzwert 50 %, Live/Master-Unterschied,
  Aussetzer-Ausschluss, ungleiche Spielanzahlen).
- XML-Save/Load-Tests haben jetzt echte Assertions (statt reiner "wirft nicht"-Tests) und nutzen
  isolierte Temp-Dateien mit Cleanup statt des geteilten, reihenfolgeabhängigen `C:\Temp\test.xml`.
- `IERVersion.v2018`-Tie-Break (Stocknote/Quotient) und der TeamStatus-Zweig (ausgeschiedene
  Mannschaften) im Team-Ranking sind jetzt abgedeckt.
- `GamePlanFactory`/`MatchTeamAndGames` haben jetzt einen expliziten Grenzwerttest für 2 und 22
  Teams (Minimum/Maximum laut `gpf.json`).
- `RankingTests.TestRanking` hat jetzt eine Assertion für den Volltreffer-Gleichstand-Fall.