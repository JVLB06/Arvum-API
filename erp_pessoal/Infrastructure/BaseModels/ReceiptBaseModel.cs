namespace Infrastructure.BaseModels
{
    public class ReceiptBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
