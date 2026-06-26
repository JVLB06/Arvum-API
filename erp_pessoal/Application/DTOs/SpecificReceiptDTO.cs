namespace Application.DTOs
{
    public class SpecificReceiptDTO
    {
        public int Id { get; set; }
        public int SpecificId { get; set; }
        public DateTime ExtractDate { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public int ReceiptId { get; set; }
        public decimal ReceiptValue { get; set; }
        public DateTime ReceiptDate { get; set; }
        public decimal Balance { get; set; }
    }
}
