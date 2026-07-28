using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class ExtractBalanceMapper
    {
        public static ExtractBalanceDTO ToDTO (ExtractBalanceBaseModel model)
        {
            if (model == null) return null;

            return new ExtractBalanceDTO
            {
                Id = model.Id,
                Balance = model.Balance,
            };
        }
    }
}
