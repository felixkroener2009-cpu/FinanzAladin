# 📋 SCHNELL-TEST - FinanzAladin Datenbank

## 🎯 In 5 Minuten testen

### Schritt 1️⃣: Dashboard Test (30 Sekunden)
```
1. Browser: https://localhost:7066
2. Klick: "Dashboard" (Navigation)
3. Sollte zeigen: 
   ✅ Kontostand: 0€
   ✅ Einnahmen: 0€
   ✅ Ausgaben: 0€
   ✅ KEIN FEHLER
```

### Schritt 2️⃣: Transaktion hinzufügen (1 Minute)
```
1. Klick: "Neue Buchung"
2. Ausfüllen:
   - Titel:       "Gehalt"
   - Betrag:      1000
   - Datum:       Heute
   - Typ:         Einnahme
   - Kategorie:   Einkommen
   - Notiz:       Test
3. Klick: "Speichern"
4. Sollte weiterleiten zu "Alle Buchungen"
5. ✅ "Gehalt 1000€" sichtbar
```

### Schritt 3️⃣: Zweite Transaktion (1 Minute)
```
1. Klick: "Neue Buchung"
2. Ausfüllen:
   - Titel:       "Pizza"
   - Betrag:      15
   - Datum:       Heute
   - Typ:         Ausgabe
   - Kategorie:   Essen
   - Notiz:       Test
3. Klick: "Speichern"
4. ✅ Jetzt 2 Transaktionen sichtbar
```

### Schritt 4️⃣: Liste überprüfen (1 Minute)
```
1. Du solltest sehen:
   ✅ "Gehalt"  +1000€  (grünes Badge)
   ✅ "Pizza"   -15€    (rotes Badge)
2. Buttons "Löschen" vorhanden?
   ✅ JA
```

### Schritt 5️⃣: Dashboard Check (30 Sekunden)
```
1. Klick: "Dashboard"
2. Sollte zeigen:
   ✅ Kontostand: 985€   (1000 - 15)
   ✅ Einnahmen:  1000€
   ✅ Ausgaben:   15€
   ✅ KORREKT!
```

---

## ✅ ERGEBNIS

Wenn alles oben funktioniert:

```
🎉 DATENBANK FUNKTIONIERT PERFEKT!
```

Folgende Funktionen sind getestet:
- ✅ Datenbank-Verbindung
- ✅ Transaktion hinzufügen (INSERT)
- ✅ Transaktion auslesen (SELECT)
- ✅ Aggregation berechnen (SUM)
- ✅ UI aktualisiert sich

---

## ❌ WENN FEHLER AUFTRITT

### Fehler: "relation Transactions does not exist"
```
Lösung:
1. App-Fenster schließen (Ctrl+C)
2. F5 drücken (Neustarten)
3. Erneut testen
```

### Fehler: "Cannot connect to server"
```
Lösung:
1. Überprüfe Internetverbindung
2. Überprüfe Supabase-Anmeldedaten
3. Check appsettings.json ConnectionString
```

### Fehler: "Eingabevalidierung fehlgeschlagen"
```
Lösung:
1. Alle Felder ausfüllen
2. Betrag > 0
3. Titel nicht leer
```

---

## 📊 TEST-MATRIX

| Funktion | Test | Ergebnis |
|----------|------|----------|
| Dashboard laden | ✅ | |
| Transaktion + (Income) | ✅ | |
| Transaktion - (Expense) | ✅ | |
| Liste anzeigen | ✅ | |
| Aggregationen | ✅ | |
| Fehlerbehandlung | ✅ | |

Fülle diese Tabelle bei jedem Test aus!

---

## 🚀 FERTIG!

Wenn alle Tests ✅ sind:
- Datenbank funktioniert
- App funktioniert
- Ready for production!

Happy Testing! 🎉
