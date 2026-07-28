using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class ThinkingReader : IThinkingReader
    {
        #region Preferences
        public async Task<IEnumerable<PreferenceDTO>> ReadPreferencesAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                     SELECT 
                         RU.USER_ID AS UserId,
                         RU.ID_PREF AS Id,
                         RU.GASTO_ID AS ExternalId,
                         RU.EXCLUIR AS Exclude,
                         RU.REDUZIR AS Reduce,
                         RU.BLOQUEADO AS Block,
                         G.NOME AS ExpenseName
                     FROM 
                        RESTRICOES_USUARIO RU
                     INNER JOIN 
                        GASTOS G ON RU.GASTO_ID = G.ID_GASTO 
                     WHERE 1=1
                        AND RU.USER_ID = @userId
                        AND RU.ATIVO = TRUE;";

            var query = await conn.QueryAsync<PreferenceBaseModel>(sql, new { userId });

            if (query is null || !query.Any())
                return Enumerable.Empty<PreferenceDTO>();

            return query.Select(item => PreferenceMapper.ToDTO(item));
        }

        public async Task<PreferenceDTO> ReadPreferenceAsync(int userId, int mainId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT
                    USER_ID AS UserId,
                    ID_PREF AS Id,
                    GASTO_ID AS ExternalId,
                    EXCLUIR AS Exclude,
                    REDUZIR AS Reduce,
                    BLOQUEADO AS Block
                FROM 
                    RESTRICOES_USUARIO
                WHERE 1=1
                    AND USER_ID = @userId
                    AND GASTO_ID = @mainId
                LIMIT 1;";

            var query = await conn.QueryFirstOrDefaultAsync<PreferenceBaseModel>(sql, new
            {
                userId,
                mainId
            });

            if (query is null)
                return null;

            return PreferenceMapper.ToDTO(query);
        }
        #endregion

        #region Preferences_Utils
        public async Task<IEnumerable<GeneralInfoDTO>> ReadDebtTotalAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    COALESCE(SUM(vlr), 0) AS Total
                FROM 
                    divida
                WHERE 
                    user_id = @userId
                    AND ativo = TRUE;";

            var query = await conn.QueryAsync<GeneralInfoBaseModel>(sql, new { userId });

            if (query is null || !query.Any())
                return Enumerable.Empty<GeneralInfoDTO>();

            return query.Select(item => GeneralInfoMapper.ToDTO(item));
        }

        public async Task<IEnumerable<GeneralInfoDTO>> ReadReceiptTotalAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    COALESCE(SUM((vlr_min + vlr_max) / 2), 0) AS Total
                FROM 
                    rendas
                WHERE 
                    user_id = @userId
                    AND ativo = TRUE;";

            var query = await conn.QueryAsync<GeneralInfoBaseModel>(sql, new { userId });

            if (query is null || !query.Any())
                return Enumerable.Empty<GeneralInfoDTO>();

            return query.Select(item => GeneralInfoMapper.ToDTO(item));
        }

        public async Task<IEnumerable<GeneralInfoDTO>> ReadExpensesTotalAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT
                    COALESCE(SUM((vlr_min + vlr_max) / 2), 0) AS Total,
                    CASE fixvar 
                        WHEN TRUE THEN 'Fix'
                        WHEN FALSE THEN 'Var'
                    END AS Kind
                FROM 
                    gastos
                WHERE 1=1
                    AND user_id = @userId
                    AND ativo = TRUE
                GROUP BY fixvar;";

            var query = await conn.QueryAsync<GeneralInfoBaseModel>(sql, new { userId });

            if (query is null || !query.Any())
                return Enumerable.Empty<GeneralInfoDTO>();

            return query.Select(item => GeneralInfoMapper.ToDTO(item));
        }

        public async Task<IEnumerable<PreferencesInfoDTO>> ReadExclusionsAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT
                    G.id_gasto AS Id,
                    G.nome AS Name,
                    G.vlr_min AS MinValue,
                    G.vlr_max AS MaxValue
                FROM 
                    gastos G
                LEFT JOIN 
                    RESTRICOES_USUARIO RU ON RU.gasto_id = G.id_gasto
                WHERE 
                    G.user_id = @userId
                    AND (RU.excluir IS NOT TRUE)
                    AND G.ativo = TRUE
                    AND G.prioridade >= 4;";

            var query = await conn.QueryAsync<PreferencesInfoBaseModel>(sql, new { userId });

            if (query is null || !query.Any())
                return Enumerable.Empty<PreferencesInfoDTO>();

            return query.Select(item => PreferencesInfoMapper.ToDTO(item));
        }

        public async Task<IEnumerable<PreferencesInfoDTO>> ReadReductionsAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT
                    G.id_gasto AS Id,
                    G.nome AS Name,
                    G.vlr_min AS MinValue,
                    G.vlr_max AS MaxValue
                FROM 
                    gastos G
                LEFT JOIN 
                    RESTRICOES_USUARIO RU ON RU.gasto_id = G.id_gasto
                WHERE 
                    G.user_id = @userId
                    AND G.ativo = TRUE
                    AND (RU.reduzir IS NOT TRUE)
                    AND G.prioridade >= 4;";

            var query = await conn.QueryAsync<PreferencesInfoBaseModel>(sql, new { userId });

            if (query is null || !query.Any())
                return Enumerable.Empty<PreferencesInfoDTO>();

            return query.Select(item => PreferencesInfoMapper.ToDTO(item));
        }
        #endregion
    }
}