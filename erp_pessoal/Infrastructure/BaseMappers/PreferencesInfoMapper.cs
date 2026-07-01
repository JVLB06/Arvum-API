using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class PreferencesInfoMapper
    {
        public static PreferencesInfoDTO ToDTO(PreferencesInfoBaseModel model)
        {
            return new PreferencesInfoDTO
            {
                Id = model.Id,
                Name = model.Name,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue
            };
        }
    }
}
