# Funktionales Dokument (DE)

## 1. Zusammenfassung

YepPet ist eine pet-friendly Plattform, die darauf ausgerichtet ist, Orte, Unterkuenfte und Services zu entdecken, die Haustiere akzeptieren.
Im aktuellen Stand wird das Produkt mit einer Angular-Webanwendung und simulierten Daten validiert, bevor ein reales Backend eingebunden wird.

Der aktuelle funktionale Fokus ist:

- Entdeckung pet-friendly Orte
- klare Navigation zwischen Startseite, Ergebnissen, Detailseite und Favoriten
- Filterung nach Stadt, Typ, Haustier und Suchtext
- erste Kartenunterstuetzung innerhalb der Feature `places`
- Hilfe und Kontakt als informative Ebenen

## 2. Aktueller Umfang

Enthaelt:

- funktionale Startseite
- Navigation nach `places`
- Ortsliste mit Filtern
- Detailansicht eines Orts
- Fake-Favoriten
- `Hilfe`
- `Kontakt`
- `permissions` als getrennte Ansicht ausserhalb des oeffentlichen Hauptflusses
- funktionale Karte in `places` und `place detail`

Nicht im Umfang dieses Dokuments:

- reales Backend
- Persistenz der Favoriten
- echte authentifizierte Benutzer
- echte Berechtigungen
- externe Integrationen von Drittanbietern
- vollstaendige Mehrsprachigkeit

## 3. Akteure

Aktuelle Akteure:

- `Oeffentlicher Benutzer`

Fuer spaeter vorgesehene Akteure:

- `Authentifizierter Benutzer`
- `Benutzer mit internen Berechtigungen`

## 4. Aktuelle funktionale Domäne

Hauptelemente:

- `Place`
- `Favorite`
- `City`
- `PlaceFilters`

Funktionale Beziehungen:

- eine Stadt kann viele Orte haben
- ein Ort kann Hunde, Katzen oder beide akzeptieren
- ein Benutzer kann viele Favoriten haben
- ein Filter kann Orte nach Stadt, Typ, Haustier und Suche einschränken

## 5. Funktionale UML

### 5.1 Systemkontext

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart LR</span>
  <span style="color:#93c5fd;">U[Oeffentlicher Benutzer]</span> --&gt;|<span style="color:#fcd34d;">Browser</span>| <span style="color:#c4b5fd;">W[YepPet Web]</span>
  <span style="color:#c4b5fd;">W</span> --&gt;|<span style="color:#86efac;">Fake-Daten</span>| <span style="color:#86efac;">M[(Mocks)]</span>
  <span style="color:#c4b5fd;">W</span> --&gt;|<span style="color:#67e8f9;">Kartenergebnisse</span>| <span style="color:#67e8f9;">MAP[Places Map]</span>
  <span style="color:#c4b5fd;">W</span> --&gt;|<span style="color:#f9a8d4;">Information</span>| <span style="color:#f9a8d4;">HELP[Hilfe / Kontakt]</span></code></pre>

Zusammenfassung des Diagramms:

- es zeigt die aeusserste funktionale Sicht auf das System
- der Benutzer konsumiert eine Webanwendung, die aktuell mit simulierten Daten arbeitet
- die Karte ist Teil des Sucherlebnisses und kein getrenntes System
- `Hilfe` und `Kontakt` sind informative Stuetzpunkte des Produkts

### 5.2 Akteure und Zugriffe

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart LR</span>
  <span style="color:#93c5fd;">PUB[[Oeffentlicher Benutzer]]</span>
  <span style="color:#f9a8d4;">AUTH[[Authentifizierter Benutzer]]</span>
  <span style="color:#fcd34d;">DEV[[Benutzer mit Berechtigungen]]</span>

  <span style="color:#93c5fd;">PUB</span> --&gt; <span style="color:#c4b5fd;">HOME[Startseite]</span>
  <span style="color:#93c5fd;">PUB</span> --&gt; <span style="color:#c4b5fd;">PLACES[Places]</span>
  <span style="color:#93c5fd;">PUB</span> --&gt; <span style="color:#c4b5fd;">DETAIL[Ort-Detail]</span>
  <span style="color:#93c5fd;">PUB</span> --&gt; <span style="color:#c4b5fd;">FAV[Fake-Favoriten]</span>
  <span style="color:#93c5fd;">PUB</span> --&gt; <span style="color:#c4b5fd;">HELP[Hilfe / Kontakt]</span>

  <span style="color:#f9a8d4;">AUTH</span> -. Zukunft .-&gt; <span style="color:#c4b5fd;">FAV</span>
  <span style="color:#fcd34d;">DEV</span> -. Zukunft .-&gt; <span style="color:#c4b5fd;">PERM[Entwicklerbereich / Permissions]</span></code></pre>

