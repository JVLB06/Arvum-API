using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class InvestmentMapper
    {
        public static InvestmentDTO ToDto(RegisterInvestmentModel model)
        {
            return new InvestmentDTO
            {
                Description = model.Description,
                Value = model.Value,
                Interest = model.Interest,
                InitialDate = model.InitialDate
            };
        }
    }
}
