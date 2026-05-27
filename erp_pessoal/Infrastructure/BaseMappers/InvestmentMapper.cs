using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class InvestmentMapper
    {
        public static InvestmentDTO ToDTO(InvestmentBaseModel model)
        {
            return new InvestmentDTO
            {
                Id = model.Id,
                Description = model.Name,
                Value = model.Value,
                InitialDate = model.InitialDate,
                ReceiveDate = model.ReceiveDate,
                ReceivedValue = model.ReceivedValue
            };
        }
    }
}
