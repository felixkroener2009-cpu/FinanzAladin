namespace FinanzAladin.Classes
{
    public class Transaction
    {

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public TransactionType Type { get; set; }
        public int CategoryId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
