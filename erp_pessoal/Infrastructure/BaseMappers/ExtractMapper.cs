using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class ExtractMapper
    {
        public static ExtractDTO ToInput(ExtractBaseModel model)
        {
            return new ExtractDTO
            {
                Id = model.Id,
                Name = model.Name,
                Value = model.Value,
                ExtractDate = model.ExtractDate,
                Kind = model.Kind,
                Balance = model.Balance
            };
        }
    }
}
