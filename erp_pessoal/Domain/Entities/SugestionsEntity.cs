namespace Domain.Entities
{
    public class SugestionsEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public decimal MediumValue { get; private set; }

        public SugestionsEntity(int id, string name, decimal minValue, decimal maxValue)
        {
            Id = id;
            Name = name;
            MediumValue = (minValue+maxValue)/2;
        }
    }
}
