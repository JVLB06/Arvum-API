using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Writers
{
    public class ThinkingWriter : IThinkingWriter
    {
        public async Task SetPreferenceAsync(PreferenceEntity preference)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                INSERT INTO RESTRICOES_USUARIO
                (
                    USER_ID,
                    GASTO_ID,
                    EXCLUIR,
                    REDUZIR,
                    BLOQUEAR,
                    ATIVO)
                VALUES
                (
                    @UserId,
                    @ExternalId,
                    @Exclude,
                    @Reduce,
                    @Block,
                    TRUE);";

            await conn.ExecuteAsync(sql, new { preference.UserId, preference.ExternalId, preference.Exclude, preference.Reduce, preference.Block });
        }

        public async Task PutPreferenceAsync(PreferenceEntity preference)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                UPDATE RESTRICOES_USUARIO
                SET
                    EXCLUIR = @Exclude,
                    REDUZIR = @Reduce,
                    BLOQUEAR = @Block,
                    ATIVO = TRUE
                WHERE
                    USER_ID = @UserId
                    AND ID_PREF = @Id
                    AND GASTO_ID = @ExternalId;";

            await conn.ExecuteAsync(sql, new { preference.UserId, preference.ExternalId, preference.Exclude, preference.Reduce, preference.Block, preference.Id });
        }

        public async Task DeletePreferenceAsync(int userId, int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                UPDATE RESTRICOES_USUARIO
                SET 
                    ATIVO = FALSE,
                    EXCLUIR = FALSE,
                    REDUZIR = FALSE
                WHERE 
                    USER_ID = @userId
                    AND ID_PREF = @id;";

            await conn.ExecuteAsync(sql, new { userId, id });
        }
    }
}