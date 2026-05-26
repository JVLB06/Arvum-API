namespace Presentation.WebModels
{
    public class RegisterReceiptModel
    {
        public int? ReceiptId { get; set; }
        public string Name { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
