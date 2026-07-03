using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class PreferenceMapper
    {
        public static PreferenceDTO ToDTO(PreferenceBaseModel model)
        {
            return new PreferenceDTO
            {
                UserId = model.UserId,
                Id = model.Id,
                ExternalId = model.ExternalId,
                Exclude = model.Exclude,
                Reduce = model.Reduce,
                Block = model.Block,
                Name = model.Name
            };
        }
    }
}
