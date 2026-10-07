using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class GeneralGoalsService : IGeneralGoalsService
    {
        private readonly IGeneralGoalsReader _generalGoalsReader;
        private readonly IGeneralGoalsWriter _generalGoalsWriter;

        public GeneralGoalsService(
            IGeneralGoalsReader generalGoalsReader,
            IGeneralGoalsWriter generalGoalsWriter
            )
        {
            _generalGoalsReader = generalGoalsReader;
            _generalGoalsWriter = generalGoalsWriter;
        }

        public async Task<IEnumerable<GoalEntity>> GetActiveGoalsAsync(int userId)
        {
            var connect = await _generalGoalsReader.GetActiveGoalsAsync(userId);

            return connect.Select(goal => new GoalEntity(
                goal.UserId,
                goal.Id,
                goal.Description,
                goal.Value,
                goal.GoalDate,
                goal.Progress,
                goal.GoalPaid
            ));
        }

        public async Task<IEnumerable<GoalEntity>> GetGoalsProgressAsync(int userId)
        {
            return await GetActiveGoalsAsync(userId);
        }

        public async Task<GoalCompositionDTO?> GetGoalCompositionAsync(int userId, int goalId)
        {
            return await _generalGoalsReader.GetGoalCompositionAsync(userId, goalId);
        }

        public async Task RegisterGoalAsync(GoalDTO goal, int userId)
        {
            await _generalGoalsWriter.CreateGoalAsync(new GoalEntity(
                userId,
                null,
                goal.Description,
                goal.Value,
                goal.GoalDate,
                goal.Progress
            ));
        }

        public async Task UpdateGoalAsync(GoalDTO goal, int userId)
        {
            await _generalGoalsWriter.UpdateGoalAsync(new GoalEntity(
                goal.Id,
                userId,
                goal.Description,
                goal.Value,
                goal.GoalDate,
                goal.Progress
            ));
        }

        public async Task DeleteGoalAsync(int id)
        {
            await _generalGoalsWriter.DeleteGoalAsync(id);
        }

        public async Task EndGoalAsync(int id)
        {
            await _generalGoalsWriter.EndGoalAsync(id);
        }

        public async Task<IEnumerable<GoalEntity>> GetDoneGoalsAsync(int userId)
        {
            var connect = await _generalGoalsReader.GetInactiveGoalsAsync(userId);
            return connect.Select(debt => new GoalEntity(
                debt.Id,
                debt.UserId,
                debt.Description,
                debt.Value,
                debt.GoalDate,
                debt.Progress
            ));
        }
    }
}
