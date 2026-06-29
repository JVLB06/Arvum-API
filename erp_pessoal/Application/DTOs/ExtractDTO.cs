namespace Application.DTOs
{
    public class ExtractDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime ExtractDate { get; set; }
        public string Kind { get; set; }
        public decimal Balance { get; set; }
        public int? ExternalId { get; set; }
    }
}
