using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;
using Npgsql;

namespace Infrastructure.Persistence.Readers
{
    public class GeneralExpensesReader : IGeneralExpensesReader
    {
        public async Task<IEnumerable<ExpenseDTO>> ReadExpensesAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                            SELECT 
                                id_gasto AS Id,
                                nome AS Description,        
                                vlr_min AS MinValue,
                                vlr_max AS MaxValue,
                                prioridade AS Priority,
                                data_venc AS DueDate,
                                fixvar AS IsFixed
                            FROM 
                                gastos 
                            WHERE 1=1 
                                AND user_id = @userId
                                AND ativo = TRUE;";

            var results = await conn.QueryFirstOrDefaultAsync<ExpenseBaseModel>(
                sql, userId);

            if (results is null)
                return null;

            return (IEnumerable<ExpenseDTO>)ExpenseMapper.ToDTO(results);
        }
    }
}
