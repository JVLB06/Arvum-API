using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralDebtsService
    {
        Task<IEnumerable<DebtEntity>> GetDebtsAsync(int id);
        Task RegisterDebtAsync(DebtDTO debt, int userId);
        Task UpdateDebtAsync(DebtDTO debt, int userId);
        Task DeleteDebtAsync(int id);
        Task PayDebtAsync(int id);
        Task<IEnumerable<DebtEntity>> GetPaidDebtsAsync(int userId);
    }
}
