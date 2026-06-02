namespace Application.Interfaces
{
    public interface IGeneralGoalsWriter
    {
        Task CreateGoalAsync(GoalEntity goal);
        Task UpdateGoalAsync(GoalEntity goal);
        Task DeleteGoalAsync(int id);
        Task EndGoalAsync(int id);
    }
}
