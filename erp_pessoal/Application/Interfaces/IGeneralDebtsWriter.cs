using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralDebtsWriter
    {
        Task CreateDebtAsync(DebtEntity debt);
        Task UpdateDebtAsync(DebtEntity debt);
        Task InactivateDebtAsync(int debtId);
        Task PayDebtAsync(int debtId);
    }
}
