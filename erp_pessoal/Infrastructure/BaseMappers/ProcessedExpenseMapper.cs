using Infrastructure.BaseModels;
using Application.DTOs;

namespace Infrastructure.BaseMappers
{
    public class ProcessedExpenseMapper
    {
        public static ProcessedExpenseDTO ToDTO(ProcessedExpenseBaseModel model) {
            return new ProcessedExpenseDTO
            {
                Month = model.Month,
                TotalExpenses = model.TotalExpenses,
                TotalIncomes = model.TotalIncomes,
                ExpensesLimit = model.ExpensesLimit
            };
        }
    }
}
