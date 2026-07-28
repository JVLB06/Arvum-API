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
                                    AND user_id = @userId 
                                    AND ativo = TRUE";

            var results = await conn.QueryAsync<GoalBaseModel>(sql, new { userId });

            if (results is null || !results.Any())
                return Enumerable.Empty<GoalDTO>();

            return results.Select(item => GoalMapper.ToDTO(item));
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
                                    AND user_id = @userId 
                                    AND ativo = FALSE
                                    AND progresso >= 100";

            var results = await conn.QueryAsync<GoalBaseModel>(sql, new { userId });

            if (results is null || !results.Any())
                return Enumerable.Empty<GoalDTO>();

            return results.Select(item => GoalMapper.ToDTO(item));
        }
    }
}