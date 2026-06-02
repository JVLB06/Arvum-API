namespace Domain.Entities
{
    public class ExpenseEntity
    {
        public int? UserId { get; private set; }
        public int? Id { get; private set; }
        public string Description { get; private set; }
        public decimal MinValue { get; private set; }
        public decimal MaxValue { get; private set; }
        public int Priority { get; private set; }
        public DateTime DueDate { get; private set; }
        public bool IsFixed { get; private set; }

        public ExpenseEntity(int? userId, int? id, string description, decimal minValue, decimal maxValue, int priority, DateTime dueDate, bool isFixed)
        {
            UserId = userId;
            Id = id;
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
            Priority = priority;
            DueDate = dueDate;
            IsFixed = isFixed;
        }
    }
}
