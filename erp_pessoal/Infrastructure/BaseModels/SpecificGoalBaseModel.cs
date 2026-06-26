namespace Infrastructure.BaseModels
{
    public class SpecificGoalBaseModel
    {
        public int Id { get; set; }
        public int SpecificId { get; set; } 
        public DateTime ExtractDate { get; set; } 
        public string Description { get; set } 
        public decimal EntryValue { get; set; }
        public int GoalId { get; set; }
        public string GoalName {  get; set; }
        public decimal FullGoalValue {  get; set; }
        public DateTime GoalDate {  get; set; }
        public float Progress {  get; set; }
        public decimal Balance {  get; set; }
    }
}
