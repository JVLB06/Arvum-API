using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvestmentEntity
    {
        public int Id { get; private set; }
        public int? UserId { get; private set; }
        public string Description { get; private set; }
        public decimal Value { get; private set; }
        public decimal Interest { get; private set; }
        public DateTime InitialDate { get; private set; }
        public DateTime? ReceiveDate { get; private set; }
        public decimal? ReceivedValue { get; private set; }

        public InvestmentEntity(int? id, string description, decimal value, decimal interest, DateTime initialDate, DateTime? receiveDate, decimal? receivedValue, int? userId)
        {
            Id = (int)(id == null ? 0 : id);
            Description = description;
            Value = value;
            Interest = interest;
            InitialDate = initialDate;
            ReceiveDate = receiveDate;
            ReceivedValue = receivedValue;
            UserId = userId;
        }
    }
}
