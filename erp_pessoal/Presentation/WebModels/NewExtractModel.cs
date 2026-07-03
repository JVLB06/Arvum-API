namespace Presentation.WebModels
{
    public class NewExtractModel
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
