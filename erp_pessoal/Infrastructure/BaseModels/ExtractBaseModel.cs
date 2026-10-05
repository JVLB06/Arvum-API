namespace Infrastructure.BaseModels
{
    public class ExtractBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime ExtractDate { get; set; }
        public string Kind { get; set; }
        public decimal Balance { get; set; }
        public int? ExternalId { get; set; }
        public int? GoalId { get; set; }
        public string? GoalName { get; set; }
    }
}
