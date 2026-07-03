using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGeneralGoalsReader
    {
        Task<IEnumerable<GoalDTO>> GetActiveGoalsAsync(int userId);
        Task<IEnumerable<GoalDTO>> GetInactiveGoalsAsync(int userId);
    }
}
