namespace Infrastructure.BaseModels
{
    public class ExtractBaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime ExtractDate { get; set; }
        public string Kind { get; set; }
        public decimal Balance { get; set; }
    }
}
