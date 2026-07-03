using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class ReceiptMapper
    {
        public static ReceiptDTO ToDTO(RegisterReceiptModel model)
        {
            return new ReceiptDTO
            {
                Description = model.Name,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                PaymentDate = model.PaymentDate
            };
        }
    }
}
