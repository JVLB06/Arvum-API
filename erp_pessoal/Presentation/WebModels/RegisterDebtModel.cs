namespace Presentation.WebModels
{
    public class RegisterDebtModel
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime InitDate { get; set; }
    }
}
