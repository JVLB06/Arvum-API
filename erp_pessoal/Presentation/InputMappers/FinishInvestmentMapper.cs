using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class FinishInvestmentMapper
    {
        public static FinishInvestmentDTO ToDTO(RegisterFinishInvestmentModel model)
        {
            return new FinishInvestmentDTO
            {
                Id = model.Id,
                ReceiveDate = model.ReceiveDate,
                ReceivedValue = model.ReceivedValue
            };
        }
    }
}
