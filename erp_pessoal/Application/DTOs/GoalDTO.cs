namespace Application.DTOs
{
    public class GoalDTO
    {
        public int? UserId { get; set; }
        public int? Id { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime GoalDate { get; set; }
        public decimal Progress { get; set; }
    }
}
