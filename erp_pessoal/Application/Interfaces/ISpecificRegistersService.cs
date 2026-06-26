using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISpecificRegistersService
    {
        Task<IEnumerable<ExtractEntity>> GetExtractAsync(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificGoalEntity>> GetGoalPaymentsAsync(int userId, DateTime initialDate, DateTime endDate);
    }
}
