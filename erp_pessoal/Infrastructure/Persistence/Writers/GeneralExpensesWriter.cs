using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Writers
{
    public class GeneralExpensesWriter : IGeneralExpensesWriter
    {
        public async Task CreateExpenseAsync(ExpenseEntity expense)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"INSERT INTO 
                                    gastos 
                                    (user_id, nome, vlr_min, vlr_max, data_venc, prioridade, fixvar, ativo) 
                                VALUES 
                                    (@UserId, @Description, @MinValue, @MaxValue, @DueDate, @Priority, @IsFixed, TRUE);";

            await conn.ExecuteAsync(sql, new
            {
                expense.UserId,
                expense.Description,
                expense.MinValue,
                expense.MaxValue,
                expense.DueDate,
                expense.Priority,
                expense.IsFixed
            });
        }

        public async Task UpdateExpenseAsync(ExpenseEntity expense)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                                UPDATE 
                                    gastos 
                                SET 
                                    nome = @Description, 
                                    vlr_min = @MinValue, 
                                    vlr_max = @MaxValue, 
                                    data_venc = @DueDate, 
                                    prioridade = @Priority, 
                                    fixvar = @IsFixed 
                                WHERE 1=1
                                    AND id_gasto = @Id;";

            await conn.ExecuteAsync(sql, new
            {
                expense.Id,
                expense.Description,
                expense.MinValue,
                expense.MaxValue,
                expense.DueDate,
                expense.Priority,
                expense.IsFixed
            });
        }

        public async Task DeleteExpenseAsync(int expenseId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                                UPDATE 
                                    gastos 
                                SET 
                                    ativo = FALSE 
                                WHERE 1=1
                                    AND id_gasto = @ExpenseId;";

            await conn.ExecuteAsync(sql, new { ExpenseId = expenseId });
        }
    }
}
