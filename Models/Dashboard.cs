namespace FinanzAladin.Classes
{
    public class Dashboard
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance => TotalIncome - TotalExpenses;
        public decimal Savings => Balance;
    }
}
