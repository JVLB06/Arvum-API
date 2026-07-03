namespace Application.DTOs
{
    public class ReceiptDTO
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
