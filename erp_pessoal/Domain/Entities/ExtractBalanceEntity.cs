namespace Domain.Entities
{
    public class ExtractBalanceEntity
    {
        public int Id { get; private set; }
        public decimal Balance { get; private set; }

        public ExtractBalanceEntity(int id, decimal balance)
        {
            Id = id;
            Balance = balance;
        }
    }
}
