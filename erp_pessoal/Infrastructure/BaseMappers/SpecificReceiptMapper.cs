using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class SpecificReceiptMapper
    {
        public static SpecificReceiptDTO ToDTO(SpecificReceiptBaseModel model)
        {
            if (model == null) return null;

            return new SpecificReceiptDTO
            {
                Id = model.Id,
                SpecificId = model.SpecificId,
                ExtractDate = model.ExtractDate,
                Description = model.Description,
                Value = model.Value,
                ReceiptId = model.ReceiptId,
                ReceiptValue = model.ReceiptValue,
                ReceiptDate = model.ReceiptDate,
                Balance = model.Balance,
            };
        }
    }
}
