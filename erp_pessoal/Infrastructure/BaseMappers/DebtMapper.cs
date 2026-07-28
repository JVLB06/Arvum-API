using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class DebtMapper
    {
        public static DebtDTO ToDTO(DebtBaseModel model)
        {
            if (model == null) return null;

            return new DebtDTO
            {
                Id = model.Id,
                Name = model.Name,
                Value = model.Value,
                InitialDate = model.InitialDate,
                ReceiveDate = model.ReceiveDate,
                Paid = model.Paid
            };
        }
    }
}
