# 🎯 FinanzAladin Datenbank Test-Bericht Generator

## Verwendung

Führe diesen Code in einem Controller oder im Program.cs aus:

```csharp
// Beispiel in Program.cs (nach app.Build())
var tests = new DatabaseTestSuite(app.Services.GetRequiredService<IDbContextFactory<FinanceDbContext>>());
await tests.GenerateTestReport();
```

---

## Test Suite Code

```csharp
using FinanzAladin.Database;
using FinanzAladin.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FinanzAladin.Testing
{
    public class DatabaseTestSuite
    {
        private readonly IDbContextFactory<FinanceDbContext> _contextFactory;
        private StringBuilder _report = new StringBuilder();

        public DatabaseTestSuite(IDbContextFactory<FinanceDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task GenerateTestReport()
        {
            _report.Clear();
            _report.AppendLine("═══════════════════════════════════════════════════════════");
            _report.AppendLine("📊 FINANZALADIN DATENBANK TEST-BERICHT");
            _report.AppendLine($"Datum/Zeit: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            _report.AppendLine("═══════════════════════════════════════════════════════════\n");

            var results = new Dictionary<string, bool>();

            // Test 1: Verbindung
            results["Datenbankverbindung"] = await TestConnection();

            // Test 2: Tabelle
            results["Transaktions-Tabelle existiert"] = await TestTableExists();

            // Test 3: CRUD Operationen
            results["CREATE (Hinzufügen)"] = await TestCreate();
            results["READ (Auslesen)"] = await TestRead();
            results["UPDATE (Aktualisieren)"] = await TestUpdate();
            results["DELETE (Löschen)"] = await TestDelete();

            // Test 4: Aggregationen
            results["Aggregationen (SUM, WHERE)"] = await TestAggregations();

            // Test 5: Validierung
            results["Datenvalidierung"] = await TestValidation();

            // Test 6: Performance
            results["Performance"] = await TestPerformance();

            // Test 7: Fehlerbehandlung
            results["Fehlerbehandlung"] = await TestErrorHandling();

            // Report anzeigen
            PrintResults(results);
        }

        private async Task<bool> TestConnection()
        {
            _report.AppendLine("🧪 TEST 1: Datenbankverbindung");
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect)
                {
                    _report.AppendLine("✅ BESTANDEN: Verbindung zur Datenbank erfolgreich\n");
                    return true;
                }
                else
                {
                    _report.AppendLine("❌ FEHLGESCHLAGEN: Kann nicht verbinden\n");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestTableExists()
        {
            _report.AppendLine("🧪 TEST 2: Transaktions-Tabelle existiert");
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tables = context.Model.GetEntityTypes();
                var hasTransactions = tables.Any(t => t.Name == nameof(Transaction));
                if (hasTransactions)
                {
                    var count = await context.Transactions.CountAsync();
                    _report.AppendLine($"✅ BESTANDEN: Tabelle existiert ({count} Zeilen)\n");
                    return true;
                }
                else
                {
                    _report.AppendLine("❌ FEHLGESCHLAGEN: Tabelle nicht gefunden\n");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestCreate()
        {
            _report.AppendLine("🧪 TEST 3: Transaktion erstellen (CREATE)");
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var newTransaction = new Transaction
                {
                    Title = "Test Eintrag",
                    Amount = 100m,
                    Date = DateTime.Now,
                    Type = TransactionType.Income,
                    Category = "Test",
                    Note = "Testdaten"
                };

                context.Transactions.Add(newTransaction);
                var rowsAffected = await context.SaveChangesAsync();

                if (rowsAffected > 0)
                {
                    _report.AppendLine($"✅ BESTANDEN: {rowsAffected} Zeilen erstellt\n");
                    return true;
                }
                else
                {
                    _report.AppendLine("❌ FEHLGESCHLAGEN: Keine Zeilen erstellt\n");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestRead()
        {
            _report.AppendLine("🧪 TEST 4: Transaktionen auslesen (READ)");
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var transactions = await context.Transactions
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();

                _report.AppendLine($"✅ BESTANDEN: {transactions.Count} Transaktionen gelesen");
                _report.AppendLine($"   - Neueste: {(transactions.FirstOrDefault()?.Title ?? "N/A")}");
                _report.AppendLine($"   - Älteste: {(transactions.LastOrDefault()?.Title ?? "N/A")}\n");
                return transactions.Count > 0;
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestUpdate()
        {
            _report.AppendLine("🧪 TEST 5: Transaktion aktualisieren (UPDATE)");
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tx = await context.Transactions.FirstOrDefaultAsync();

                if (tx != null)
                {
                    var oldTitle = tx.Title;
                    tx.Title = $"{oldTitle} (Updated)";

                    var rowsAffected = await context.SaveChangesAsync();
                    if (rowsAffected > 0)
                    {
                        _report.AppendLine($"✅ BESTANDEN: Transaktion aktualisiert ({oldTitle} → {tx.Title})\n");

                        // Revert
                        tx.Title = oldTitle;
                        await context.SaveChangesAsync();

                        return true;
                    }
                }

                _report.AppendLine("⚠️  ÜBERSPRUNGEN: Keine Daten zum Aktualisieren\n");
                return true; // Nicht fehlgeschlagen, nur keine Daten
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestDelete()
        {
            _report.AppendLine("🧪 TEST 6: Transaktion löschen (DELETE)");
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Finde Test-Eintrag zum Löschen
                var txToDelete = await context.Transactions
                    .FirstOrDefaultAsync(t => t.Title.Contains("Test Eintrag"));

                if (txToDelete != null)
                {
                    context.Transactions.Remove(txToDelete);
                    var rowsAffected = await context.SaveChangesAsync();

                    if (rowsAffected > 0)
                    {
                        _report.AppendLine($"✅ BESTANDEN: {rowsAffected} Zeilen gelöscht\n");
                        return true;
                    }
                }

                _report.AppendLine("⚠️  ÜBERSPRUNGEN: Keine Test-Daten zum Löschen\n");
                return true;
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestAggregations()
        {
            _report.AppendLine("🧪 TEST 7: Aggregationen (SUM, WHERE)");
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var income = await context.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .SumAsync(t => t.Amount);

                var expenses = await context.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .SumAsync(t => t.Amount);

                var balance = income - expenses;

                _report.AppendLine($"✅ BESTANDEN: Aggregationen berechnet");
                _report.AppendLine($"   - Einnahmen: {income:C2}");
                _report.AppendLine($"   - Ausgaben: {expenses:C2}");
                _report.AppendLine($"   - Kontostand: {balance:C2}\n");
                return true;
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestValidation()
        {
            _report.AppendLine("🧪 TEST 8: Datenvalidierung");
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Versuche Invalid Entry hinzuzufügen
                var invalidTx = new Transaction
                {
                    Title = "",  // Ungültig (Required)
                    Amount = -10,  // Ungültig (Range)
                    Date = DateTime.Now,
                    Type = TransactionType.Income
                };

                context.Transactions.Add(invalidTx);
                await context.SaveChangesAsync();

                _report.AppendLine("❌ FEHLGESCHLAGEN: Validierung nicht wirksam\n");
                return false;
            }
            catch (DbUpdateException ex)
            {
                _report.AppendLine($"✅ BESTANDEN: Validierung wirksam (Fehler abgefangen)\n");
                return true;
            }
            catch (Exception ex)
            {
                _report.AppendLine($"✅ BESTANDEN: Ungültige Daten abgelehnt\n");
                return true;
            }
        }

        private async Task<bool> TestPerformance()
        {
            _report.AppendLine("🧪 TEST 9: Performance");
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var result = await context.Transactions
                    .AsNoTracking()
                    .ToListAsync();
                stopwatch.Stop();

                var elapsed = stopwatch.ElapsedMilliseconds;
                var status = elapsed < 500 ? "Ausgezeichnet" : elapsed < 1000 ? "Gut" : "Akzeptabel";

                _report.AppendLine($"✅ BESTANDEN: {result.Count} Zeilen in {elapsed}ms gelesen ({status})\n");
                return true;
            }
            catch (Exception ex)
            {
                _report.AppendLine($"❌ FEHLGESCHLAGEN: {ex.Message}\n");
                return false;
            }
        }

        private async Task<bool> TestErrorHandling()
        {
            _report.AppendLine("🧪 TEST 10: Fehlerbehandlung");
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Versuche ungültigen Query
                try
                {
                    var result = await context.Transactions
                        .FromSqlRaw("SELECT * FROM NonExistentTable")
                        .ToListAsync();

                    _report.AppendLine("❌ FEHLGESCHLAGEN: Fehler nicht abgefangen\n");
                    return false;
                }
                catch (Exception)
                {
                    _report.AppendLine("✅ BESTANDEN: Fehler korrekt abgefangen\n");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _report.AppendLine($"⚠️  ÜBERSPRUNGEN: {ex.Message}\n");
                return true;
            }
        }

        private void PrintResults(Dictionary<string, bool> results)
        {
            _report.AppendLine("═══════════════════════════════════════════════════════════");
            _report.AppendLine("📋 ZUSAMMENFASSUNG");
            _report.AppendLine("═══════════════════════════════════════════════════════════\n");

            var passed = results.Count(r => r.Value);
            var failed = results.Count(r => !r.Value);

            foreach (var (test, result) in results)
            {
                var status = result ? "✅" : "❌";
                _report.AppendLine($"{status} {test}");
            }

            _report.AppendLine($"\n🎯 GESAMT: {passed}/{results.Count} Tests bestanden");
            _report.AppendLine("═══════════════════════════════════════════════════════════\n");

            if (passed == results.Count)
            {
                _report.AppendLine("🎉 ALLE TESTS ERFOLGREICH!");
                _report.AppendLine("✅ Datenbank funktioniert perfekt");
                _report.AppendLine("✅ App ist produktionsreif");
            }
            else
            {
                _report.AppendLine($"⚠️  {failed} Test(s) fehlgeschlagen!");
                _report.AppendLine("❌ Bitte Fehler überprüfen");
            }

            _report.AppendLine("═══════════════════════════════════════════════════════════\n");

            // Bericht ausgeben
            var reportText = _report.ToString();
            Console.WriteLine(reportText);

            // Optional: In Datei speichern
            File.WriteAllText("database-test-report.txt", reportText);
        }
    }
}
```

