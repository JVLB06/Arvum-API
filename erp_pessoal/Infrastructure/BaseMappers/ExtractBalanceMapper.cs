using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class ExtractBalanceMapper
    {
        public static ExtractBalanceDTO ToDTO (ExtractBalanceBaseModel model)
        {
            return new ExtractBalanceDTO
            {
                Id = model.Id,
                Balance = model.Balance,
            };
        }
    }
}
