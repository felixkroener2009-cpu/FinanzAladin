using FinanzAladin.Classes;

namespace Finanzmanager.Services;

public class TransactionService
{
    public List<Transaction> Transactions { get; set; } = new();

    public void AddTransaction(Transaction transaction)
    {
        transaction.Id = Transactions.Count + 1;
        Transactions.Add(transaction);
    }

    public decimal GetIncome()
    {
        return Transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);
    }

    public decimal GetExpenses()
    {
        return Transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);
    }

    public decimal GetBalance()
    {
        return GetIncome() - GetExpenses();
    }
}