using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class GoalMapper
    {
        public static GoalDTO ToDTO(RegisterGoalModel model)
        {
            return new GoalDTO
            {
                UserId = model.UserId,
                Id = model.Id,
                Description = model.Description,
                Value = model.Value,
                GoalDate = model.GoalDate,
                Progress = model.Progress
            };
        }
    }
}
