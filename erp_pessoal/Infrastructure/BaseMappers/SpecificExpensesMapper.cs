using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class SpecificExpensesMapper
    {
        public static SpecificExpensesDTO ToDTO(SpecificExpensesBaseModel model)
        {
            return new SpecificExpensesDTO
            {
                Id = model.Id,
                SpecificId = model.SpecificId,
                ExtractDate = model.ExtractDate,
                Description = model.Description,
                EntryValue = model.EntryValue,
                ExpenseId = model.ExpenseId,
                ExpenseName = model.ExpenseName,
                ExpenseValue = model.ExpenseValue,
                ExpenseDate = model.ExpenseDate,
                Variable = model.Variable,
                Balance = model.Balance
            };
        }
    }
}
