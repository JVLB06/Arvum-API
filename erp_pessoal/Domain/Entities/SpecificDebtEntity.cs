namespace Domain.Entities
{
    public class SpecificDebtEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int SpecificId { get; private set; }
        public DateTime ExtractDate { get; private set; }
        public string Description { get; private set; }
        public decimal EntryValue { get; private set; }
        public int DebtId { get; private set; }
        public string DebtName { get; private set; }
        public decimal DebtValue { get; private set; }
        public DateTime DebtDate { get; private set; }
        public DateTime DebtEndDate { get; private set; }
        public decimal Balance { get; private set; }

        public SpecificDebtEntity(int id, int userId, int specificId, DateTime extractDate, string description, decimal entryValue, int debtId, string debtName, decimal debtValue, DateTime debtDate, DateTime debtEndDate, decimal balance)
        {
            Id = id;
            UserId = userId;
            SpecificId = specificId;
            ExtractDate = extractDate;
            Description = description;
            EntryValue = entryValue;
            DebtId = debtId;
            DebtName = debtName;
            DebtValue = debtValue;
            DebtDate = debtDate;
            DebtEndDate = debtEndDate;
            Balance = balance;
        }
    }
}
