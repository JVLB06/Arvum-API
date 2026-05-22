namespace Domain.Entities
{
    public class ConnectionEntity
    {
        public int? Id { get; private set; }
        public string Name { get; private set; }
        public bool Authenticated { get; private set; }

        public ConnectionEntity(int? id, string name)
        {
            Id = id;
            Name = name;

            if (id is null)
            {
                Authenticated = false;
                return;
            }

            Authenticated = true;
        }
    }
}