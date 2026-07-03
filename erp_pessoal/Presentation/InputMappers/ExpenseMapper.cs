using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class ExpenseMapper
    {
        public static ExpenseDTO ToDTO(RegisterExpenseModel model)
        {
            return new ExpenseDTO
            {
                UserId = model.UserId,
                Id = model.Id,
                Description = model.Description,
                MinValue = model.MinValue,
                MaxValue = model.MaxValue,
                Priority = model.Priority,
                DueDate = model.DueDate,
                IsFixed = model.IsFixed
            };
        }
    }
}
