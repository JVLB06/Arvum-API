namespace Presentation.WebModels
{
    public class RegisterExpenseModel
    {
        public int? UserId { get; set; }
        public int? Id { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsFixed { get; set; }
    }
}
