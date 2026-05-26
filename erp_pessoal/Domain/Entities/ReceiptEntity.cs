namespace Domain.Entities
{
    public class ReceiptEntity
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        public DateTime PaymentDate { get; private set; }

        public ReceiptEntity(int id, string description, float minValue, float maxValue, DateTime paymentDate)
        {
            Id = id;
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
            PaymentDate = paymentDate;
        }
    }
}
