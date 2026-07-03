namespace Domain.Entities
{
    public class SpecificExpensesEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int SpecificId { get; private set; }
        public DateTime ExtractDate { get; private set; }
        public string Description { get; private set; }
        public decimal EntryValue { get; private set; }
        public int ExpenseId { get; private set; }
        public string ExpenseName { get; private set; }
        public decimal ExpenseValue { get; private set; }
        public DateTime ExpenseDate { get; private set; }
        public bool Variable { get; private set; }
        public decimal Balance { get; private set; }

        public SpecificExpensesEntity(int id, int userId, int specificId, DateTime extractDate, string description, decimal entryValue, int expenseId, string expenseName, decimal expenseValue, DateTime expenseDate, bool variable, decimal balance)
        {
            Id = id;
            UserId = userId;
            SpecificId = specificId;
            ExtractDate = extractDate;
            Description = description;
            EntryValue = entryValue;
            ExpenseId = expenseId;
            ExpenseName = expenseName;
            ExpenseValue = expenseValue;
            ExpenseDate = expenseDate;
            Variable = variable;
            Balance = balance;
        }
    }
}
