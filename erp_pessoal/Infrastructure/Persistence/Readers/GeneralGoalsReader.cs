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
                                    m.id_meta AS Id,
                                    m.user_id AS UserId,
                                    m.nome AS Description,
                                    m.vlr AS Value,
                                    m.data_meta AS GoalDate,
                                    COALESCE(
                                        ROUND((COALESCE(SUM(mp.vlr), 0) / NULLIF(m.vlr, 0)) * 100, 2), 
                                        m.progresso, 
                                        0
                                    ) AS Progress,
                                    COALESCE(SUM(mp.vlr), 0) AS GoalPaid
                                FROM meta m 
                                LEFT JOIN meta_pgto mp ON mp.meta_invest_id = m.id_meta AND mp.ativo = TRUE
                                WHERE 1=1
                                    AND m.user_id = @userId 
                                    AND m.ativo = TRUE
                                GROUP BY m.id_meta, m.user_id, m.nome, m.vlr, m.data_meta, m.progresso
                                ORDER BY m.data_meta ASC";

            var results = await conn.QueryAsync<GoalBaseModel>(sql, new { userId });

            if (results is null || !results.Any())
                return Enumerable.Empty<GoalDTO>();

            return results.Select(item => GoalMapper.ToDTO(item));
        }

        public async Task<IEnumerable<GoalDTO>> GetInactiveGoalsAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"SELECT 
                                    m.id_meta AS Id,
                                    m.user_id AS UserId,
                                    m.nome AS Description,
                                    m.vlr AS Value,
                                    m.data_meta AS GoalDate,
                                    m.progresso AS Progress,
                                    COALESCE(SUM(mp.vlr), 0) AS GoalPaid
                                FROM meta m 
                                LEFT JOIN meta_pgto mp ON mp.meta_invest_id = m.id_meta AND mp.ativo = TRUE
                                WHERE 1=1
                                    AND m.user_id = @userId 
                                    AND m.ativo = FALSE
                                    AND m.progresso >= 100
                                GROUP BY m.id_meta, m.user_id, m.nome, m.vlr, m.data_meta, m.progresso
                                ORDER BY m.data_meta DESC";

            var results = await conn.QueryAsync<GoalBaseModel>(sql, new { userId });

            if (results is null || !results.Any())
                return Enumerable.Empty<GoalDTO>();

            return results.Select(item => GoalMapper.ToDTO(item));
        }

        public async Task<GoalCompositionDTO?> GetGoalCompositionAsync(int userId, int goalId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sqlGoal = @"
                SELECT 
                    id_meta AS GoalId,
                    nome AS GoalName,
                    vlr AS TargetValue,
                    data_meta AS GoalDate
                FROM meta
                WHERE id_meta = @goalId AND user_id = @userId AND ativo = TRUE;";

            var goal = await conn.QueryFirstOrDefaultAsync<GoalCompositionDTO>(sqlGoal, new { goalId, userId });
            if (goal is null) return null;

            const string sqlItems = @"
                SELECT 
                    COALESCE(d.id_invest, i.id_invest, mp.id_pgto_meta) AS Id,
                    COALESCE(d.nome, i.nome, mp.historico, 'Aporte Direto') AS Name,
                    SUM(mp.vlr) AS TotalContributed,
                    CASE 
                        WHEN dp.lcto_id IS NOT NULL THEN 'divida'
                        WHEN ip.lcto_id IS NOT NULL THEN 'investimento'
                        ELSE 'meta'
                    END AS Type
                FROM meta_pgto mp
                LEFT JOIN divida_pgto dp ON dp.lcto_id = mp.lcto_id AND dp.ativo = TRUE
                LEFT JOIN divida d ON d.id_invest = dp.divida_id
                LEFT JOIN investimento_pgto ip ON ip.lcto_id = mp.lcto_id AND ip.ativo = TRUE
                LEFT JOIN investimentos i ON i.id_invest = ip.invest_id
                WHERE mp.meta_invest_id = @goalId AND mp.user_id = @userId AND mp.ativo = TRUE
                GROUP BY 
                    COALESCE(d.id_invest, i.id_invest, mp.id_pgto_meta),
                    COALESCE(d.nome, i.nome, mp.historico, 'Aporte Direto'),
                    CASE 
                        WHEN dp.lcto_id IS NOT NULL THEN 'divida'
                        WHEN ip.lcto_id IS NOT NULL THEN 'investimento'
                        ELSE 'meta'
                    END;";

            var items = await conn.QueryAsync<GoalCompositionItemDTO>(sqlItems, new { goalId, userId });
            goal.Items = items.ToList();
            goal.TotalAccumulated = goal.Items.Sum(x => x.TotalContributed);
            goal.ProgressPercentage = goal.TargetValue > 0 
                ? Math.Round((goal.TotalAccumulated / goal.TargetValue) * 100, 2) 
                : 0;

            return goal;
        }
    }
}