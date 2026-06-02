using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralGoalsService
    {
            Task<IEnumerable<GoalEntity>> GetActiveGoalsAsync(int userId);
            Task RegisterGoalAsync(GoalDTO goal, int userId);
            Task UpdateGoalAsync(GoalDTO goal, int userId);
            Task DeleteGoalAsync(int id);
            Task EndGoalAsync(int id);
            Task<IEnumerable<GoalEntity>> GetDoneGoalsAsync(int userId);
    }
}
