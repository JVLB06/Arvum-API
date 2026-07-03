namespace Infrastructure.BaseModels
{
    public class SpecificInvestmentBaseModel
    {
        public int Id { get; set; }
        public int SpecificId { get; set; }
        public DateTime ExtractDate { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public int InvestId { get; set; }
        public string InvestName { get; set; }
        public decimal InvestValue { get; set; }
        public float Interest { get; set; }
        public DateTime InvestDate { get; set; }
        public decimal Balance { get; set; }
    }
}
