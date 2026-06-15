using System.ComponentModel.DataAnnotations;

namespace FinanzAladin.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bitte gib einen Titel ein.")]
        public string Title { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Der Betrag muss größer als 0 sein.")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public TransactionType Type { get; set; } = TransactionType.Expense;

        public string Category { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User? User { get; set; }
    }
}

