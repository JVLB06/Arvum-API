using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Writers
{
    public class GeneralGoalsWriter : IGeneralGoalsWriter
    {
        public async Task CreateGoalAsync(GoalEntity goal)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"INSERT INTO meta (user_id, nome, vlr, data_meta, progresso, ativo)
                                VALUES (@UserId, @Description, @Value, @GoalDate, @Progress, TRUE)";

            await conn.ExecuteAsync(sql, new
            {
                UserId = goal.UserId,
                Description = goal.Description,
                Value = goal.Value,
                GoalDate = goal.GoalDate,
                Progress = goal.Progress
            });
        }

        public async Task UpdateGoalAsync(GoalEntity goal)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE 
                                    meta 
                                 SET 
                                    nome = @Description, 
                                    vlr = @Value, 
                                    data_meta = @GoalDate, 
                                    progresso = @Progress 
                                WHERE 1=1
                                    AND id_meta = @Id 
                                    AND user_id = @UserId;";

            await conn.ExecuteAsync(sql, new
            {
                UserId = goal.UserId,
                Description = goal.Description,
                Value = goal.Value,
                GoalDate = goal.GoalDate,
                Progress = goal.Progress
            });
        }

        public async Task DeleteGoalAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE 
                                    meta 
                                SET 
                                    ativo = FALSE 
                                WHERE 1=1
                                    AND id_meta = @Id;";

            await conn.ExecuteAsync(sql, new {Id = id});
        }

        public async Task EndGoalAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE
                                    meta
                                SET
                                    ativo = FALSE,
                                    progresso = 100
                                WHERE 1=1
                                    AND id_meta = @Id;";

            await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
