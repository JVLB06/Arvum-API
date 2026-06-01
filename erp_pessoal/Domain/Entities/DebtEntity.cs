namespace Domain.Entities
{
    public class DebtEntity
    {
        public int? UserId { get; private set; }
        public int? Id { get; private set; }
        public string Name { get; private set; }
        public decimal Value { get; private set; }
        public DateTime InitialDate { get; private set; }
        public DateTime ReceiveDate { get; private set; }
        public bool Paid { get; private set; }

        public DebtEntity(int? id,string name,decimal value,DateTime initialDate,DateTime receiveDate,bool paid, int? userId)
        {
            UserId = userId;
            Id = id;
            Name = name;
            Value = value;
            InitialDate = initialDate;
            ReceiveDate = receiveDate;
            Paid = paid;
        }
    }
}
