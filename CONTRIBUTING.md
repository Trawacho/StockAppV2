# Contributing zu StockAppV2

Dieses Dokument beschreibt den Branching- und Release-Workflow sowie die
Konventionen für Beiträge zu diesem Repository.

---

## Branch-Modell

```
main                    ← immer release-fähig, jeder Merge wird getaggt (vX.Y.Z)
 └── develop            ← Integrationsbranch für die Weiterentwicklung
      ├── feature/xxx   ← neue Funktionalität
      ├── fix/xxx       ← Bugfixes
      └── refactor/xxx  ← Refactorings ohne Verhaltensänderung
hotfix/xxx (von main)   ← dringende Produktionsfixes, siehe unten
```

- **`main`** enthält ausschließlich Code-Stände, die released wurden oder
  released werden können. Es wird **nicht direkt** auf `main` committet.
- **`develop`** ist die Basis für alle laufende Entwicklung. Es wird auch
  hier **nicht direkt committet** – jede Änderung, egal wie klein, läuft über
  einen eigenen `feature/`-, `fix/`- oder `refactor/`-Branch, der von
  `develop` abgezweigt und nach Aufräumen der Historie wieder nach `develop`
  gemergt wird (siehe Ausnahme unten für reine Doku-Änderungen).
- **`feature/<kurzname>`**, **`fix/<kurzname>`**, **`refactor/<kurzname>`**
  – kurze, sprechende Branch-Namen in Kleinbuchstaben mit Bindestrichen,
  z. B. `fix/netmq-stocktv-stability`, `feature/ziel2-support`.
- **`hotfix/<kurzname>`** – nur für kritische Fixes, die *vor* dem nächsten
  regulären Release in Produktion müssen. Wird von `main` abgezweigt und
  anschließend **sowohl in `main` als auch in `develop`** gemergt, damit der
  Fix nicht beim nächsten Release aus `develop` verloren geht.

### Ausnahme: reine Doku-Änderungen

Commits, die **ausschließlich** Dokumentation ändern (z. B. `README.md`,
`CONTRIBUTING.md`, `CLAUDE.md`, `NETWORKING.md`) und keinerlei Code-,
Test- oder Konfigurationsänderung enthalten, dürfen ohne den regulären
Release-Ablauf direkt nach `main` (z. B. per Cherry-Pick) und anschließend
zurück nach `develop` gemergt werden. Kein Versionsbump, kein Tag nötig.
Sobald ein Commit auch nur eine Code-Zeile ändert, gilt wieder der normale
Weg über `develop`.

## Ablauf für Features & Fixes

1. Branch von `develop` erstellen: `git checkout -b fix/kurzname develop`
2. Änderungen umsetzen (siehe Checkliste unten).
3. Tests schreiben/aktualisieren, lokal `dotnet test` laufen lassen.
4. Vor dem Merge Historie aufräumen (siehe unten).
5. Direkt nach `develop` mergen – kein Pull-Request-Prozess nötig.
6. Feature-Branch nach dem Merge löschen.

## Historie sauber halten

Es gibt keinen PR-Zwang, dafür aber die Pflicht, die Historie nicht mit
Rauschen zuzumüllen (keine Commits wie `update`, `wip`, `merge conflict`):

- Zwischen-Commits während der Arbeit sind ok, aber **vor dem Merge nach
  `develop`** per `git rebase -i develop` zu sinnvollen, einzeln
  nachvollziehbaren Commits zusammenfassen/umformulieren.
- Besteht ein Fix/Feature ohnehin nur aus einem sauberen Commit, reicht ein
  **Fast-Forward-Merge** (`git merge --ff-only fix/kurzname`) – erzeugt
  keinen zusätzlichen Merge-Commit.
- Bei mehreren zusammengehörigen Commits ist ein Merge-Commit
  (`git merge --no-ff fix/kurzname`) sinnvoll, um den Zusammenhang sichtbar
  zu halten – dann aber ohne zusätzliche Aufräum-Commits danach.
- Kein Force-Push auf `develop`/`main`. Rebase/Squash passiert nur auf dem
  eigenen Feature-Branch, bevor er gemerged wird.

## Release-Ablauf

Der vollständige Ablauf (Versionsbump an allen Stellen, Merge/Tag, MS-Store-
Build, Installer-Build, GitHub-Release) steht in [`RELEASE.md`](./RELEASE.md)
im Repository-Root – dort **nicht** duplizieren, sondern editieren.

Versionierung folgt [SemVer](https://semver.org/lang/de/): `MAJOR.MINOR.PATCH`
– Breaking Changes/größere Feature-Sprünge erhöhen MINOR (dieses Projekt
hatte historisch noch keine bewussten Breaking Changes, daher bislang keine
MAJOR-Erhöhung über 1).

## Commit-Messages

Es gilt sinngemäß [Conventional Commits](https://www.conventionalcommits.org/de/),
wie bereits in der Historie gelebt:

| Präfix      | Bedeutung                                      |
|-------------|-------------------------------------------------|
| `feat:`     | neue Funktionalität                             |
| `fix:`      | Bugfix                                          |
| `refactor:` | Code-Umbau ohne Verhaltensänderung              |
| `test:`     | neue oder angepasste Tests                      |
| `docs:`     | Dokumentation                                   |
| `chore:`    | Build, Versionsbump, Abhängigkeiten, Sonstiges  |

Kurzer, prägnanter erster Zeilentext (was + warum), Details bei Bedarf im
Body.

## Checkliste vor jedem Merge nach `develop`

Aus `CLAUDE.md` – bitte vor dem Mergen prüfen:

- [ ] Gehört die Logik in **Core** (Spielregeln) oder **UI** (Anzeige)?
- [ ] Sind alle ViewModels mit Store-Events korrekt **disposed**
      (jedes `+=` hat ein `-=` in `Dispose()`)?
- [ ] `ObservableCollection` statt `IEnumerable` für UI-Bindings verwendet?
- [ ] Neue Core-Klassen sind **testbar** (keine externen Abhängigkeiten)?
- [ ] Existiert bereits ähnliche Logik (z. B. `Paragraph610Evaluator`)?
- [ ] Neue/angepasste **Tests** geschrieben (`StockApp.Test`)?
- [ ] Namespace korrekt (`StockApp.*`, nicht `StockAppV2.*`)?
- [ ] Build läuft in Debug **und** Release?

## Tests ausführen

```bash
# Alle Tests
dotnet test .\StockApp.Test\StockApp.Test.csproj

# Einzelnen Test
dotnet test .\StockApp.Test\StockApp.Test.csproj --filter "FullyQualifiedName~TeamBewerbTest"
```

Fokus auf **Core** und **XML**-Logik. UI-Änderungen brauchen nur bei
komplexerer ViewModel-Logik eigene Tests.

## Architektur

Details zu Projektstruktur, MVVM-Mustern, Stores, Event-Kaskaden und
bekannten Fehlermustern stehen in [`CLAUDE.md`](./CLAUDE.md).
