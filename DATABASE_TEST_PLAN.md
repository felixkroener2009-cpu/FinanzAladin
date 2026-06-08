# 🧪 VOLLSTÄNDIGER FUNKTIONSTEST - FinanzAladin Datenbank

## Test-Szenario

Wir testen alle kritischen Funktionen:

### 1. ✅ Datenbankverbindung
- Kann die App die PostgreSQL-Datenbank erreichen?
- Supabase Verbindung funktioniert?

### 2. ✅ Transaktionen hinzufügen
- Können neue Einträge erstellt werden?
- Werden alle Felder korrekt gespeichert?

### 3. ✅ Transaktionen auflisten
- Können alle Einträge gelesen werden?
- Funktioniert die Sortierung (neueste zuerst)?

### 4. ✅ Transaktionen löschen
- Können Einträge gelöscht werden?
- Liste wird aktualisiert?

### 5. ✅ Aggregationen
- GetBalance() funktioniert?
- GetIncome() / GetExpenses() funktionieren?

---

## MANUELLE TESTS - In der Web-UI

### Test 1: Dashboard besuchen
```
1. Öffne Browser: https://localhost:7066
2. Klicke auf "Dashboard" in der Navigation
3. Erwartetes Ergebnis:
   ✅ Kontostand: 0€ (am Anfang)
   ✅ Einnahmen: 0€
   ✅ Ausgaben: 0€
   ✅ Keine Fehlermeldung
```

### Test 2: Transaktion hinzufügen
```
1. Klicke auf "Neue Buchung" (oder "/add-transaction")
2. Fülle das Formular aus:
   - Titel: "Gehalt"
   - Betrag: 2000
   - Datum: Heute
   - Typ: Einnahme
   - Kategorie: "Einkommen"
   - Notiz: "Monatliches Gehalt"
3. Klicke "Speichern"
4. Erwartetes Ergebnis:
   ✅ Weiterleitung zur Transaktions-Liste
   ✅ Transaktion ist sichtbar
   ✅ Kein Fehler
```

### Test 3: Zweite Transaktion (Ausgabe)
```
1. Klicke wieder auf "Neue Buchung"
2. Fülle das Formular aus:
   - Titel: "Lebensmittel"
   - Betrag: 50
   - Datum: Heute
   - Typ: Ausgabe
   - Kategorie: "Einkaufen"
   - Notiz: "Wöchentliche Einkäufe"
3. Klicke "Speichern"
4. Erwartetes Ergebnis:
   ✅ 2 Transaktionen in der Liste
```

### Test 4: Alle Transaktionen anzeigen
```
1. Klicke auf "Alle Buchungen"
2. Erwartetes Ergebnis:
   ✅ Tabelle mit 2 Einträgen:
      - Gehalt: 2000€ (grünes Badge "Einnahme")
      - Lebensmittel: 50€ (rotes Badge "Ausgabe")
   ✅ Neueste Einträge oben
   ✅ Jede Zeile hat "Löschen" Button
```

### Test 5: Dashboard überprüfen
```
1. Klicke auf "Dashboard"
2. Erwartetes Ergebnis:
   ✅ Kontostand: 1950€ (2000 - 50)
   ✅ Einnahmen: 2000€
   ✅ Ausgaben: 50€
   ✅ Grüne Card für positive Werte
   ✅ Rote Card für Ausgaben
```

### Test 6: Transaktion löschen
```
1. Gehe zu "Alle Buchungen"
2. Klicke "Löschen" bei "Lebensmittel"
3. Bestätige im Dialog: "Möchtest du diese Buchung wirklich löschen?"
4. Erwartetes Ergebnis:
   ✅ Transaktion verschwindet
   ✅ Nur noch 1 Eintrag sichtbar
   ✅ Keine Fehlermeldung
```

### Test 7: Dashboard aktualisieren
```
1. Gehe zum Dashboard
2. Erwartetes Ergebnis:
   ✅ Kontostand: 2000€ (nur noch Gehalt)
   ✅ Einnahmen: 2000€
   ✅ Ausgaben: 0€
```

### Test 8: Mehrere Transaktionen
```
Füge mindestens 5-10 weitere Transaktionen hinzu:
- Verschiedene Typen (Einnahmen/Ausgaben)
- Verschiedene Daten
- Verschiedene Beträge

Erwartetes Ergebnis:
✅ Alle werden angezeigt
✅ Sortierung funktioniert (neueste zuerst)
✅ Dashboard zeigt korrekte Summen
✅ Keine Performance-Probleme
```

---

## AUTOMATISIERTE TESTS - Code

Verwende die `DatabaseTests.cs` Klasse:

```csharp
// In Program.cs oder in einem Test-Endpoint hinzufügen:
var tests = new DatabaseTests(dbContextFactory);
await tests.RunAllTests();
```

Dies führt folgende automatische Tests durch:
1. ✅ Datenbankverbindung
2. ✅ Tabelle existiert
3. ✅ Transaktion hinzufügen
4. ✅ Alle Transaktionen auslesen
5. ✅ Aggregationen berechnen
6. ✅ Transaktion löschen

---

## FEHLERBEHANDLUNG

### Wenn folgende Fehler auftreten:

#### 1. "relation Transactions does not exist"
```
Lösung:
✅ App neustart
✅ Oder manuell SQL ausführen:
   CREATE TABLE IF NOT EXISTS "Transactions" (...)
```

#### 2. "Cannot connect to database"
```
Lösung:
✅ ConnectionString prüfen (appsettings.json)
✅ Supabase Verbindung überprüfen
✅ Firewall/VPN prüfen
✅ PostgreSQL Port 5432 erreichbar?
```

#### 3. "Timeout"
```
Lösung:
✅ Netzwerkverbindung überprüfen
✅ Supabase-Server-Status prüfen
✅ Connection String Timeout erhöhen
```

---

## ERFOLGS-KRITERIEN

Alle folgenden Punkte müssen erfüllt sein:

✅ Dashboard lädt ohne Fehler
✅ Transaktionen können hinzugefügt werden
✅ Alle Transaktionen werden angezeigt
✅ Transaktionen können gelöscht werden
✅ Dashboard zeigt korrekte Summen
✅ Aggregationen funktionieren (Income, Expenses, Balance)
✅ Keine Datenverluste
✅ Keine unerwarteten Fehler

---

## PERFORMANCE-TESTS

Nach dem Datenbank-Test solltest du auch Performance testen:

1. **Schnelligkeit:**
   - Dashboard sollte < 2 Sekunden laden
   - Transaktion hinzufügen < 1 Sekunde
   - Liste mit 100 Einträgen sollte smooth sein

2. **Speicher:**
   - App sollte nicht mehr als ~150MB RAM nutzen
   - Kein Memory Leak

3. **Skalierung:**
   - Funktioniert mit 1000 Transaktionen?
   - Funktioniert mit 10000 Transaktionen?

---

## ABSCHLUSSBERICHT

Nach allen Tests einen Report erstellen:

```
FUNKTIONSTEST-ABSCHLUSS
========================

Getestet: [Datum/Zeit]
Tester: [Name]

Datenbank:        ✅ FUNKTIONIERT
Hinzufügen:       ✅ FUNKTIONIERT
Auflisten:        ✅ FUNKTIONIERT
Löschen:          ✅ FUNKTIONIERT
Aggregationen:    ✅ FUNKTIONIERT
Performance:      ✅ OK
UI/UX:            ✅ GUT

Status: ✅ PRODUKTIONSREIF
```

---

Viel Erfolg beim Testen! 🎉
