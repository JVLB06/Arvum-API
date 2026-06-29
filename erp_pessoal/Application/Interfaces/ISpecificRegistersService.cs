using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISpecificRegistersService
    {
        Task<IEnumerable<ExtractEntity>> GetExtractAsync(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificGoalEntity>> GetGoalPaymentsAsync(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificExpensesEntity>> GetExpensePayementsAsync(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificDebtEntity>> GetDebtPaymentsAsync(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificInvestmentEntity>> GetInvestmentPaymentsAsync(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificReceiptEntity>> GetReceiptPaymentsAsync(int userId, DateTime initialDate, DateTime endDate);
        Task<int> SetExtractAsync(ExtractDTO main, int userId);
        Task UpdateExtractAsync(ExtractDTO main, int userId);
        Task DeleteExtractAsync(ExtractDeleteDTO main, int userId);
        Task CalculateBalancesAsync(int userId, int entryId);
    }
}
