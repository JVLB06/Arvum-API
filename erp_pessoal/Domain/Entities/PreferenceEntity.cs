namespace Domain.Entities
{
    public class PreferenceEntity
    {
        public int? UserId { get; private set; }
        public int? Id { get; private set; }
        public int ExternalId { get; private set; }
        public bool Exclude { get; private set; }
        public bool Reduce { get; private set; }
        public bool Block { get; private set; }
        public string? Name { get; private set; }

        public PreferenceEntity(int? userId, int? id, int externalId, bool exclude, bool reduce, bool block, string? name)
        {
            UserId = userId;
            Id = id;
            ExternalId = externalId;
            Exclude = exclude;
            Reduce = reduce;
            Block = block;
            Name = name;
        }
    }
}
