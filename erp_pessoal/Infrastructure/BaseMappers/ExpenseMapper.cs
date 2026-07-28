using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public class ExpenseMapper
    {
        public static ExpenseDTO ToDTO(ExpenseBaseModel model)
        {
            if (model == null) return null;

            return new ExpenseDTO
            {
                Id = model.Id,
                MaxValue = model.MaxValue,
                MinValue = model.MinValue,
                Description = model.Description,
                DueDate = model.DueDate,
                UserId = model.UserId
            };
        }
    }
}
