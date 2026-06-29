using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISpecificRegistersWriter
    {
        Task<int> CreateMainExtractAsync(ExtractEntity extract);
        Task<int> CreateExpenseExtractAsync(ExtractEntity extract, int entryId);
        Task<int> CreateDebtExtractAsync(ExtractEntity extract, int entryId);
        Task<int> CreateGoalExtractAsync(ExtractEntity extract, int entryId);
        Task<int> CreateInvestmentExtractAsync(ExtractEntity extract, int entryId);
        Task<int> CreateReceiptExtractAsync(ExtractEntity extract, int entryId);
        Task UpdateMainExtractAsync(ExtractEntity extract);
        Task UpdateExpenseExtractAsync(ExtractEntity extract);
        Task UpdateDebtExtractAsync(ExtractEntity extract);
        Task UpdateGoalExtractAsync(ExtractEntity extract);
        Task UpdateInvestmentExtractAsync(ExtractEntity extract);
        Task UpdateReceiptExtractAsync(ExtractEntity extract);
        Task DeleteMainExtractAsync(int id, int userId);
        Task DeleteExpenseExtractAsync(int id, int userId);
        Task DeleteDebtExtractAsync(int id, int userId);
        Task DeleteGoalExtractAsync(int id, int userId);
        Task DeleteInvestmentExtractAsync(int id, int userId);
        Task DeleteReceiptExtractAsync(int id, int userId);
        Task UpdateMultipleBalanceAsync(IEnumerable<ExtractBalanceEntity> balances);
    }
}
