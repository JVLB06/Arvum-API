namespace Domain.Entities
{
    public class ExtractEntity
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public string Name { get; private set; }
        public decimal Value { get; private set; }
        public DateTime ExtractDate { get; private set; }
        public string Kind { get; private set; }
        public decimal? Balance { get; private set; }

        public ExtractEntity(int? id, int? userId, string name, decimal value, DateTime extractDate, string kind, decimal? balance) { 
            Id = id;
            UserId = userId;
            Name = name;
            Value = value;
            ExtractDate = extractDate;
            Kind = kind;
            Balance = balance is null ? 0 : balance;
        }
    }
}
