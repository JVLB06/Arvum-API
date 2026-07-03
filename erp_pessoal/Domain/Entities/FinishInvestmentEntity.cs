using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FinishInvestmentEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public DateTime ReceiveDate { get; private set; }
        public decimal ReceivedValue { get; private set; }

        public FinishInvestmentEntity(int id, int userId, DateTime receiveDate, decimal receivedValue)
        {
            Id = id;
            UserId = userId;
            ReceiveDate = receiveDate;
            ReceivedValue = receivedValue;
        }
    }
}