Zusammenfassung des Diagramms:

- es spiegelt den aktuellen funktionalen Stand und die naechste Zukunft wider
- heute ist der Hauptfluss oeffentlich
- `favorites` existiert bereits mit Fake-Status, wird spaeter aber an authentifizierte Benutzer gebunden
- der Bereich `permissions` bleibt fuer interne oder berechtigte Benutzer reserviert

### 5.3 Hauptanwendungsfaelle

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart LR</span>
  <span style="color:#93c5fd;">U[[Oeffentlicher Benutzer]]</span>

  <span style="color:#93c5fd;">U</span> --&gt; <span style="color:#c4b5fd;">UC01([UC-01 Startseite sehen])</span>
  <span style="color:#93c5fd;">U</span> --&gt; <span style="color:#c4b5fd;">UC02([UC-02 Orte suchen])</span>
  <span style="color:#93c5fd;">U</span> --&gt; <span style="color:#c4b5fd;">UC03([UC-03 Ort-Detail ansehen])</span>
  <span style="color:#93c5fd;">U</span> --&gt; <span style="color:#c4b5fd;">UC04([UC-04 Favoriten speichern])</span>
  <span style="color:#93c5fd;">U</span> --&gt; <span style="color:#c4b5fd;">UC05([UC-05 Hilfe aufrufen])</span>
  <span style="color:#93c5fd;">U</span> --&gt; <span style="color:#c4b5fd;">UC06([UC-06 Kontakt aufnehmen])</span></code></pre>

Zusammenfassung des Diagramms:

- es fasst die fuer den oeffentlichen Benutzer sichtbaren Funktionen zusammen
- die aktuellen Anwendungsfaelle konzentrieren sich auf Entdeckung, Detailansicht und Speichern von Orten
- `Hilfe` und `Kontakt` sind Informationswege, keine zentralen Geschaeftsfluesse

### 5.4 Hauptnavigation des Produkts

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart LR</span>
  <span style="color:#93c5fd;">HOME[Startseite]</span> --&gt; <span style="color:#c4b5fd;">PLACES[Places]</span>
  <span style="color:#93c5fd;">HOME</span> --&gt; <span style="color:#c4b5fd;">HELP[So funktioniert es]</span>
  <span style="color:#93c5fd;">HOME</span> --&gt; <span style="color:#c4b5fd;">CONTACT[Kontakt]</span>
  <span style="color:#c4b5fd;">PLACES</span> --&gt; <span style="color:#67e8f9;">DETAIL[Ort-Detail]</span>
  <span style="color:#c4b5fd;">PLACES</span> --&gt; <span style="color:#86efac;">FAV[Favoriten]</span>
  <span style="color:#67e8f9;">DETAIL</span> --&gt; <span style="color:#86efac;">FAV</span></code></pre>

Zusammenfassung des Diagramms:

- es repraesentiert die funktionale Hauptnavigation, die bereits ausprobiert werden kann
- `Startseite` ist der Einstiegspunkt
- `Places` ist der funktionale Kern des Produkts
- `Ort-Detail` und `Favoriten` schliessen den funktionalen Zyklus

### 5.5 Funktionaler Entdeckungsfluss

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart TD</span>
  <span style="color:#93c5fd;">A[Startseite betreten]</span> --&gt; <span style="color:#c4b5fd;">B[CTA, Stadt oder Chip auswaehlen]</span>
  <span style="color:#c4b5fd;">B</span> --&gt; <span style="color:#86efac;">C[Places mit Filtern oeffnen]</span>
  <span style="color:#86efac;">C</span> --&gt; <span style="color:#67e8f9;">D[Karte und Liste sehen]</span>
  <span style="color:#67e8f9;">D</span> --&gt; <span style="color:#fcd34d;">E[Zum Detail wechseln]</span>
  <span style="color:#fcd34d;">E</span> --&gt; <span style="color:#f9a8d4;">F[Als Favorit speichern]</span></code></pre>

