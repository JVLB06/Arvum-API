namespace Presentation.WebModels
{
    public class PreferenceModel
    {
        public int ExternalId { get; set; }
        public bool Exclude { get; set; }
        public bool Reduce { get; set; }
        public bool Block { get; set; }
    }
}
