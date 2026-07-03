namespace Domain.Entities
{
    public class ReceiptEntity
    {
        public int Id { get; private set; }
        public int? UserId { get; private set; }
        public string Description { get; private set; }
        public decimal MinValue { get; private set; }
        public decimal MaxValue { get; private set; }
        public DateTime PaymentDate { get; private set; }

        public ReceiptEntity(int? id, string description, decimal minValue, decimal maxValue, DateTime paymentDate, int? userId)
        {
            Id = (int)(id == null ? 0 : id);
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
            PaymentDate = paymentDate;
            UserId = userId;
        }
    }
}
