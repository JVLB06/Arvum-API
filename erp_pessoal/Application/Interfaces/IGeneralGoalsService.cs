using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralGoalsService
    {
            Task<IEnumerable<GoalEntity>> GetActiveGoalsAsync(int userId);
            Task RegisterGoalAsync(GoalEntity goal, int userId);
            Task UpdateGoalAsync(GoalEntity goal, int userId);
            Task DeleteGoalAsync(int id);
            Task EndGoalAsync(int id);
            Task<IEnumerable<GoalEntity>> GetDoneGoalsAsync(int userId);
    }
}
