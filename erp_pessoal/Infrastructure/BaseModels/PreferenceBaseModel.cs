namespace Infrastructure.BaseModels
{
    public class PreferenceBaseModel
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public bool Exclude {  get; set; }
        public bool Reduce { get; set; }
        public bool Block { get; set; }
        public string? Name { get; set; }
    }
}
