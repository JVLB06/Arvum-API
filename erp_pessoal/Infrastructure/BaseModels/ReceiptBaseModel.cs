namespace Infrastructure.BaseModels
{
    public class ReceiptBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
