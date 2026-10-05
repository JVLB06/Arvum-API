using Application.DTOs;

namespace Application.Interfaces
{
    public interface IGeneralExpensesReader
    {
        Task<IEnumerable<ExpenseDTO>> ReadExpensesAsync(int userId);
        Task<IEnumerable<ProcessedExpenseDTO>> ReadExpensesComparativeAsync(int userId);
    }
}
