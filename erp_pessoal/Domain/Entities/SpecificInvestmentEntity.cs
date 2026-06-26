namespace Domain.Entities
{
    public class SpecificInvestmentEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int SpecificId { get; private set; }
        public DateTime ExtractDate { get; private set; }
        public string Description { get; private set; }
        public decimal Value { get; private set; }
        public int InvestId { get; private set; }
        public string InvestName { get; private set; }
        public decimal InvestValue { get; private set; }
        public float Interest { get; private set; }
        public DateTime InvestDate { get; private set; }
        public decimal Balance { get; private set; }

        public SpecificInvestmentEntity(int id, int userId, int specificId, DateTime extractDate, string description, decimal value, int investId, string investName, decimal investValue, float interest, DateTime investDate, decimal balance)
        {
            Id = id;
            UserId = userId;
            SpecificId = specificId;
            ExtractDate = extractDate;
            Description = description;
            Value = value;
            InvestId = investId;
            InvestName = investName;
            InvestValue = investValue;
            Interest = interest;
            InvestDate = investDate;
            Balance = balance;
        }
    }
}
