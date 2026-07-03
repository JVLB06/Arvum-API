using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class ExtractDeleteMapper
    {
        public static ExtractDeleteDTO ToInput(ExtractDeleteModel model)
        {
            return new ExtractDeleteDTO
            {
                Id = model.Id,
                Kind = model.Kind
            };
        }
    }
}
