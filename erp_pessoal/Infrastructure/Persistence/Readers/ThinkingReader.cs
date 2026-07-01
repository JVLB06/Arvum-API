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
                        AND RU.USER_ID = @user
                        AND RU.ATIVO = TRUE;";

            var query = await conn.QueryAsync<PreferenceBaseModel>(sql, new { userId});

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
                    AND USER_ID = @user
                    AND GASTO_ID = @gasto
                LIMIT 1;";

            var query = await conn.QueryFirstOrDefaultAsync<PreferenceBaseModel>(sql, new
            {
                userId,
                mainId
            });

            if (query is null)
                return null;

            return (PreferenceDTO)PreferenceMapper.ToDTO(query);
        }
    }
}
