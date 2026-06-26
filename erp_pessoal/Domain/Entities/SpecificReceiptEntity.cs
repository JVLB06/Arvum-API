namespace Domain.Entities
{
    public class SpecificReceiptEntity
    {
        public int Id { get; private set; }
        public int UseId { get; private set; }
        public int SpecificId { get; private set; }
        public DateTime ExtractDate { get; private set; }
        public string Description { get; private set; }
        public decimal Value { get; private set; }
        public int ReceiptId { get; private set; }
        public decimal ReceiptValue { get; private set; }
        public DateTime ReceiptDate { get; private set; }
        public decimal Balance { get; private set; }

        public SpecificReceiptEntity(int id, int userId, int specificId, DateTime extractDate, string description,  decimal value, int receiptId, decimal receiptValue, DateTime receiptDate, decimal balance)
        {
            Id = id;
            UseId = userId;
            SpecificId = specificId;
            ExtractDate = extractDate;
            Description = description;
            Value = value;
            ReceiptId = receiptId;
            ReceiptValue = receiptValue;
            ReceiptDate = receiptDate;
            Balance = balance;
        }
    }
}
