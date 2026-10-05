namespace Application.DTOs
{
    public class GoalCompositionItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalContributed { get; set; }
        public string Type { get; set; } = string.Empty; // "divida", "investimento", "meta"
    }

    public class GoalCompositionDTO
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public DateTime GoalDate { get; set; }
        public decimal TotalAccumulated { get; set; }
        public decimal ProgressPercentage { get; set; }
        public List<GoalCompositionItemDTO> Items { get; set; } = new();
    }
}
