namespace Application.DTOs
{
    public class InvestmentDTO
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public decimal Interest { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public decimal? ReceivedValue { get; set; }
    }
}
