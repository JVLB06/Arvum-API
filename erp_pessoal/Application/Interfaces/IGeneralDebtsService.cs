using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralDebtsService
    {
        Task<IEnumerable<DebtEntity>> GetDebtsAsync(int id);
    }
}
