using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class GoalMapper
    {
        public static GoalDTO ToDTO(GoalBaseModel model)
        {
            if (model == null) return null;

            return new GoalDTO
            {
                Id = model.Id,
                Description = model.Description,
                GoalDate = model.GoalDate,
                Progress = model.Progress,
                UserId = model.UserId,
                Value = model.Value
            };
        }
    }
}
