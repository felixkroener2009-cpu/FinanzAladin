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

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();

            return await context.Transactions
                .AsNoTracking()
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public List<Transaction> GetTransactions()
        {
            using var context = _dbContextFactory.CreateDbContext();

            return context.Transactions
                .AsNoTracking()
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

        public async Task DeleteTransactionAsync(int id)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var transaction = await context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            if (transaction != null)
            {
                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();
            }
        }

        public void DeleteTransaction(int id)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var transaction = context.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                context.Transactions.Remove(transaction);
                context.SaveChanges();
            }
        }

        public async Task<decimal> GetIncomeAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Transactions
                .AsNoTracking()
                .Where(t => t.Type == TransactionType.Income)
                .SumAsync(t => t.Amount);
        }

        public decimal GetIncome()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.Transactions
                .AsNoTracking()
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);
        }

        public async Task<decimal> GetExpensesAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Transactions
                .AsNoTracking()
                .Where(t => t.Type == TransactionType.Expense)
                .SumAsync(t => t.Amount);
        }

        public decimal GetExpenses()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.Transactions
                .AsNoTracking()
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var income = await GetIncomeAsync();
            var expenses = await GetExpensesAsync();
            return income - expenses;
        }

        public decimal GetBalance()
        {
            return GetIncome() - GetExpenses();
        }
    }
}