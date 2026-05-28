namespace Infrastructure.BaseModels
{
    public class DebtBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime ReceiveDate { get; set; }
        public bool Paid { get; set; }
    }
}
