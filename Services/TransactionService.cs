using FinanzAladin.Database;
using FinanzAladin.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzAladin.Services
{
    public class TransactionService
    {
        private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

        public TransactionService(IDbContextFactory<FinanceDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int userId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                return await context.Transactions
                    .AsNoTracking()
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetTransactionsAsync: {ex.Message}");
                throw;
            }
        }

        public List<Transaction> GetTransactions(int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            return context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToList();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
        }

        public void AddTransaction(Transaction transaction)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.Transactions.Add(transaction);
            context.SaveChanges();
        }

        public async Task DeleteTransactionAsync(int id, int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var transaction = await context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transaction != null)
            {
                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();
            }
        }

        public void DeleteTransaction(int id, int UserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var transaction = context.Transactions.FirstOrDefault(t => t.Id == id && t.UserId == UserId);
            if (transaction != null)
            {
                context.Transactions.Remove(transaction);
                context.SaveChanges();
            }
        }

        public async Task<decimal> GetIncomeAsync(int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Type == TransactionType.Income)
                .SumAsync(t => t.Amount);
        }

        public decimal GetIncome(int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Type == TransactionType.Income)
                .Sum(t => t.Amount);
        }

        public async Task<decimal> GetExpensesAsync(int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense)
                .SumAsync(t => t.Amount);
        }

        public decimal GetExpenses(int userId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var income = await GetIncomeAsync(userId);
            var expenses = await GetExpensesAsync(userId);
            return income - expenses;
        }

        public decimal GetBalance(int userId)
        {
            return GetIncome(userId) - GetExpenses(userId);
        }
    }
}