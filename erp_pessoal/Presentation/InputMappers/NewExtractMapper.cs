using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class NewExtractMapper
    {
        public static ExtractDTO ToInput (NewExtractModel model)
        {
            return new ExtractDTO
            {
                Id = model.Id,
                Name = model.Name,
                Value = model.Value,
                ExtractDate = model.ExtractDate,
                Kind = model.Kind,
                Balance = model.Balance,
                ExternalId = model.ExternalId
            };
        }
    }
}
