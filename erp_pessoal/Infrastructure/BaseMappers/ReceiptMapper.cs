using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class ReceiptMapper
    {
        public static ReceiptDTO ToInput(ReceiptBaseModel model)
        {
            return new ReceiptDTO
            {
                Id = model.Id,
                Description = model.Name,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                PaymentDate = model.PaymentDate
            };
        }
    }
}
