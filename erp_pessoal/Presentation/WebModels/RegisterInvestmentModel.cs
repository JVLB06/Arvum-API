namespace Presentation.WebModels
{
    public class RegisterInvestmentModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public decimal Interest { get; set; }
        public DateTime InitialDate { get; set; }
    }
}