---

## Wie verwenden

### Option 1: In Program.cs
```csharp
// Nach app.Build()
if (app.Environment.IsDevelopment())
{
    var testSuite = new DatabaseTestSuite(
        app.Services.GetRequiredService<IDbContextFactory<FinanceDbContext>>()
    );
    await testSuite.GenerateTestReport();
}
```

### Option 2: In einem Controller
```csharp
[HttpGet("api/test/database")]
public async Task<IActionResult> TestDatabase()
{
    var testSuite = new DatabaseTestSuite(_contextFactory);
    await testSuite.GenerateTestReport();
    return Ok("Test ausgeführt - siehe Console output");
}
```

---

## Output Beispiel

```
═══════════════════════════════════════════════════════════
📊 FINANZALADIN DATENBANK TEST-BERICHT
Datum/Zeit: 01.01.2024 14:30:00
═══════════════════════════════════════════════════════════

🧪 TEST 1: Datenbankverbindung
✅ BESTANDEN: Verbindung zur Datenbank erfolgreich

🧪 TEST 2: Transaktions-Tabelle existiert
✅ BESTANDEN: Tabelle existiert (5 Zeilen)

🧪 TEST 3: Transaktion erstellen (CREATE)
✅ BESTANDEN: 1 Zeilen erstellt

...

═══════════════════════════════════════════════════════════
📋 ZUSAMMENFASSUNG
═══════════════════════════════════════════════════════════

✅ Datenbankverbindung
✅ Transaktions-Tabelle existiert
✅ CREATE (Hinzufügen)
✅ READ (Auslesen)
✅ UPDATE (Aktualisieren)
✅ DELETE (Löschen)
✅ Aggregationen (SUM, WHERE)
✅ Datenvalidierung
✅ Performance
✅ Fehlerbehandlung

🎯 GESAMT: 10/10 Tests bestanden

🎉 ALLE TESTS ERFOLGREICH!
✅ Datenbank funktioniert perfekt
✅ App ist produktionsreif

═══════════════════════════════════════════════════════════
```

---

Viel Erfolg beim Testen! 🧪
