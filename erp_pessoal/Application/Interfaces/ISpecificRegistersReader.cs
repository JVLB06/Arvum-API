using Application.DTOs;

namespace Application.Interfaces
{
    public interface ISpecificRegistersReader
    {
        Task<IEnumerable<ExtractDTO>> ReadExtractByUser(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificGoalDTO>> ReadGoalEntryByUser(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificExpensesDTO>> ReadExpenseEntryByUser(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificDebtDTO>> ReadDebtsEntryByUser(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificReceiptDTO>> ReadReceiptsEntryByUser(int userId, DateTime initialDate, DateTime endDate);
        Task<IEnumerable<SpecificInvestmentDTO>> ReadInvestmentsEntryByUser(int userId, DateTime initialDate, DateTime endDate);
    }
}
