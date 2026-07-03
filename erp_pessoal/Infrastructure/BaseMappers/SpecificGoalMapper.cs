using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class SpecificGoalMapper
    {
        public static SpecificGoalDTO ToDTO(SpecificGoalBaseModel model)
        {
            return new SpecificGoalDTO
            {
                Id = model.Id,
                SpecificId = model.SpecificId,
                ExtractDate = model.ExtractDate,
                Description = model.Description,
                EntryValue = model.EntryValue,
                GoalId = model.GoalId,
                GoalName = model.GoalName,
                FullGoalValue = model.FullGoalValue,
                GoalDate = model.GoalDate,
                Progress = model.Progress,
                Balance = model.Balance
            };
        }
    }
}
