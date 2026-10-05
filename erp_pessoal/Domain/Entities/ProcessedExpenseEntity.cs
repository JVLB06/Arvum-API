namespace Domain.Entities
{
    public class ProcessedExpenseEntity
    {
        public DateTime Month { get; private set; }

        public decimal TotalExpenses { get; private set; }
        public decimal TotalIncomes { get; private set; }
        public decimal ExpensesLimit { get; private set; }

        public ProcessedExpenseEntity(DateTime month, decimal totalExpenses, decimal totalIncomes, decimal expensesLimit)
        {
            Month = month;
            TotalExpenses = totalExpenses;
            TotalIncomes = totalIncomes;
            ExpensesLimit = expensesLimit;
        }
    }
}
