using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralExpensesService
    {
        Task<IEnumerable<ExpenseEntity>> GetExpensesAsync(int userId);
        Task RegisterExpenseAsync(ExpenseDTO expense, int userId);
        Task UpdateExpenseAsync(ExpenseDTO expense, int userId);
        Task DeleteExpenseAsync(int id);
    }
}
