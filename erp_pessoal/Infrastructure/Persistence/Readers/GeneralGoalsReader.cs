using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class GeneralGoalsReader : IGeneralGoalsReader
    {
        public async Task<IEnumerable<GoalDTO>> GetActiveGoalsAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"SELECT 
                                    id_meta AS Id,
                                    nome AS Description,
                                    vlr AS Value,
                                    data_meta AS GoalDate,
                                    progresso AS Progress
                                FROM meta 
                                WHERE 1=1
                                    AND UserId = @user_id 
                                    AND ativo = TRUE";

            var results = await conn.QueryFirstOrDefaultAsync<GoalBaseModel>(sql, new { UserId = userId });

            if (results is null)
                return null;

            return (IEnumerable<GoalDTO>)GoalMapper.ToDTO(results);
        }

        public async Task<IEnumerable<GoalDTO>> GetInactiveGoalsAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"SELECT 
                                    id_meta AS Id,
                                    nome AS Description,
                                    vlr AS Value,
                                    data_meta AS GoalDate,
                                    progresso AS Progress
                                FROM meta 
                                WHERE 1=1
                                    AND UserId = @user_id 
                                    AND ativo = FALSE
                                    AND progresso >= 100";

            var results = await conn.QueryFirstOrDefaultAsync<GoalBaseModel>(sql, new { UserId = userId });

            if (results is null)
                return null;

            return (IEnumerable<GoalDTO>)GoalMapper.ToDTO(results);
        }
    }
}
