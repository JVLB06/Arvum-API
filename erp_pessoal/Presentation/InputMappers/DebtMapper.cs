using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class DebtMapper
    {
        public static DebtDTO ToDTO(RegisterDebtModel model)
        {
            return new DebtDTO
            {
                Id = model.Id,
                Name = model.Description,
                Value = model.Value,
                ReceiveDate = model.EndDate,
                InitialDate = model.InitDate
            };
        }
    }
}
