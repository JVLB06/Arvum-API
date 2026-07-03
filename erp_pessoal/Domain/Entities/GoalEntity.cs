namespace Domain.Entities
{
    public class GoalEntity
    {
        public int? UserId { get; private set; }
        public int? Id { get; private set; }
        public string Description { get; private set; }
        public decimal Value { get; private set; }
        public DateTime GoalDate { get; private set; }
        public decimal Progress { get; private set; }

        public GoalEntity(int? userId, int? id, string description, decimal value, DateTime goalDate, decimal progress)
        {
            UserId = userId;
            Id = id;
            Description = description;
            Value = value;
            GoalDate = goalDate;
            Progress = progress;
        }
    }
}
