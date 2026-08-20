# Finalspiele – Fachkonzept

> Status: **Fachlich abgeschlossen für 4.1–4.4** (Abschnitt 1–6), Domänenmodell-Grundrichtung in
> Abschnitt 7 festgelegt – offene Detailfragen siehe Abschnitt 8. 4.5 KO-Runde wird als eigenständiges
> Konzept separat behandelt. Dieses Dokument entstand gemeinsam mit dem Projektverantwortlichen als
> fachliche Grundlage für eine spätere Programmerweiterung in StockAppV2.

## Inhaltsverzeichnis

- [1. Zweck](#1-zweck)
- [2. Begriffe](#2-begriffe)
- [3. Voraussetzungen & Auslöser](#3-voraussetzungen--auslöser)
- [4. Arten von Finalspielen](#4-arten-von-finalspielen)
  - [4.1 Platzierungsspiele](#41-platzierungsspiele)
  - [4.2 Bahnenspiele (mit sofortigem Ausscheiden)](#42-bahnenspiele-mit-sofortigem-ausscheiden)
  - [4.3 Bahnenspiele](#43-bahnenspiele)
  - [4.4 Page-PlayOff](#44-page-playoff)
  - [4.5 KO-Runde](#45-ko-runde)
- [5. Ablauf & Regeln (allgemein, art-übergreifend)](#5-ablauf--regeln-allgemein-art-übergreifend)
  - [5.1 Finaler Entscheid (§ 4.0.5 DSpO)](#51-finaler-entscheid--405-dspo)
- [6. Sonderfälle](#6-sonderfälle)
  - [6.1 Aufgabe während eines Finalspiels](#61-aufgabe-während-eines-finalspiels)
- [7. Verhältnis zum bestehenden Domänenmodell](#7-verhältnis-zum-bestehenden-domänenmodell)
- [8. Offene Fragen](#8-offene-fragen)

## 1. Zweck

Finalspiele sind ein **optionaler, zusätzlicher Turnierabschnitt nach der regulären Gruppenphase**
(Vorrunde) im Teambewerb. Sie lösen zwei Probleme, die die reine Gruppenphase allein nicht abdeckt:

1. **Fehlende Vergleichbarkeit zwischen Gruppen**: Bei mehreren separaten Gruppen (oder einer
   Split-Gruppe) haben die Teams unterschiedlicher Gruppen nie direkt gegeneinander gespielt. Eine
   gemeinsame Endplatzierung über alle Gruppen hinweg lässt sich aus den Gruppenphasen-Ergebnissen allein
   nicht fair ableiten. Finalspiele stellen die fehlende direkte Begegnung her (4.1 Platzierungsspiele,
   teilweise auch 4.4 Page-PlayOff mit zwei Gruppen).
2. **Zu grobe Differenzierung an der Tabellenspitze**: Auch innerhalb einer einzelnen Gruppe reicht die
   reguläre Platzierung manchen Turnierformaten nicht – z.B. wenn ein "echter" Turniersieger über direkte
   K.o.- oder Playoff-Spiele ermittelt werden soll, statt sich allein auf die Gruppenphasen-Tabelle zu
   verlassen (4.2/4.3 Bahnenspiele, 4.4 Page-PlayOff mit einer Gruppe).

Es gibt nicht **ein** Finalspiele-Format, sondern mehrere unterschiedliche Modi (siehe Abschnitt 4), die
je nach Turnier, Verband oder Vereinstradition zum Einsatz kommen – von schlichten Direktvergleichen
(4.1) über Ausscheidungssysteme (4.2/4.3) bis zum offiziell in der DESV-Spielordnung geregelten
Page-PlayOff (4.4).

**Leitsatz – StockTV-Fähigkeit**: StockAppV2 dient nicht nur der manuellen Turnierauswertung, sondern
auch der Anbindung an StockTV (Live-Anzeige und Verarbeitung von Ergebnissen, siehe `StockApp.Comm`).
Daher gilt für die gesamte Finalspiele-Erweiterung:

> **Jedes Finalspiel muss sich technisch genauso über StockTV abbilden und live übertragen lassen wie ein
> reguläres Spiel der Gruppenphase – die Finalspiele-Erweiterung darf die bestehende StockTV-Anbindung
> an keiner Stelle einschränken.**

An StockTV wird **nicht** der komplette Spielplan übertragen – pro Spiel wird jeweils nur eine **Liste
der Teamnamen inkl. Anspiel-Hinweis** gesendet, und zwar **dynamisch/ereignisgesteuert**: Sobald ein
Spiel abgeschlossen ist, werden die neuen Paarungen ermittelt und übertragen. Das passt direkt zu 4.2–4.4,
wo Paarungen erst rundenweise/stufenweise bekannt werden – ein vorab bekannter Gesamt-Spielplan ist dafür
nicht nötig. Der **Anspiel-Hinweis ist nicht verpflichtend** und kann entfallen; das gilt auch für den
Finalen Entscheid (5.1, strukturell kein reguläres Spiel) – notfalls wird er ohne Anspiel-Hinweis
übertragen.

**Leitsatz – Persistenz (`.skmr`)**: Ein Turnier wird vollständig über die `.skmr`-XML-Datei gespeichert
und wieder geladen (siehe `StockApp.XML`, CLAUDE.md Abschnitt "XML-Persistenz"). Daher gilt ebenfalls für
die gesamte Finalspiele-Erweiterung:

> **Jede Finalspiele-Art (4.1–4.4) muss vollständig in der `.skmr`-Datei abgebildet werden können – ein
> Turnier mit begonnenen oder abgeschlossenen Finalspielen muss sich speichern und verlustfrei wieder
> laden lassen, genau wie die reguläre Gruppenphase.**

Die konkrete Ausmodellierung (welche DTOs, welche Struktur in der XML) folgt erst in Abschnitt 7 – hier
ist zunächst nur die Anforderung selbst festgehalten, nicht deren Umsetzung.

**Leitsatz – Live-Ansicht**: Die reguläre Gruppenphase ist bereits über die bestehende In-App-Live-Ansicht
(z.B. `LiveResultsTeamViewModel`) live mitverfolgbar – unabhängig von StockTV. Daher gilt auch hierfür:

> **Finalspiele (4.1–4.4) müssen sich genauso über die bestehende Live-Ansicht mitverfolgen lassen wie die
> Gruppenphase – Nutzer dürfen während der Finalspiele nicht auf die Live-Ansicht verzichten müssen.**

Wie die Live-Ansicht die unterschiedlichen Strukturen der einzelnen Finalspiele-Arten (Leiter-Position bei
4.2/4.3, Stufen bei 4.4, ...) konkret darstellt, ist noch offen (siehe Abschnitt 8) – hier steht zunächst
nur fest, *dass* sie gebraucht wird.

## 2. Begriffe

| Begriff | Bedeutung |
| --- | --- |
| Vorrunde / Gruppenphase | Der reguläre, bereits bestehende Turnierteil (Round-Robin je Gruppe), aus dessen Ergebnis die Finalspiele-Teilnehmer und -Paarungen ermittelt werden. |
| Finalspiele | Sammelbegriff für alle optionalen Turnierabschnitte nach der Gruppenphase (siehe Abschnitt 4 für die einzelnen Arten). |
| Platzierungsspiele (4.1) | Gleichplatzierte Teams zweier Gruppen spielen je ein Direktvergleichs-Spiel; daraus ergibt sich eine neue, eigenständige Platzierung. |
| Bahnenspiele mit sofortigem Ausscheiden (4.2) | Ladder-System (eine oder zwei Gruppen): Verlierer der jeweils letzten Bahn scheidet pro Runde endgültig aus; Bahnenzahl schrumpft jede Runde um eins. |
| Bahnenspiele (4.3) | Wie 4.2, aber ohne Ausscheiden – alle Bahnen bleiben über eine feste, konfigurierbare Rundenzahl aktiv; die finale Bahn-Position ergibt die Platzierung. |
| Page-PlayOff (4.4) | Offiziell in § 4.0.4 DSpO geregeltes 4-Team-Playoff-System mit den Stufen Ausscheidung, Qualifikation 1, Qualifikation 2 und Finale. |
| Ausscheidung / Qualifikation 1 / Qualifikation 2 / Finale | Offizielle Stufennamen des Page-PlayOff (4.4), siehe dort für den genauen Ablauf. |
| KO-Runde (4.5) | Echtes K.o.-System (Sechzehntelfinale bis Finale) ohne zwingende Gruppenphase; Paarungsbildung, Bahn, Kehren- und Spielanzahl werden manuell als eigener Spielplan festgelegt, keine feste Algorithmik wie bei 4.1–4.4. |
| Best-of-N | Bei 4.5 mögliches Format, bei dem eine Begegnung nicht in einem Einzelspiel, sondern in einer Serie (z.B. Best-of-3/5/7) entschieden wird – wer zuerst die Mehrheit der Spiele gewinnt, gewinnt die Begegnung. |
| Finaler Entscheid | Offizielles Verfahren (§ 4.0.5 DSpO) zur Auflösung eines Unentschiedens ohne zusätzliche reguläre Kehre – je 4 Spieler pro Mannschaft werfen auf Zielringe (Punkte, höher gewinnt) oder auf Distanz zur Daube (cm, niedriger gewinnt); Sieger bekommt +1 Stockpunkt im betroffenen Finalspiel. Details siehe Abschnitt 5.1. |
| TeamA / TeamB | Die zwei Seiten eines `IGame`. Bei Finalspielen (außer 4.4) gilt: TeamA = das Team mit Anspiel (siehe Grundsatz 1, Abschnitt 4). |
| Anspiel | Das Recht, in der ersten Kehre eines Spiels zuerst zu spielen (`IGame.IsTeamA_Starting` im bestehenden Domänenmodell). |
| Leiter / Ladder | Bildliche Bezeichnung für das Bahnen-basierte Auf-/Abstiegssystem bei 4.2 und 4.3 (Bahn 1 = oberste Position). |
| Stehengebliebenes Team / Absteiger / Aufsteiger | Rollenbezeichnungen der Teams innerhalb der Ladder-Mechanik von 4.2/4.3, siehe dort für die genaue Bewegungslogik pro Runde. |
| Bye | Ein Team, das aufgrund ungerader/ungleicher Mannschaftszahl kein Finalspiel bestreitet. Erscheint nicht in der Finalspiele-Ergebnisliste (Grundsatz 3, Abschnitt 4). |
| Finalspiele-Ergebnisliste | Die eigenständige, neue Platzierungsliste aus den Finalspielen – enthält nur tatsächlich gespielte/entschiedene Plätze, getrennt von der regulären Gruppenphasen-Ergebnisliste (beide müssen in der Gesamtdarstellung gemeinsam sichtbar sein). |

## 3. Voraussetzungen & Auslöser

**Bewerbsart**: Finalspiele gibt es grundsätzlich **nur im Teambewerb**, nicht im Zielbewerb.

**Zeitpunkt**: Finalspiele setzen in der Regel eine **abgeschlossene Gruppenphase** voraus – die
Paarungen und/oder Startpositionen von 4.1–4.4 werden aus der finalen Gruppenphasen-Ergebnisliste (bzw.
den finalen Ergebnislisten beider Gruppen) abgeleitet. Vor Abschluss der Gruppenphase sind diese vier
Arten nicht sinnvoll möglich. **Ausnahme: 4.5 KO-Runde** kann auch ganz ohne vorausgehende Gruppenphase
als eigenständiges Turnierformat gespielt werden (siehe dort).

Als "finale Gruppenphasen-Ergebnisliste" gilt – falls die Gruppenphase über `Paragraph610Evaluator`
vorzeitig abgebrochen wurde (§610, >50 % gespielte Spiele) – die **§610-angepasste** Ergebnisliste
(`GetAdjustedGames`), nicht die unangepasste: §610 ist die offizielle Regel zum Turnierabschluss bei
Frühabbruch, ihr Ergebnis ist damit auch offiziell das Endergebnis der Gruppenphase, aus dem 4.1–4.4 ihre
Teilnehmer/Paarungen ableiten.

**Gruppen-Voraussetzung je Art** (Details siehe jeweils Abschnitt 4):

| Art | Eine Gruppe | Zwei Gruppen (bzw. Split-Gruppe) | Ohne Gruppenphase |
| --- | --- | --- | --- |
| 4.1 Platzierungsspiele | – | ✅ (zwingend) | – |
| 4.2 Bahnenspiele mit sofortigem Ausscheiden | ✅ | ✅ | – |
| 4.3 Bahnenspiele | ✅ | ✅ | – |
| 4.4 Page-PlayOff | ✅ | ✅ | – |
| 4.5 KO-Runde | ✅ | ✅ | ✅ |

"Zwei Gruppen" schließt bei **allen vier Arten (4.1–4.4)** eine Split-Gruppe gleichwertig mit ein – eine
Split-Gruppe ist strukturell bereits zwei Blöcke auf gemeinsamen Bahnen; `TeamBewerb.GetSplitTeamsRanked`
liefert dafür bereits die Grundlage.

**Auslöser**: Der Finalspiele-Modus wird **erst nach Abschluss der Gruppenphase** festgelegt – er ist
keine Vorab-Konfiguration, sondern eine Entscheidung, die der Nutzer trifft, sobald die Gruppenphase
beendet ist. Dabei wählt der Nutzer **genau eine** Finalspiele-Art aus; eine Kombination mehrerer Arten im
selben Turnier ist nicht vorgesehen. Da die Auswahl erst nach Gruppenphasen-Ende erfolgt, wird unmittelbar
die zu diesem Zeitpunkt gültige Gruppenphasen-Ergebnisliste übernommen – ein `TeamStatus`-Wechsel
zwischen Gruppenphasen-Ende und Finalspiele-Start kann also nicht auftreten.

## 4. Arten von Finalspielen

Diese Liste ist ein erster Sammelpunkt und **nicht abschließend** – wird im Gespräch erweitert/präzisiert.

> **Grundsatz 1 (gilt für 4.1–4.3 und 4.5)**: TeamA-Zuordnung und Anspiel sind bei Finalspielen
> **identisch** – wer TeamA wird, hat damit automatisch Anspiel. TeamB hat nie Anspiel. Das unterscheidet
> sich bewusst von der regulären Gruppenphase, wo TeamA (`gpf.json`) und Anspiel (`IsTeamA_Starting`)
> unabhängig voneinander sind (dort entscheidet die Bahn-Parität). Bei 4.1–4.3 wird TeamA algorithmisch
> aus der Vorrunden-Leistung hergeleitet; bei 4.5 legt der Nutzer TeamA/TeamB manuell im freien Spielplan
> fest (siehe dort) – in beiden Fällen gilt aber "TeamA = Anspiel". **Ausnahme 4.4 Page-PlayOff**: Dort
> gilt laut offizieller DSpO ein Wahlrecht + Los statt automatischer Zuordnung (siehe dort) – bewusst
> nicht angeglichen, da 4.4 eine eigenständige Regelung mit eigener Herkunft ist.
>
> **Grundsatz 2 (Runde/Nummerierung, gilt für 4.1–4.4)**: `IGame.RoundOfGame` (Wiederholung des kompletten
> Vorrunden-Spielplans) wird für Finalspiele **nicht wiederverwendet** – "Runde" bedeutet in jedem Modus
> etwas anderes (4.1: keine Runden, nur eine Welle; 4.2/4.3: eigene Leiter-Runden-Zählung; 4.4: benannte
> Stufen statt Rundennummer). Konkrete Ausmodellierung folgt in Abschnitt 7.
>
> **Grundsatz 3 (Ergebnisliste zeigt nur Gespieltes, gilt für 4.1–4.4)**: Teams, die keine Paarung
> bestreiten – Bye bei ungerader/ungleicher Mannschaftszahl (4.1–4.3), nicht ausgewählte Paarungen bei
> konfigurierter Teilnehmerzahl X kleiner als möglich (4.1–4.3), nicht teilnehmende Teams außerhalb der
> Top 4 (4.4) – tauchen **nicht** in der Finalspiele-Ergebnisliste auf. Sie bleiben nur über die reguläre
> Gruppenphasen-Ergebnisliste sichtbar; beide Listen müssen daher in der Gesamtdarstellung gemeinsam
> gezeigt werden (siehe Abschnitt 8, Ergebnisliste/Ausdruck).
>
> **4.5 KO-Runde und Grundsatz 2/3**: Noch nicht eingeordnet – siehe Abschnitt 8 (Grundsatz 1 gilt bereits,
> siehe oben). Insbesondere Grundsatz 3 ist nicht trivial übertragbar, weil 4.5 auch **ganz ohne
> Gruppenphase** spielbar sein soll und der Rückfall auf "die reguläre Gruppenphasen-Ergebnisliste" dann
> gar nicht existiert.

### 4.1 Platzierungsspiele

**Wann**: Am Ende eines Turniers, nach Abschluss der regulären Gruppenphase(n).

**Voraussetzung**: Es müssen genau zwei Gruppen vorhanden sein – entweder als **Split-Gruppe** (ein
Spielplan, zwei Blöcke teilen sich die Bahnen) oder als zwei separat angelegte **Gruppen**
(`IContainerTeamBewerbe.TeamBewerbe`).

**Ablauf**:

1. Aus der finalen Ergebnisliste jeder der beiden Gruppen wird die Platzierung ermittelt (reguläres Ranking,
   z.B. via `TeamBewerb.GetTeamsRanked`).
2. Die jeweils **gleichplatzierten Teams** der beiden Gruppen spielen einmal gegeneinander:
   Platz 1 (Gruppe A) vs. Platz 1 (Gruppe B), Platz 2 vs. Platz 2, Platz 3 vs. Platz 3, usw.
3. Alle diese Spiele werden **gleichzeitig** gespielt (bei 10 Teams pro Gruppe: 10 Spiele auf 10 Bahnen
   parallel).
4. Die **Bahnzuteilung ist frei wählbar** – die Paarung "Platz 1 vs. Platz 1" muss nicht auf Bahn 1
   stattfinden, sondern kann auf jede beliebige Bahn gelegt werden.

**Konfigurierbare Anzahl Paarungen**: Der Nutzer kann festlegen, dass **nicht alle** Rang-Paarungen
gespielt werden, sondern nur die besten X. Beispiel: bei 2 Gruppen à 5 Teams nur X=2 → es werden nur
A1 vs. B1 und A2 vs. B2 gespielt; A3/A4/A5 und B3/B4/B5 spielen kein Platzierungsspiel.

**Neue Ergebnisliste** (eigenständig, nicht die reguläre Gruppen-Ergebnisliste):

- Sieger von "1 vs. 1" → Gesamt-Platz 1, Verlierer → Gesamt-Platz 2
- Sieger von "2 vs. 2" → Gesamt-Platz 3, Verlierer → Gesamt-Platz 4
- Sieger von "3 vs. 3" → Gesamt-Platz 5, Verlierer → Gesamt-Platz 6
- usw. – allgemein: Sieger der Begegnung um Platz *k* → Gesamt-Platz `2k-1`, Verlierer → Gesamt-Platz `2k`
- **Wichtig**: Die Finalspiele-Ergebnisliste enthält **nur die tatsächlich gespielten Paarungen**. Wird nur
  X von möglichen Paarungen gespielt (siehe oben), zeigt die Liste nur die Plätze 1 bis `2X` – alle nicht
  gespielten Teams tauchen darin **nicht** auf (sie bleiben nur in der ursprünglichen
  Gruppenphasen-Ergebnisliste sichtbar). Dieses Prinzip ist allgemein als Grundsatz 3 (Abschnitt 4)
  gefasst und gilt auch für 4.2/4.3 (dort begrenzt die Anzahl teilnehmender Paarungen X, nicht ein
  Abbruch).

**Beispiel** (2 Gruppen à 10 Teams, 10 Bahnen gleichzeitig):

| Begegnung | Bahn (frei wählbar) | Sieger wird | Verlierer wird |
| --- | --- | --- | --- |
| A1 vs. B1 | z.B. Bahn 7 | Platz 1 | Platz 2 |
| A2 vs. B2 | z.B. Bahn 3 | Platz 3 | Platz 4 |
| ... | ... | ... | ... |
| A10 vs. B10 | z.B. Bahn 1 | Platz 19 | Platz 20 |

**Ungleiche Gruppengröße**: Hat eine Gruppe mehr Teams als die andere, spielen
die "überzähligen" Teams (ohne gleichplatzierten Gegner) kein Platzierungsspiel. Sie werden **nicht** mehr
gesondert ans Ende der Finalspiele-Ergebnisliste gesetzt, sondern folgen genau dem allgemeinen Prinzip:
nicht gespielte Teams tauchen in der Finalspiele-Ergebnisliste gar nicht auf (siehe "Neue Ergebnisliste"
unten). Ihre Platzierung bleibt über die reguläre Gruppenphasen-Ergebnisliste sichtbar.

**Unentschieden → "Finaler Entscheid"**: Steht ein Platzierungsspiel nach regulärer Wertung auf Gleichstand,
wird der **"Finale Entscheid"** gespielt – siehe Abschnitt 5.1 für das vollständige, offizielle Verfahren
(§ 4.0.5 DSpO). Bei 4.1 löst **jeder** Gleichstand den Finalen Entscheid aus (anders als bei 4.4, wo er
erst als letzte Eskalationsstufe beim zweispieligen Finale greift).

**Anspiel & TeamA/TeamB-Zuordnung**: Abweichend von der regulären Bahnrotations-Formel (Bahn-Parität
entscheidet) wird hier die Team-Zuordnung selbst aus der Gruppenphasen-Leistung abgeleitet – die
Reihenfolge bestimmt gleichzeitig, wer TeamA wird *und* wer das Anspiel hat:

1. Mehr Punkte aus der Gruppenphase
2. Bei Gleichstand: bessere (höhere) Stockpunkte-Differenz
3. Bei weiterem Gleichstand: höhere Anzahl eigener Stockpunkte
4. Bei weiterem Gleichstand: das Team aus Gruppe A

> Das unterscheidet sich bewusst vom court-basierten Anspiel-Schema der regulären Gruppenphase
> (`isTeamA_Starting = Court % 2 != 0`, siehe CLAUDE.md) – hier hängt Anspiel/TeamA an der bisherigen
> Turnierleistung, nicht an der (frei wählbaren) Bahn.

**Kehren-Anzahl**: Bleibt wie in der Gruppenphase (6 oder 8 Kehren, `Is8TurnsGame` unverändert übernommen).

**Überzählige Teams**: Bekommen in der Finalrunde **gar kein Spiel** (auch keinen Pause-Eintrag) und
erscheinen entsprechend nicht in der Finalspiele-Ergebnisliste (siehe "Neue Ergebnisliste" oben).

**Ergebnisliste/Ausdruck**: Da nicht gespielte Teams in der Finalspiele-Ergebnisliste fehlen, **muss** die
endgültige Ergebnisdarstellung immer **beide** Ergebnisse zeigen – das aus der Gruppenphase und das aus der
Finalrunde – damit keine Mannschaft "verschwindet". Das erfordert nicht zwingend zwei Seiten: Bei
entsprechend kleiner Mannschaftszahl kann beides auch auf einer Seite dargestellt werden. Es soll mehrere
Darstellungsoptionen geben; welche genau, ist noch zu sammeln (siehe Abschnitt 8).

### 4.2 Bahnenspiele (mit sofortigem Ausscheiden)

**Grundprinzip**: Eine "Ladder" (Bahnen-Leiter) über mehrere Runden, bei der Bahn 1 die höchste und die
jeweils letzte aktive Bahn die niedrigste Position darstellt. Pro Runde wird eine Bahn weniger benötigt,
bis am Ende nur noch Bahn 1 übrig ist. Funktioniert sowohl mit **einer** als auch mit **zwei**
Vorrunden-Gruppen.

**Start-Paarung** (Runde 1, nach Platzierung der Vorrunde):

- Bei **einer Gruppe**: 1 vs. 2 auf Bahn 1, 3 vs. 4 auf Bahn 2, 5 vs. 6 auf Bahn 3, usw. – jeweils die
  zwei nächstplatzierten Teams auf der nächsten Bahn.
- Bei **zwei Gruppen**: gleichplatzierte Teams gegeneinander (wie bei 4.1) – A1 vs. B1 auf Bahn 1, A2 vs.
  B2 auf Bahn 2, usw., statt Paarung innerhalb einer Gruppe.

**Ungerade/ungleiche Mannschaftszahl**:

- Eine Gruppe, ungerade Mannschaftszahl: Das niedrigstplatzierte Team spielt gar nicht mit.
- Zwei Gruppen, unterschiedliche Größe (z.B. 7 vs. 8): Das/die überzähligen Teams der größeren Gruppe ohne
  Rang-Gegenpart spielen gar nicht mit.
- Zwei gleich große Gruppen (z.B. 7 vs. 7): Kreuzpaarung ergibt automatisch eine gerade Gesamtzahl – kein
  Freilos nötig.
- In allen "spielt nicht mit"-Fällen erscheint das betroffene Team – gemäß Grundsatz 3, Abschnitt 4 –
  **nicht** in der Finalspiele-Ergebnisliste, sondern nur über die Vorrunden-Ergebnisliste. Die
  Leiter-Platzierung (Bahn 1 =
  höchster offener Platz, letzte Bahn = niedrigster offener Platz) bezieht sich dabei nur auf die Teams,
  die tatsächlich in der Leiter mitspielen (z.B. bei 9 Teams mit 1 Bye-Team: Plätze 1–8 unter den 8
  spielenden Teams, das Bye-Team bleibt außen vor).

**Ablauf pro Runde**:

- Auf jeder Bahn **außer der letzten**: Der Verlierer **bleibt auf der Bahn stehen** (Seitenwechsel
  TeamA/TeamB), der Sieger **springt eine Bahn nach oben** (Richtung Bahn 1) und spielt dort in der
  nächsten Runde gegen den dort wartenden (stehengebliebenen) Verlierer.
- Auf der **letzten (untersten) Bahn**: Der Verlierer scheidet endgültig aus und bekommt den
  **niedrigsten noch offenen Platz** der Gesamtwertung. Der Sieger springt (wie überall sonst) eine
  Bahn nach oben.
- Auf **Bahn 1**: Der Sieger bekommt den **höchsten noch offenen Platz** der Gesamtwertung und scheidet
  aus der Leiter aus (fertig platziert). Der Verlierer bleibt stehen (Seitenwechsel) und wartet auf den
  nächsten Herausforderer aus Runde+1.
- Danach wird eine Bahn weniger gebraucht (die bisher letzte Bahn entfällt) – die nächste Runde startet
  mit einer Bahn weniger.
- Das setzt sich fort, bis nur noch Bahn 1 übrig ist; das letzte Spiel dort vergibt gleichzeitig die
  beiden letzten noch offenen Plätze (Sieger den besseren, Verlierer den schlechteren).

**Anspiel & TeamA/TeamB-Zuordnung**:

- Runde 1: Das aus der Vorrunde besserplatzierte Team der Paarung ist TeamA und hat Anspiel.
  - Bei einer Gruppe ergibt sich das direkt aus der Rangfolge (z.B. bei 1 vs. 2 ist Team 1 = TeamA).
  - Bei zwei Gruppen (gleicher Rang, aber unterschiedliche Gruppen) über dieselbe Vergleichsreihenfolge
    wie bei 4.1: Punkte, Differenz, Stockpunkte, dann Team aus Gruppe A.
- Ab Runde 2: Das **stehengebliebene Team** (der Verlierer, der auf der Bahn bleibt) ist immer TeamA
  und hat Anspiel; der ankommende Herausforderer wird TeamB.

**Kehren-Anzahl**: Wie in der Vorrunde.

**Unentschieden**: Zählt wie eine Niederlage (kein Entscheid wie bei 4.1 nötig).

**Anzahl teilnehmender Paarungen (konfigurierbar)**: Der Nutzer legt fest, wie viele der bestplatzierten
Paare überhaupt an der Finalrunde teilnehmen – alle möglichen Paarungen, oder nur die besten X (Bahn 1
bis Bahn X in der ursprünglichen Nummerierung). Diese X Paare bilden eine **in sich geschlossene Leiter**:
Sie braucht immer exakt **X Runden**, um sich vollständig aufzulösen (jede Runde vergibt gleichzeitig
einen Platz von oben und einen von unten, bis alle 2X Teams platziert sind). Rundenanzahl und Anzahl der
teilnehmenden Paarungen sind bei 4.2 also **derselbe Wert** – anders als bei 4.3 (siehe dort), wo beides
unabhängig konfigurierbar ist, weil dort niemand ausscheidet.

**Anzahl platzierter Teams**: Bei X teilnehmenden Paaren (2X Teams) sind nach den X Runden **alle 2X
Teams** platziert (Plätze 1 bis 2X) – die Leiter ist danach vollständig aufgelöst, niemand bleibt
"hängen". Beispiel: 2 Gruppen à 9 Teams (18 gesamt), Nutzer wählt nur X=3 Paarungen → nur die 3
bestplatzierten Paare (6 Teams) nehmen teil und spielen in 3 Runden die Plätze 1–6 untereinander aus. Die
übrigen 12 Teams nehmen an dieser Finalrunde **gar nicht teil** (Grundsatz 3, Abschnitt 4) – sie bleiben
nur über die Vorrunden-Ergebnisliste sichtbar. Top- und Bottom-Platzierungen sind durch die
Ladder-Mechanik strukturell **gekoppelt** – das ist so gewollt.

**Rematch bei zwei Gruppen**: A- und B-Teams sind sich in der Vorrunde nicht begegnet (nur gruppenintern) –
Runde 1 ist daher immer ein neues Duell. In späteren Runden können zwei Teams **derselben** Gruppe in der
Ladder aufeinandertreffen (falls beide mehrfach gewinnen) – das ist **kein Problem**, muss also nicht
vermieden werden.

**Beispiel** (8 Teams, 4 Bahnen, 4 Runden – durchgerechnet und schlüssig; `*` = TeamA / hat Anspiel):

| Runde | Aktive Bahnen | Bahn 1 (TeamA\* vs. TeamB → Sieger-Platz) | letzte Bahn (TeamA\* vs. TeamB → Verlierer-Platz) |
| --- | --- | --- | --- |
| 1 | 4 (Bahn 1–4) | 1\* vs. 2 → Platz 1 | Bahn 4: 7\* vs. 8 → Platz 8 |
| 2 | 3 (Bahn 1–3) | Verlierer Bahn 1\* vs. Sieger Bahn 2 → Platz 2 | Bahn 3: Verlierer Bahn 3\* vs. Sieger Bahn 4 → Platz 7 |
| 3 | 2 (Bahn 1–2) | Verlierer Bahn 1\* vs. Sieger Bahn 2 → Platz 3 | Bahn 2: Verlierer Bahn 2\* vs. Sieger Bahn 3 → Platz 6 |
| 4 | 1 (Bahn 1) | Verlierer Bahn 1\* vs. Sieger Bahn 2 → Sieger = Platz 4, Verlierer = Platz 5 | – |

Anmerkung: "Bahn X" in Runde n+1 bezieht sich jeweils auf das Ergebnis von Bahn X aus Runde n. Bei zwei
Gruppen läuft der Mechanismus identisch, nur dass die Runde-1-Paarung A1\* vs. B1, A2\* vs. B2, usw. lautet.

### 4.3 Bahnenspiele

**Abgrenzung zu 4.2**: Kein Ausscheiden – alle teilnehmenden Bahnen bleiben über die gesamte Dauer aktiv,
keine Bahn entfällt. Dadurch kann die Leiter beliebig lange weiterlaufen; sie löst sich nicht von selbst
auf. Deshalb braucht 4.3 **zwei unabhängige Konfigurationswerte** (anders als 4.2, wo beide Werte
zwangsläufig identisch sind, siehe dort):

1. **Anzahl teilnehmender Paarungen (X)**: Wie viele der bestplatzierten Paare überhaupt an der
   Finalrunde teilnehmen – alle möglichen Paarungen, oder nur die besten X. Legt die Größe der Leiter
   fest (X Bahnen, 2X Teams).
2. **Rundenanzahl (R)**: Wie viele Runden gespielt werden – unabhängig von X wählbar.

**Start-Paarung**: Wie bei 4.2 – kann aus **einer oder zwei Gruppen** bestehen (gleiche Regeln zur
Paarung unter den X teilnehmenden Paaren, ungerader/ungleicher Mannschaftszahl und Anspiel/TeamA-
Bestimmung in Runde 1 wie dort).

**Bewegung pro Runde** (symmetrische Ladder statt "Verlierer bleibt stehen" wie bei 4.2):

- Auf **Bahn 1**: Der Sieger **bleibt** auf Bahn 1 (verteidigt die Position). Der Verlierer **steigt eine
  Bahn ab** (zu Bahn 2).
- Auf einer **mittleren Bahn**: Der Sieger **steigt eine Bahn auf** (Richtung Bahn 1), der Verlierer
  **steigt eine Bahn ab** (Richtung letzte Bahn).
- Auf der **letzten (untersten) Bahn**: Der Verlierer **bleibt** unten (kann nicht weiter absteigen). Der
  Sieger steigt eine Bahn auf.

**Anspiel & TeamA/TeamB-Zuordnung**: TeamA ist immer das Team, das von einer höheren Bahn in diese Bahn
**abgestiegen** ist; TeamB ist das Team, das von einer niedrigeren Bahn **aufgestiegen** ist.

- Bahn 1: Der Sieger, der dort bleibt, ist TeamA (Sonderfall "um null Bahnen abgestiegen").
- Letzte Bahn: Der Verlierer, der dort bleibt, ist **TeamB** (Sonderfall "um null Bahnen aufgestiegen") –
  der von oben ankommende Absteiger ist dort TeamA.
- Mittlere Bahnen: Der Absteiger von der Bahn darüber ist TeamA, der Aufsteiger von der Bahn darunter
  ist TeamB.

**Beispiel** (X=4 gewählte Paarungen, 8 Teams, 4 Bahnen; alle 4 Bahnen bleiben über alle Runden aktiv –
keine scheidet aus):

| Bahn | Runde 1 (TeamA\* vs. TeamB) | Runde 2 (TeamA\* vs. TeamB) |
| --- | --- | --- |
| 1 | 1\* vs. 2 | Sieger Bahn 1\* vs. Sieger Bahn 2 |
| 2 | 3\* vs. 4 | Verlierer Bahn 1\* vs. Sieger Bahn 3 |
| 3 | 5\* vs. 6 | Verlierer Bahn 2\* vs. Sieger Bahn 4 |
| 4 | 7\* vs. 8 | Verlierer Bahn 3\* vs. Verlierer Bahn 4 (bleibt = TeamB) |

**Ergebnisliste/Platzierung**: Nach der letzten gespielten Runde (R) wird die Platzierung **aller 2X
teilnehmenden Teams** aus ihrer dann aktuellen Bahn-Position abgeleitet: TeamA auf Bahn *k* → Platz
`2k-1`, TeamB auf Bahn *k* → Platz `2k` (für *k* = 1 bis X). Da bei 4.3 niemand ausscheidet, bekommen
**immer alle 2X teilnehmenden Teams** eine Platzierung – unabhängig davon, wie groß R ist. Nicht
teilnehmende Teams (falls X kleiner als die maximal mögliche Anzahl Paarungen) erscheinen gemäß
Grundsatz 3, Abschnitt 4 nicht in der Finalspiele-Ergebnisliste.

### 4.4 Page-PlayOff

Standard-Page-Playoff-System (4 Teams), auch international im Eisstocksport genutzt. **Quelle**: § 4.0.4
DSpO ([Deutscher Eisstock-Verband, DSpO-Download](https://www.eisstock-verband.com/download/2-03-0-dspo/))
– offizielle Regelung, kein Eigenbau. Offizielle Stufen-Bezeichnungen: **Ausscheidung**,
**Qualifikation 1**, **Qualifikation 2**, **Finale** (siehe Grundsatz 2, Abschnitt 4).

**Herkunft der 4 Teams** (laut § 4.0.4 DSpO):

- Bei **einer** Vorrundengruppe: Ränge 1–4 spielen weiter. **Qualifikation 1** = Platz 1 vs. Platz 2.
  **Ausscheidung** = Platz 3 vs. Platz 4.
- Bei **zwei** Vorrundengruppen: Ränge 1 und 2 jeder Gruppe spielen weiter. **Qualifikation 1** = die
  beiden Erstplatzierten (A1 vs. B1). **Ausscheidung** = die beiden Zweitplatzierten (A2 vs. B2).

**Ablauf**: **Ausscheidung** und **Qualifikation 1** werden zuerst gespielt, **gleichzeitig und
unabhängig voneinander** (analog zur Vorrunde/4.1 – keine Abhängigkeit zwischen den beiden). Erst danach
folgen Qualifikation 2 und das Finale, die jeweils vom Ergebnis der vorherigen Stufe abhängen:

1. **Ausscheidung**: Verlierer scheidet sofort aus → **Platz 4**. Sieger → **Qualifikation 2**.
1. **Qualifikation 1**: Sieger ist bereits als **erster Finalteilnehmer** qualifiziert (wartet auf
   Finale). Verlierer → **Qualifikation 2**.
1. **Qualifikation 2**: Sieger Ausscheidung vs. Verlierer Qualifikation 1 → Verlierer = **Platz 3**,
   Sieger → **Finale**.
1. **Finale**: Sieger Qualifikation 1 vs. Sieger Qualifikation 2 → Sieger = **Platz 1**, Verlierer =
   **Platz 2**.

**Anspiel** (laut § 4.0.4 DSpO – **abweichend** von 4.1–4.3!):

- Die laut Vorrunde **besserplatzierte Mannschaft darf entscheiden**, wer die erste Kehre anspielt (kein
  automatisches "TeamA = besserplatziert" wie bei 4.1–4.3, sondern ein **Wahlrecht**).
- Sind die beiden Vorrundenplätze **gleichwertig** (z.B. A1 vs. B1 bei zwei Gruppen – nicht direkt
  vergleichbar), entscheidet das **Los**. Der Losgewinner hat dann das Anspiel-Wahlrecht.
- 4.4 übernimmt diese offizielle Regel **exakt so** (Wahlrecht + Los). 4.1–4.3 bleiben unverändert bei der
  automatischen TeamA-Zuordnung – die beiden Konzepte haben nichts miteinander zu tun und werden nicht
  angeglichen.
- **Hinweis für Abschnitt 7 (Domänenmodell)**: Da das Anspiel hier auf einer **Entscheidung** beruht (nicht
  automatisch berechenbar), braucht 4.4 dafür eine UI-Erfassung (wer entscheidet sich für welches Anspiel;
  bei Gleichwertigkeit ggf. eine "Los"-Erfassung) – anders als bei 4.1–4.3, wo `IsTeamA_Starting` komplett
  aus den Daten hergeleitet werden kann.

**Kehren-Anzahl**: Wie in der Vorrunde.

**Spielanzahl pro Stufe** (laut § 4.0.4 DSpO):

- Ausscheidung, Qualifikation 1, Qualifikation 2: jeweils **ein** Spiel.
- Finale bei Damen/Herren auf Eis: **zwei** Spiele (nach dem ersten Spiel wechselt das Anspiel).
- Finale in allen anderen Wettbewerben (Sommer, Jugend, ...): **ein** Spiel.

**Unentschieden** (laut § 4.0.4 DSpO):

- Ausscheidung, Qualifikation 1, Qualifikation 2 sowie das **einspielige** Finale: Bei Gleichstand nach
  6 Kehren gewinnt automatisch die laut Vorrunde **besserplatzierte Mannschaft** – **kein** "Finaler
  Entscheid" nötig.
- Nur beim **zweispieligen** Finale (Damen/Herren, Eis): Steht es nach beiden Spielen bei Gewinnpunkten
  *und* Differenz gleich, wird der **"Finale Entscheid" nach § 4.0.5 DSpO** gespielt – siehe Abschnitt 5.1
  für das vollständige Verfahren.
- 4.4 wird exakt so umgesetzt, wie in der DSpO beschrieben (gestufte Eskalation). 4.1 bleibt unverändert
  beim bisherigen "jeder Gleichstand → sofort Finaler Entscheid" – 4.1–4.3 haben mit Page-PlayOff nichts
  zu tun und werden nicht angeglichen.

**Bahneinteilung**: Die DSpO gibt dazu in "10.3.2" nur eine **unverbindliche Empfehlung** – für StockAppV2
gilt daher wie bei 4.1: Die Bahn ist **frei durch den Nutzer wählbar**, für jede der vier Stufen
(Ausscheidung, Qualifikation 1, Qualifikation 2, Finale) einzeln.

**Ergebnisliste**: Nur die 4 teilnehmenden Teams bekommen eine Platzierung (1.–4.); alle anderen Teams
erscheinen nicht in der Page-Playoff-Ergebnisliste (Grundsatz 3, Abschnitt 4).

### 4.5 KO-Runde

> ⏸ **Zurückgestellt**: Die KO-Runde setzt – anders als 4.1–4.4 – keine abgeschlossene Gruppenphase voraus
> und ist damit kein "Finalspiel" im Sinne von Abschnitt 1 (dort explizit definiert als Turnierabschnitt
> *nach* der Gruppenphase). Sie gehört vermutlich **nicht in dieses Fachkonzept**, sondern muss als
> eigenständiges Konzept separat betrachtet werden – u.a. weil dann bei einem Turnier pro Runde/Abschnitt
> einzeln festgelegt werden müsste, welcher Modus überhaupt gilt. Der bisherige Stand unten bleibt als
> Ausgangspunkt für diese spätere, separate Betrachtung stehen, wird hier aber nicht weiter vertieft.

**Grundprinzip**: Echtes K.o.-System (Sechzehntelfinale/1⁄16, Achtelfinale/1⁄8, Viertelfinale/1⁄4,
Halbfinale/1⁄2, Finale) – wer eine Begegnung verliert, scheidet **sofort und endgültig** aus, keine
zweite Chance. Das unterscheidet die KO-Runde von 4.2 (dort verliert man nicht sofort endgültig, man
bleibt auf der Bahn stehen) und von 4.4 (dort bekommt der Qualifikation-1-Verlierer über Qualifikation 2
noch eine zweite Chance).

**Keine Gruppenphase zwingend erforderlich**: Anders als 4.1–4.4 setzt die KO-Runde **keine**
abgeschlossene Gruppenphase voraus – sie kann auch als eigenständiges, alleinstehendes Turnierformat ganz
ohne vorherige Vorrunde gespielt werden. Das weicht vom bisherigen Grundsatz in Abschnitt 3 ab (bislang:
"Finalspiele setzen eine abgeschlossene Gruppenphase voraus").

**Paarungsbildung/Setzung**: Komplett frei – keine feste Algorithmik und keine Setzungs-Empfehlung (wie
z.B. "bestplatziert gegen schlechtestplatziert") wie bei 4.1–4.4. Paarungen können ausgelost werden, oder
– falls eine Gruppenphase vorausgegangen ist – nach einem Muster aus deren Ergebnis gebildet werden. Der
Nutzer legt das jeweils vollständig selbst fest.

**Teilnehmerzahl**: Es ist **nur eine gerade Anzahl** an Mannschaften möglich – kein automatisches
Freilos/Bye-Handling wie bei 4.1–4.3. Bei ungerader Anzahl kann die KO-Runde nicht gestartet werden.

**Platzierung früh ausgeschiedener Teams**: Alle Teams, die in derselben Runde ausscheiden (z.B. alle
Achtelfinal-Verlierer), bekommen eine **gemeinsame Platzierungs-Stufe** – keine individuelle
Differenzierung untereinander.

**Spielplan-Charakter**: Da Paarungsbildung, Bahnzuteilung, Kehrenanzahl und Spielanzahl nicht vorab
algorithmisch bestimmbar sind, wird die KO-Runde als **eigene Art Spielplan** umgesetzt – ähnlich wie
`gpf.json` eine von Hand kuratierte Lookup-Tabelle ist (siehe CLAUDE.md), nicht wie der automatisch
hergeleitete Ablauf bei 4.1–4.4. Konkret vom Nutzer frei festzulegen:

- Bahn je Paarung
- Kehren-Anzahl
- Spielanzahl je Begegnung (siehe "Best-of-N" unten)
- TeamA/TeamB je Begegnung (der Nutzer legt das bei der Paarungsbildung fest)

**Anspiel**: Wie bei 4.1–4.3 gilt Grundsatz 1 (Abschnitt 4) – **TeamA hat immer Anspiel**. Da die
TeamA/TeamB-Zuordnung bei 4.5 manuell im freien Spielplan erfolgt (nicht algorithmisch aus der
Vorrunden-Leistung), legt der Nutzer damit faktisch auch das Anspiel fest.

**Best-of-N**: Eine Begegnung kann statt in einem einzelnen Spiel auch in einer Serie entschieden werden
(Best-of-3, Best-of-5, Best-of-7) – wer zuerst die Mehrheit der Spiele gewinnt, gewinnt die Begegnung.
Neu gegenüber 4.1–4.4, wo jede Begegnung höchstens 2 Spiele hat (4.4-Finale, fest 2 Spiele).

**Unentschieden**:

- **Bei einem Spiel** (Einzelspiel-Begegnung, oder jedes einzelne Spiel innerhalb einer Serie): Steht es
  nach regulärer Wertung unentschieden, wird der **"Finale Entscheid"** gespielt (siehe Abschnitt 5.1).
- **Bei einer Begegnung mit genau zwei Spielen** (analog zum zweispieligen Finale bei 4.4): Steht die
  Begegnung nach beiden Spielen weiterhin unentschieden, entscheidet:
  1. Die höhere **Gesamt-Stockpunktezahl** über beide Spiele.
  2. Ist auch das gleich: die Mannschaft, die die **zuletzt entschiedene Kehre** (über beide Spiele
     hinweg, von hinten gezählt) gewonnen hat. Eine Kehre mit 0:0-Ergebnis gilt dabei als **nicht
     entschieden** – dann zählt die vorletzte Kehre, dann die drittletzte usw., bis eine entschiedene
     Kehre gefunden ist (Prinzip analog zu IER § 395 Hinweis b).

Noch offen: ob Best-of-N pro Begegnung individuell oder turnierweit einheitlich konfiguriert wird, und ob
das Anspiel zwischen den Spielen einer Serie wechselt – siehe Abschnitt 8.

## 5. Ablauf & Regeln (allgemein, art-übergreifend)

### 5.1 Finaler Entscheid (§ 4.0.5 DSpO)

Offizielles Verfahren zur Auflösung eines Unentschiedens **ohne** zusätzliche reguläre Kehre. Wird von
4.1 (bei jedem Gleichstand) und 4.4 (nur als letzte Eskalationsstufe beim zweispieligen Finale) verwendet
– siehe dort für den jeweiligen Auslöser.

**StockTV**: Der Finale Entscheid wird **ohne StockTV-Anbindung** umgesetzt – reine Eingabefelder
innerhalb von StockApp, die nicht von StockTV befüllt werden.

**Teilnehmer**: Die vier Spieler jeder Mannschaft, die im **letzten Finalspiel** eingesetzt waren.

**Bahn**: Der Finale Entscheid findet auf **derselben Bahn** statt wie das unentschiedene Finalspiel
selbst.

**Vorbereitung**:

- Jede Mannschaft legt für ihre vier Spieler eine **Startreihenfolge (1–4)** fest.
- Der Wettbewerbsleiter notiert diese auf einem eigenen **Wertungsblatt für den Finalen Entscheid**.
- **Anspiel** hat die Mannschaft, die im letzten Finalspiel in der **ersten Kehre** das Anspiel hatte.

**Ablauf**: Abwechselnd je ein Versuch, in der festgelegten Reihenfolge – TeamA Spieler 1, TeamB
Spieler 1, TeamA Spieler 2, TeamB Spieler 2, usw. Alle vier Spieler jeder Mannschaft müssen je einen
Versuch abgeben.

**Zwei Varianten** (Variante 1 hat Vorrang, sobald mittlere Zielringe vorhanden/eingezeichnet sind):

- **Variante 1 – mit Zielringen** (analog IER § 503, Zielwettbewerb): Wurf auf die mittleren Zielringe,
  Daube liegt bei jedem Versuch auf dem Mittelkreuz. Punktewertung pro Versuch; ein außer der Reihe
  gemachter oder vergessener Versuch zählt **0 Punkte**. Der Spielerstock wird nach Ergebnisfeststellung
  vom Schiedsrichter aus dem Zielfeld entfernt. Summe der 4 Versuche pro Mannschaft – **höhere
  Punktesumme gewinnt**.
- **Variante 2 – ohne Zielringe**: Abstand Spielerstock–Daube wird gemessen (verlässt die Daube durch den
  Versuch das Mittelkreuz, wird sie zurückgelegt, IER § 424 angewendet, dann gemessen). Anliegen des
  Stockes an der Daube = 0,00 cm; ein außer der Reihe gemachter oder vergessener Versuch sowie generell
  jeder Versuch, der die max. Entfernung überschreitet (Stock erreicht/verlässt das Feld), zählt **max.
  130,00 cm**. Summe der 4 Entfernungen pro Mannschaft – **niedrigere Entfernungssumme gewinnt**
  (umgekehrte Vergleichsrichtung gegenüber Variante 1!).

**Weiterhin unentschieden nach je 4 Versuchen**: Fortsetzung im **1-gegen-1-Duell** – die vorher
festgelegte Reihenfolge muss nicht mehr eingehalten werden, aber kein Spieler darf zweimal hintereinander
für seine Mannschaft antreten. Sobald eine Mannschaft in einem Duell besser abschneidet (mehr Punkte
bzw. kürzere Entfernung), hat sie den Finalen Entscheid gewonnen.

**Ergebnis-Verwertung**: Der Sieger des Finalen Entscheids bekommt **1 zusätzlichen Stockpunkt** in der
Endwertung des betreffenden Finalspiels – er **löst den bestehenden Stockpunkte-Gleichstand auf**, ist
also kein separates/eigenständiges Spielergebnis. IER § 402 und § 454 (Zielwettbewerb-Regeln) gelten
dabei vollinhaltlich. Für die Erfassung folgt daraus: Es braucht eine Erfassung pro Spieler/Versuch
(mindestens Summen je Mannschaft und Variante), keine zwei einfachen Zahlenfelder (siehe Abschnitt 7).

## 6. Sonderfälle

### 6.1 Aufgabe während eines Finalspiels

Eine Mannschaft kann ein laufendes Finalspiel **aufgeben**, ohne dass alle Kehren gespielt werden – die
Gegner-Mannschaft erhält den Sieg. Betrifft grundsätzlich alle Finalspiele-Arten (4.1–4.4), da alle auf
`IGame` basieren.

**Wertung**: Das bis zum Zeitpunkt der Aufgabe erspielte Kehren-Ergebnis **ist das Endergebnis** und
bleibt so bestehen – es wird nicht zurückgesetzt oder überschrieben. Die nicht mehr gespielten Kehren
entfallen einfach.

**Turnierverlauf**: Läuft danach normal weiter, genau wie nach jedem regulären Sieg (z.B. bei 4.2/4.3
rückt die Gegner-Mannschaft wie üblich eine Bahn auf).

**Finaler Entscheid** (5.1): Auch dort kann eine Mannschaft aufgeben, ohne dass alle vier
Spieler-Versuche abgegeben werden müssen – gleiches Prinzip wie oben.

## 7. Verhältnis zum bestehenden Domänenmodell

*(Wie fügt sich das Konzept in `ITeamBewerb`/`ITeam`/`IGame` bzw. `IZielBewerb` ein?
Neue Entität nötig, oder Erweiterung bestehender?)*

Die folgenden Rechercheergebnisse halten den **Ist-Stand des Codes** fest (Stand: 2026-08-20) und bilden
die Grundlage für die anschließende Design-Entscheidung.

### Rechercheergebnisse (Ist-Stand, keine Design-Entscheidung)

**Zentraler Befund: Gruppenübergreifende Spiele sind im Domänenmodell strukturell nicht vorgesehen.**
Betrifft direkt 4.1 (zwingend zwei Gruppen) sowie 4.2/4.3 bei zwei Gruppen.

- `IGame.TeamA`/`TeamB` sind zwar generische `ITeam`-Referenzen, aber ein Spiel wird **pro Team
  gespeichert**, nicht pro `ITeamBewerb`: `Team._games` (`Team.cs:109`), und `TeamBewerb.Games` ist nur
  `_teams.SelectMany(t => t.Games)` (`TeamBewerb.cs:273`) – abgeleitet aus den eigenen Teams *dieser
  einen* Gruppe.
- `IContainerTeamBewerbe` (`ContainerTeamBewerbe.cs:7-55`) hat **keine eigene Games-Collection**, nur
  eine Liste von `ITeamBewerb`. Es gibt aktuell keinen Ort im Domänenmodell, an dem ein
  gruppenübergreifendes Spiel (z.B. A1 vs. B1 bei 4.1) verankert werden könnte.
- **Noch härter in der XML-Persistenz**: `SerialisableGame` referenziert Teams nur über
  `StartnumberTeamA`/`StartnumberTeamB` (int), aufgelöst beim Laden ausschließlich **innerhalb derselben**
  `SerialisableTeamBewerb` (`SerialisableTeamBewerb.cs:109-110`). Zwei Gruppen haben aber je eine eigene
  Startnummer 1, 2, 3, ... – ein Cross-Gruppen-Spiel lässt sich mit diesem Schema nicht eindeutig
  referenzieren, unabhängig vom gewählten Speicherformat.
- `SerialisableContainerTeamBewerbe` transportiert ebenfalls nur die Liste der Gruppen, sonst nichts
  (`SerialisableContainerTeamBewerbe.cs:9-30`) – auch auf Container-Ebene existiert kein Feld, an das sich
  ein Finalspiele-Block anhängen ließe.

**Zum Leitsatz "Live-Ansicht" (Abschnitt 1):**

- `LiveResultsTeamViewModel` ist hart an genau ein `ITeamBewerb` gebunden
  (`LiveResultsTeamViewModel.cs:16,31-33`) und zeigt `_teamBewerb.GetTeamsRanked(IsLive)` – die normale
  Spielpunkte/Stockpunkte-Rangliste (`TeamRankingComparer`).
- Das passt strukturell nur zu 4.1 (dort ist das Endergebnis ebenfalls wieder eine Punkte-Rangliste aus
  Direktvergleichs-Spielen). Für 4.2/4.3 (Leiter-Position auf einer Bahn) und 4.4 (Stufen: Ausscheidung/
  Qualifikation 1/Qualifikation 2/Finale) ist das vorhandene Ranking-Modell fachlich nicht anwendbar –
  dort gibt es keine "Rangliste nach Punkten über mehrere Spiele", sondern einen strukturellen Zustand
  (welche Bahn/welche Stufe). Die Live-Ansicht bräuchte für diese Arten keine Erweiterung, sondern eine
  grundsätzlich andere Darstellungslogik.

**Zum Leitsatz "StockTV-Fähigkeit" (Abschnitt 1):**

- `StockTVSettings.GetSettings()` (`StockTVSettings.cs:148-163`) sendet pro Bahn ein kompaktes
  Byte-Telegramm (Bahn, Spielgruppe, Modus, Punkte/Kehren-Konfig) – bestätigt im Code, dass StockTV
  tatsächlich nur bahn-bezogene Konfiguration + Ergebnisse braucht, keinen kompletten Spielplan.

**Zum Finalen Entscheid (5.1):**

- Da das Ergebnis eines Finalen Entscheids auf einer Erfassung pro Spieler/Versuch beruht (Summen je
  Mannschaft und Variante, siehe 5.1), braucht die Domänenmodellierung dafür eine eigene, kleine
  Datenstruktur – keine zwei einfachen Zahlenfelder.

**Sonstiges, evtl. nützlich für die spätere Modellierung:**

- `IGameplan` (gpf.json-Ebene) hat bereits ein Präzedenzmuster für "Art"-Flags: `IsVergleich`, `IsSplit`
  (`Gameplan.cs:24-25`) – strukturell unabhängig von Finalspielen, aber ein Beispiel, wie das bestehende
  Modell "Spielplan-Art" schon anderswo als einfaches Bool-Flag löst.

**Einordnung**: Die Rechercheergebnisse bestätigen den zentralen Design-Punkt – insbesondere 4.1–4.3 mit
zwei Gruppen brauchen eine im Modell bisher nicht existierende Möglichkeit, Spiele *außerhalb* einer
einzelnen `ITeamBewerb` zu verankern. Die Entscheidung dazu steht im folgenden Abschnitt.

### Design-Entscheidung

Gruppenübergreifende Spiele werden **nicht** in das bestehende `Team.Games`/`TeamBewerb.Games`-Modell
integriert (das würde Finalspiele-Ergebnisse ungewollt in die Gruppenphasen-Wertung einfließen lassen,
siehe Grundsatz 3), sondern über eine neue, eigenständige Struktur abgebildet:

**Schicht 1 – `IGame` bleibt unverändert**: Ein einzelnes physisches Spiel (Spielstand, Kehren, Bahn,
Anspiel, StockTV-fähig) wird weiterhin über `IGame` abgebildet, genau wie in der Gruppenphase (vgl.
Abschnitt 6.1: alle Finalspiele-Arten basieren auf `IGame`).

**Schicht 2 – neu: `IFinalBegegnung`**: Eine Begegnung zwischen zwei Teams besteht aus **einem oder zwei**
`IGame`s (zwei nur beim zweispieligen Finale von 4.4) sowie optional einem `IFinalerEntscheid` (eigene,
kleine Struktur: Werte pro Spieler/Versuch, Variante 1/2 gemäß 5.1). Daraus werden Sieger/Verlierer der
Begegnung abgeleitet.

**Schicht 3 – art-spezifische Container**: Je Finalspiele-Art ein eigener Typ, der eine gemeinsame, dünne
Schnittstelle `IFinalspiele` implementiert (Art, Liste der `IFinalBegegnung`, `GetFinalspieleRanking()`):

- `PlatzierungsspieleRunde` (4.1) – flache Liste von Begegnungen.
- `LadderFinalspiele` (4.2/4.3) – gemeinsame Bahnen-Leiter-Mechanik, unterscheiden sich nur in
  Bewegungsregel und Terminierung.
- `PagePlayoff` (4.4) – vier benannte Stufen-Slots (Ausscheidung, Qualifikation 1, Qualifikation 2,
  Finale).

Bewusst **keine** gemeinsame, generische Struktur für alle vier Arten: Die Arten sind strukturell zu
unterschiedlich (flache Welle / Leiter-Runden / benannte Stufen); eine generische Lösung müsste
Leiter-Position, Stufen-Namen und flache Ränge in einer unscharfen Zusatzdaten-Struktur unterbringen.

**Verankerung im Objektgraphen**: Als neue, optionale Property auf `IContainerTeamBewerbe` (nicht auf
`ITurnier`) – passt zu "Finalspiele gibt es nur im Teambewerb" (Abschnitt 3) und zu "genau eine
Finalspiele-Art pro Turnier" (Abschnitt 3, Auslöser): ein einzelnes nullable Property, keine Liste.

**Team-Referenzierung**: Im Domänenmodell weiterhin direkte `ITeam`-Objektreferenzen (wie bei `IGame`
bereits üblich) – funktioniert gruppenübergreifend problemlos. In der **XML-Persistenz** reicht die
bisherige alleinige `StartNumber`-Referenz dagegen nicht (mehrdeutig über Gruppen hinweg, siehe
Rechercheergebnisse oben) – dort wird ein zusammengesetzter Schlüssel `(TeamBewerbID, StartNumber)`
benötigt, analog zum bereits bestehenden Muster mit `SpielGruppe`.

**`RoundOfGame`/Nummerierung**: Gemäß Grundsatz 2 (Abschnitt 4) werden die `IGame`-Felder `RoundOfGame`,
`GameNumber`, `GameNumberOverAll` bei Finalspielen nur mit technischen Platzhalterwerten befüllt – die
fachlich relevante Position (Leiter-Runde, Stufe, Gesamt-Platz) lebt in den Schichten 2/3, nicht in
diesen Feldern.

## 8. Offene Fragen

**4.5 KO-Runde**: Wie in Abschnitt 4.5 begründet, ist die KO-Runde als Ganzes zurückgestellt und gehört
vermutlich nicht in dieses Fachkonzept, da sie keine Gruppenphase voraussetzt. Die dort bereits
gesammelten offenen Punkte (Grundsatz 2/3 ohne Gruppenphase, Best-of-N-Details, StockTV-Fähigkeit) bleiben
für eine spätere, separate Betrachtung notiert.

**Ergebnisliste/Ausdruck-Layouts und Live-Ergebnis-Anzeige** (alle Finalspiele-Arten, 4.1–4.4): Offen ist
das konkrete Layout – wie Gruppenphasen- und Finalspiele-Ergebnisse gemeinsam dargestellt werden (siehe
Grundsatz 3, Abschnitt 4, und 4.1 "Ergebnisliste/Ausdruck") – und wie die Live-Ansicht mit den
unterschiedlichen Strukturen der einzelnen Arten umgeht (Leiter-Position bei 4.2/4.3, Stufen bei 4.4,
siehe Leitsatz "Live-Ansicht", Abschnitt 1). Das wird erst bei der Umsetzung festgelegt, nicht in diesem
Fachkonzept.
