using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class GeneralExpensesService : IGeneralExpensesService
    {
        private readonly IGeneralExpensesReader _reader;
        private readonly IGeneralExpensesWriter _writer;

        public GeneralExpensesService(
            IGeneralExpensesReader reader,
            IGeneralExpensesWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<IEnumerable<ExpenseEntity>> GetExpensesAsync(int userId)
        {
            var connect = await _reader.ReadExpensesAsync(userId);
            return connect.Select(expense => new ExpenseEntity(
                userId,
                expense.Id,
                expense.Description,
                expense.MinValue,
                expense.MaxValue,
                expense.Priority,
                expense.DueDate,
                expense.IsFixed
            ));
        }

        public async Task RegisterExpenseAsync(ExpenseDTO expense, int userId)
        {
            await _writer.CreateExpenseAsync(new ExpenseEntity(
                userId,
                expense.Id,
                expense.Description,
                expense.MinValue,
                expense.MaxValue,
                expense.Priority,
                expense.DueDate,
                expense.IsFixed
            ));
        }

        public async Task UpdateExpenseAsync(ExpenseDTO expense, int userId)
        {
            await _writer.UpdateExpenseAsync(new ExpenseEntity(
                userId,
                expense.Id,
                expense.Description,
                expense.MinValue,
                expense.MaxValue,
                expense.Priority,
                expense.DueDate,
                expense.IsFixed
            ));
        }

        public async Task DeleteExpenseAsync(int id)
        {
            await _writer.DeleteExpenseAsync(id);
        }

        public async Task<IEnumerable<ProcessedExpenseEntity>> GetExpensesComparativeAsync(int userId)
        {
            var connect = await _reader.ReadExpensesComparativeAsync(userId);
            return connect.Select(expense => new ProcessedExpenseEntity(
                expense.Month,
                expense.TotalIncomes,
                expense.TotalExpenses,
                expense.ExpensesLimit
                ));
        }
    }
}
