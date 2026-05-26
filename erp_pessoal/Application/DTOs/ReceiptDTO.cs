namespace Application.DTOs
{
    public class ReceiptDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
