using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class SpecificDebtMapper
    {
        public static SpecificDebtDTO ToDTO(SpecificDebtBaseModel model)
        {
            return new SpecificDebtDTO
            {
                Id = model.Id,
                SpecificId = model.SpecificId,
                ExtractDate = model.ExtractDate,
                EntryValue = model.EntryValue,
                Description = model.Description,
                DebtId = model.DebtId,
                DebtName = model.DebtName,
                DebtValue = model.DebtValue,
                DebtDate = model.DebtDate,
                DebtEndDate = model.DebtEndDate,
                Balance = model.Balance
            };
        }
    }
}
