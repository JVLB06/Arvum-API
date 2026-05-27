namespace Domain.Entities
{
    public class DeleteInvestmentEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public DeleteInvestmentEntity(int id, int userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}
