using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGeneralExpensesWriter
    {
        Task CreateExpenseAsync(ExpenseEntity expense);
        Task UpdateExpenseAsync(ExpenseEntity expense);
        Task DeleteExpenseAsync(int expenseId);
    }
}
