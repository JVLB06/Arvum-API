namespace Infrastructure.BaseModels
{
    public class ProcessedExpenseBaseModel
    {
        public DateTime Month {  get; set; }

        public decimal TotalExpenses { get; set; }
        public decimal TotalIncomes { get; set; }
        public decimal ExpensesLimit { get; set; }
    }
}
