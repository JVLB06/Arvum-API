namespace Infrastructure.BaseModels
{
    public class SpecificDebtBaseModel
    {
        public int Id { get; set; }
        public int SpecificId { get; set; }
        public DateTime ExtractDate { get; set; }
        public string Description { get; set; }
        public decimal EntryValue { get; set; }
        public int DebtId { get; set; }
        public string DebtName { get; set; }
        public decimal DebtValue { get; set; }
        public DateTime DebtDate { get; set; }
        public DateTime DebtEndDate { get; set; }
        public decimal Balance { get; set; }
    }
}
