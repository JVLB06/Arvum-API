namespace Domain.Entities
{
    public class SpecificGoalEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int SpecificId { get; private set; }
        public DateTime ExtractDate { get; private set; }
        public string Description { get; private set; }
        public decimal EntryValue { get; private set; }
        public int GoalId { get; private set; }
        public string GoalName { get; private set; }
        public decimal FullGoalValue { get; private set; }
        public DateTime GoalDate { get; private set; }
        public float Progress { get; private set; }
        public decimal Balance { get; private set; }
        
        public SpecificGoalEntity(int id, int userId, int specificId, DateTime extractDate, string description, decimal entryValue, int goalId, string goalName, decimal fullGoalValue, DateTime goalDate, float progress, decimal balance)
        {
            Id = id;
            UserId = userId;
            SpecificId = specificId;
            ExtractDate = extractDate;
            Description = description;
            EntryValue = entryValue;
            GoalId = goalId;
            GoalName = goalName;
            FullGoalValue = fullGoalValue;
            GoalDate = goalDate;
            Progress = progress;
            Balance = balance;
        }
    }
}
