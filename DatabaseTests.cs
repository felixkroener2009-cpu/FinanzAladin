// 🧪 DATENBANKTEST - FinanzAladin
// Dieser Code testet alle Datenbankfunktionen

using FinanzAladin.Database;
using FinanzAladin.Models;
using FinanzAladin.Services;
using Microsoft.EntityFrameworkCore;

namespace FinanzAladin.Tests
{
    public class DatabaseTests
    {
        private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

        public DatabaseTests(IDbContextFactory<FinanceDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // ✅ TEST 1: Datenbankverbindung
        public async Task<bool> TestDatabaseConnection()
        {
            Console.WriteLine("🧪 TEST 1: Datenbankverbindung...");
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                await context.Database.CanConnectAsync();
                Console.WriteLine("✅ Verbindung erfolgreich!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Verbindungsfehler: {ex.Message}");
                return false;
            }
        }

        // ✅ TEST 2: Tabelle existiert
        public async Task<bool> TestTableExists()
        {
            Console.WriteLine("\n🧪 TEST 2: Transactions-Tabelle existiert...");
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var count = await context.Transactions.CountAsync();
                Console.WriteLine($"✅ Tabelle existiert! (Aktuelle Zeilen: {count})");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Tabellenfehlr: {ex.Message}");
                return false;
            }
        }

        // ✅ TEST 3: Transaktion hinzufügen
        public async Task<bool> TestAddTransaction()
        {
            Console.WriteLine("\n🧪 TEST 3: Transaktion hinzufügen...");
            try
            {
                var transaction = new Transaction
                {
                    Title = "🧪 Test-Transaktion",
                    Amount = 99.99m,
                    Date = DateTime.Now,
                    Type = TransactionType.Income,
                    Category = "Test",
                    Note = "Automatisierter Test"
                };

                using var context = _dbContextFactory.CreateDbContext();
                context.Transactions.Add(transaction);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Transaktion hinzugefügt!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Hinzufügen: {ex.Message}");
                return false;
            }
        }

        // ✅ TEST 4: Alle Transaktionen auslesen
        public async Task<bool> TestGetAllTransactions()
        {
            Console.WriteLine("\n🧪 TEST 4: Alle Transaktionen auslesen...");
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var transactions = await context.Transactions
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();

                Console.WriteLine($"✅ {transactions.Count} Transaktionen gefunden:");
                foreach (var tx in transactions.Take(5))
                {
                    Console.WriteLine($"   - {tx.Title}: {tx.Amount}€ ({tx.Type})");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Auslesen: {ex.Message}");
                return false;
            }
        }

        // ✅ TEST 5: Aggregationen (Income, Expenses, Balance)
        public async Task<bool> TestAggregations()
        {
            Console.WriteLine("\n🧪 TEST 5: Aggregationen berechnen...");
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var income = await context.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .SumAsync(t => t.Amount);

                var expenses = await context.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .SumAsync(t => t.Amount);

                var balance = income - expenses;

                Console.WriteLine($"✅ Aggregationen berechnet:");
                Console.WriteLine($"   Einnahmen: {income}€");
                Console.WriteLine($"   Ausgaben: {expenses}€");
                Console.WriteLine($"   Kontostand: {balance}€");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler bei Aggregationen: {ex.Message}");
                return false;
            }
        }

        // ✅ TEST 6: Transaktion löschen
        public async Task<bool> TestDeleteTransaction()
        {
            Console.WriteLine("\n🧪 TEST 6: Transaktion löschen...");
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                // Finde Test-Transaktion
                var testTx = await context.Transactions
                    .FirstOrDefaultAsync(t => t.Title == "🧪 Test-Transaktion");

                if (testTx != null)
                {
                    context.Transactions.Remove(testTx);
                    await context.SaveChangesAsync();
                    Console.WriteLine("✅ Transaktion gelöscht!");
                    return true;
                }
                else
                {
                    Console.WriteLine("⚠️  Test-Transaktion nicht gefunden");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Löschen: {ex.Message}");
                return false;
            }
        }

        // ✅ ALLE TESTS AUSFÜHREN
        public async Task RunAllTests()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("🧪 FINANZALADIN DATENBANK-TESTS");
            Console.WriteLine("═══════════════════════════════════════════════════════════\n");

            var results = new List<(string Test, bool Passed)>
            {
                ("Datenbankverbindung", await TestDatabaseConnection()),
                ("Tabelle existiert", await TestTableExists()),
                ("Transaktion hinzufügen", await TestAddTransaction()),
                ("Alle Transaktionen auslesen", await TestGetAllTransactions()),
                ("Aggregationen", await TestAggregations()),
                ("Transaktion löschen", await TestDeleteTransaction()),
            };

            Console.WriteLine("\n═══════════════════════════════════════════════════════════");
            Console.WriteLine("📊 TEST ERGEBNISSE:");
            Console.WriteLine("═══════════════════════════════════════════════════════════\n");

            foreach (var (test, passed) in results)
            {
                var status = passed ? "✅ BESTANDEN" : "❌ FEHLGESCHLAGEN";
                Console.WriteLine($"{status} - {test}");
            }

            var passedCount = results.Count(r => r.Passed);
            var totalCount = results.Count;

            Console.WriteLine($"\n🎯 GESAMT: {passedCount}/{totalCount} Tests bestanden\n");

            if (passedCount == totalCount)
            {
                Console.WriteLine("🎉 ALLE TESTS BESTANDEN - DATENBANK FUNKTIONIERT PERFEKT!");
            }
            else
            {
                Console.WriteLine("⚠️  EINIGE TESTS FEHLGESCHLAGEN - BITTE ÜBERPRÜFEN!");
            }

            Console.WriteLine("═══════════════════════════════════════════════════════════\n");
        }
    }
}