Zusammenfassung des Diagramms:

- es beschreibt die Hauptreise des Produkts im aktuellen Zustand
- die `Startseite` fungiert als Eingang zu `places`
- `places` ist der funktionale Kern, in dem Filter, Karte und Liste zusammenkommen
- vom Detail aus kann die funktionale Aktion des Speicherns abgeschlossen werden

### 5.6 Funktionaler Filter- und Kartenfluss

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart TD</span>
  <span style="color:#93c5fd;">A[Stadt, Typ, Haustier oder Suche definieren]</span>
  <span style="color:#93c5fd;">A</span> --&gt; <span style="color:#c4b5fd;">B[Query-Parameter aktualisieren]</span>
  <span style="color:#c4b5fd;">B</span> --&gt; <span style="color:#86efac;">C[Simulierte Orte filtern]</span>
  <span style="color:#86efac;">C</span> --&gt; <span style="color:#67e8f9;">D[Karte aktualisieren]</span>
  <span style="color:#86efac;">C</span> --&gt; <span style="color:#fcd34d;">E[Liste aktualisieren]</span>
  <span style="color:#fcd34d;">E</span> --&gt; <span style="color:#f9a8d4;">F[Ausgewaehlte Filter anzeigen]</span></code></pre>

Zusammenfassung des Diagramms:

- es zeigt, wie sich eine Filteraenderung auf die gesamte `places`-Seite auswirkt
- die Query-Parameter sind die funktionale Quelle des aktuellen Suchzustands
- Karte und Liste laufen nicht getrennt, sondern reagieren auf dieselbe Filtermenge

### 5.7 Funktionaler Favoritenfluss

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart TD</span>
  <span style="color:#93c5fd;">A[Ort ist in Liste oder Detail sichtbar]</span> --&gt; <span style="color:#c4b5fd;">B[Speichern druecken]</span>
  <span style="color:#c4b5fd;">B</span> --&gt; <span style="color:#86efac;">C[Fake-Status der Favoriten aktualisieren]</span>
  <span style="color:#86efac;">C</span> --&gt; <span style="color:#67e8f9;">D[Aktiven Button spiegeln]</span>
  <span style="color:#67e8f9;">D</span> --&gt; <span style="color:#fcd34d;">E[Ort in Favoriten anzeigen]</span></code></pre>

Zusammenfassung des Diagramms:

- der Favoritenfluss kann bereits Ende-zu-Ende getestet werden
- der Status wird noch nicht persistiert, aber die UX simuliert bereits das reale Verhalten
- dasselbe Muster kann spaeter mit authentifiziertem Backend wiederverwendet werden

### 5.8 Funktionaler Hilfefluss

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart TD</span>
  <span style="color:#93c5fd;">A[Hilfemenue oeffnen]</span> --&gt; <span style="color:#c4b5fd;">B{Option}</span>
  <span style="color:#c4b5fd;">B</span> --&gt;|So funktioniert es| <span style="color:#86efac;">C[Zur erklaerenden Sektion der Startseite gehen]</span>
  <span style="color:#c4b5fd;">B</span> --&gt;|Kontakt| <span style="color:#67e8f9;">D[Zur Kontaktseite gehen]</span>
  <span style="color:#86efac;">C</span> --&gt; <span style="color:#f9a8d4;">E[Dropdown schliessen]</span>
  <span style="color:#67e8f9;">D</span> --&gt; <span style="color:#f9a8d4;">E</span></code></pre>

Zusammenfassung des Diagramms:

- `Hilfe` dient als sekundaerer Informationseinstieg
- das Dropdown konkurriert nicht mit dem primaeren CTA der Startseite
- funktional verhaelt es sich bereits wie erwartet: navigieren und sich schliessen

