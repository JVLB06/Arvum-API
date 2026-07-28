using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class GeneralInfoMapper
    {
        public static GeneralInfoDTO ToDTO(GeneralInfoBaseModel model)
        {
            if (model == null) return null;

            return new GeneralInfoDTO
            {
                Total = model.Total,
                Kind = model.Kind
            };
        }
    }
}
