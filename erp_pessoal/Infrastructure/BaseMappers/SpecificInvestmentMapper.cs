using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class SpecificInvestmentMapper
    {
        public static SpecificInvestmentDTO ToDTO(SpecificInvestmentBaseModel model)
        {
            return new SpecificInvestmentDTO
            {
                Id = model.Id,
                SpecificId = model.Id,
                ExtractDate = model.ExtractDate,
                Description = model.Description,
                Value = model.Value,
                InvestId = model.InvestId,
                InvestValue = model.InvestValue,
                InvestName = model.InvestName,
                InvestDate  = model.InvestDate,
                Balance = model.Balance
            };
        }
    }
}