### 5.9 Aktuelle funktionale Domäne

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">classDiagram</span>
  <span style="color:#c4b5fd;">class</span> <span style="color:#93c5fd;">Place</span>
  <span style="color:#c4b5fd;">class</span> <span style="color:#67e8f9;">City</span>
  <span style="color:#c4b5fd;">class</span> <span style="color:#86efac;">Favorite</span>
  <span style="color:#c4b5fd;">class</span> <span style="color:#fcd34d;">PlaceFilters</span>

  <span style="color:#67e8f9;">City</span> --&gt; <span style="color:#93c5fd;">Place</span>
  <span style="color:#86efac;">Favorite</span> --&gt; <span style="color:#93c5fd;">Place</span>
  <span style="color:#fcd34d;">PlaceFilters</span> --&gt; <span style="color:#93c5fd;">Place</span></code></pre>

Zusammenfassung des Diagramms:

- es fasst die aktuell sichtbare funktionale Domaene zusammen
- `Place` ist die zentrale Einheit des Produkts
- die Stadt kontextualisiert die Ergebnisse, Favoriten speichern sie und Filter schraenken sie ein

## 6. Zusammengefasster Katalog der Anwendungsfaelle

### UC-01 Startseite sehen

Akteur:

- `Oeffentlicher Benutzer`

Hauptfluss:

1. der Benutzer betritt die `home`
2. er sieht das Werteversprechen von YepPet
3. er kann nach `places`, `Hilfe` oder `Kontakt` navigieren

### UC-02 Orte suchen

Akteur:

- `Oeffentlicher Benutzer`

Hauptfluss:

1. der Benutzer geht zu `places`
2. er setzt Filter oder kommt ueber eine Stadt oder einen Chip dorthin
3. das System zeigt Ergebnisse auf Karte und Liste
4. der Benutzer kann Filter anpassen oder zuruecksetzen

### UC-03 Ort-Detail ansehen

Akteur:

- `Oeffentlicher Benutzer`

Hauptfluss:

1. der Benutzer oeffnet die Detailseite eines Orts aus der Liste oder ueber die Karte
2. er prueft Beschreibung, Adresse, pet-friendly Hinweise und Tags
3. er sieht die ungefaehre Position auf der Karte

### UC-04 Favoriten speichern

Akteur:

- `Oeffentlicher Benutzer`

Hauptfluss:

1. der Benutzer speichert einen Ort aus der Liste oder dem Detail
2. der Ort erscheint in `favorites`
3. der Benutzer kann ihn spaeter wieder entfernen

Hinweis:

- aktuell arbeitet der Fluss mit Fake-Status

### UC-05 Hilfe aufrufen

Akteur:

- `Oeffentlicher Benutzer`

Hauptfluss:

1. der Benutzer oeffnet das Dropdown `Hilfe`
2. er waehlt `So funktioniert es`
3. das System fuehrt ihn zur erklaerenden Sektion der `home`

### UC-06 Kontakt aufnehmen

Akteur:

- `Oeffentlicher Benutzer`

Hauptfluss:

1. der Benutzer oeffnet das Dropdown `Hilfe`
2. er waehlt `Kontakt`
3. das System zeigt die Kontaktseite

## 7. Aktuelle funktionale Regeln

- die `home` soll nicht die komplette reale Suche laden
- die Karte lebt in `places`, nicht auf der Startseite
- die Chips auf der `home` muessen navigierbar sein
- die ausgewaehlten Filter muessen sichtbar bleiben
- derselbe Ort kann in Liste, Detail, Favoriten und Karte erscheinen
- `permissions` gehoert nicht zum oeffentlichen Hauptfluss
- die `hero`-Vorschau darf nicht mit allen Staedten skalieren; sie soll nur hervorgehobenen Inhalt zeigen

## 8. Aktuelle Abnahmekriterien

- man kann von `home` nach `places` navigieren
- man kann nach Stadt, Typ, Haustier und Suche filtern
- die Karte synchronisiert sich mit den sichtbaren Ergebnissen
- das Detail zeigt den korrekten Ort
- Favoriten koennen hinzugefuegt und entfernt werden
- das Dropdown `Hilfe` schliesst sich, wenn es soll
- die responsive Basis bleibt funktionsfaehig

## 9. Dokumentreferenzen

Technisches Dokument:

- [`technisch-de.md`](/home/dmoraroca/Documents/_DATA/repos/yeppet/docs/de/technisch-de.md)

Phasen-Dokument:

- [`project-phases.md`](/home/dmoraroca/Documents/_DATA/repos/yeppet/docs/project-phases.md)
