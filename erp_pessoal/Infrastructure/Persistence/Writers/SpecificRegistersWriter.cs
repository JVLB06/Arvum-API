using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Writers
{
    public class SpecificRegistersWriter : ISpecificRegistersWriter
    {
        #region Create
        public async Task<int> CreateMainExtractAsync(ExtractEntity extract)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                INSERT INTO 
                    extrato 
                    (data, historico, vlr, saldo, ativo, user_id)
                VALUES 
                    (@ExtractDate, @Name, @Value, 0, TRUE, @UserId)
                RETURNING id_lcto;";

            return await conn.ExecuteAsync(sql, new
            {
                extract.ExtractDate,
                extract.Name,
                extract.Value,
                extract.UserId
            });
        }

        public async Task<int> CreateExpenseExtractAsync (ExtractEntity extract, int entryId)
        {
            using var conn = MainRepository.CreateConnection();
            const string sql = @"
                        INSERT INTO 
                            pagamentos 
                            (historico, vlr, data, gasto_id, user_id, lcto_id)
                        VALUES 
                            (@Name, @Value, @ExtractDate, @ExternalId, @UserId, @entryId)
                        RETURNING id_gasto_geral;";

            return await conn.ExecuteAsync(sql, new
            {
                extract.Name,
                extract.Value,
                extract.ExtractDate,
                extract.ExternalId,
                extract.UserId,
                entryId
            });
        }

        public async Task<int> CreateDebtExtractAsync(ExtractEntity extract, int entryId)
        {
            using var conn = MainRepository.CreateConnection();
            const string sql = @"
                        INSERT INTO 
                            divida_pgto 
                            (historico, vlr, data, invest_id, user_id, lcto_id)
                        VALUES 
                            (@Name, @Value, @ExtractDate, @ExternalId, @UserId, @entryId)
                        RETURNING id_pgto_divida;";

            return await conn.ExecuteAsync(sql, new
            {
                extract.Name,
                extract.Value,
                extract.ExtractDate,
                extract.ExternalId,
                extract.UserId,
                entryId
            });
        }

        public async Task<int> CreateGoalExtractAsync(ExtractEntity extract, int entryId)
        {
            using var conn = MainRepository.CreateConnection();
            const string sql = @"
                        INSERT INTO 
                            meta_pgto 
                            (historico, vlr, data, meta_invest_id, user_id, lcto_id)
                        VALUES 
                            (@Name, @Value, @ExtractDate, @ExternalId, @UserId, @entryId)
                        RETURNING id_pgto_meta;";

            return await conn.ExecuteAsync(sql, new
            {
                extract.Name,
                extract.Value,
                extract.ExtractDate,
                extract.ExternalId,
                extract.UserId,
                entryId
            });
        }

        public async Task<int> CreateInvestmentExtractAsync(ExtractEntity extract, int entryId)
        {
            using var conn = MainRepository.CreateConnection();
            const string sql = @"
                        INSERT INTO 
                            investimento_pgto 
                            (historico, vlr, data, invest_id, user_id, lcto_id)
                        VALUES 
                            (@Name, @Value, @ExtractDate, @ExternalId, @UserId, @entryId)
                        RETURNING id_invest;";

            return await conn.ExecuteAsync(sql, new
            {
                extract.Name,
                extract.Value,
                extract.ExtractDate,
                extract.ExternalId,
                extract.UserId,
                entryId
            });
        }

        public async Task<int> CreateReceiptExtractAsync (ExtractEntity extract, int entryId)
        {
            using var conn = MainRepository.CreateConnection();
            const string sql = @"
                        INSERT INTO 
                            renda_pgto 
                            (historico, vlr, data, renda_id, user_id, lcto_id)
                        VALUES 
                            (@Name, @Value, @ExtractDate, @ExternalId, @UserId, @entryId)
                        RETURNING id_renda;";

            return await conn.ExecuteAsync(sql, new
            {
                extract.Name,
                extract.Value,
                extract.ExtractDate,
                extract.ExternalId,
                extract.UserId,
                entryId
            });
        }
        #endregion

        #region Update
        public async Task UpdateMainExtractAsync(ExtractEntity extract)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                UPDATE 
                    extrato 
                SET 
                    data = @ExtractDate, historico = @Name, vlr = @Value
                WHERE 1=1
                    AND id_lcto = @Id 
                    AND user_id = @UserId 
                    AND ativo = TRUE;";

            await conn.ExecuteAsync(sql, new
            {
                extract.ExtractDate,
                extract.Name,
                extract.Value,
                extract.Id,
                extract.UserId
            });
        }

        public async Task UpdateExpenseExtractAsync(ExtractEntity extract)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        pagamentos 
                    SET 
                        historico = @Name, vlr = @Value, data = @ExtractDate 
                    WHERE 1=1
                        AND lcto_id = @Id
                        AND user_id = @UserId;";

            await conn.ExecuteAsync(sql, new
            {
                extract.ExtractDate,
                extract.Name,
                extract.Value,
                extract.Id,
                extract.UserId
            });
        }

        public async Task UpdateDebtExtractAsync(ExtractEntity extract)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        divida_pgto 
                    SET 
                        historico = @Name, vlr = @Value, data = @ExtractDate 
                    WHERE 1=1 
                        AND lcto_id = @Id 
                        AND user_id = @UserId;";

            await conn.ExecuteAsync(sql, new
            {
                extract.ExtractDate,
                extract.Name,
                extract.Value,
                extract.Id,
                extract.UserId
            });
        }

        public async Task UpdateGoalExtractAsync(ExtractEntity extract)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        meta_pgto 
                    SET 
                        historico = @Name, vlr = @Value, data = @ExtractDate 
                    WHERE 1=1 
                        AND lcto_id = @Id 
                        AND user_id = @UserId;";

            await conn.ExecuteAsync(sql, new
            {
                extract.ExtractDate,
                extract.Name,
                extract.Value,
                extract.Id,
                extract.UserId
            });
        }

        public async Task UpdateInvestmentExtractAsync(ExtractEntity extract)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        investimento_pgto 
                    SET 
                        historico = @Name, vlr = @Value, data = @ExtractDate 
                    WHERE 1=1
                        AND lcto_id = @Id 
                        AND user_id = @UserId;";

            await conn.ExecuteAsync(sql, new
            {
                extract.ExtractDate,
                extract.Name,
                extract.Value,
                extract.Id,
                extract.UserId
            });
        }

        public async Task UpdateReceiptExtractAsync(ExtractEntity extract)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        renda_pgto 
                    SET 
                        historico = @Name, vlr = @Value, data = @ExtractDate 
                    WHERE 1=1 
                        AND lcto_id = @Id 
                        AND user_id = @UserId;";

            await conn.ExecuteAsync(sql, new
            {
                extract.ExtractDate,
                extract.Name,
                extract.Value,
                extract.Id,
                extract.UserId
            });
        }
        #endregion
        
        #region Delete
        public async Task DeleteMainExtractAsync(int id, int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        extrato 
                    SET ativo = FALSE 
                    WHERE 1=1 
                        AND id_lcto = @id 
                        AND user_id = @userId;";

            await conn.ExecuteAsync(sql, new
            {
                id, userId
            });
        }

        public async Task DeleteExpenseExtractAsync(int id, int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        pagamentos 
                    SET ativo = FALSE 
                    WHERE 1=1 
                        AND lcto_id = @id 
                        AND user_id = @userId;";

            await conn.ExecuteAsync(sql, new
            {
                id,
                userId
            });
        }

        public async Task DeleteDebtExtractAsync(int id, int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        divida_pgto 
                    SET ativo = FALSE 
                    WHERE 1=1 
                        AND lcto_id = @id 
                        AND user_id = @userId;";

            await conn.ExecuteAsync(sql, new
            {
                id,
                userId
            });
        }

        public async Task DeleteGoalExtractAsync(int id, int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        meta_pgto 
                    SET ativo = FALSE 
                    WHERE 1=1 
                        AND lcto_id = @id 
                        AND user_id = @userId;";

            await conn.ExecuteAsync(sql, new
            {
                id,
                userId
            });
        }

        public async Task DeleteInvestmentExtractAsync(int id, int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        investimento_pgto 
                    SET ativo = FALSE 
                    WHERE 1=1 
                        AND lcto_id = @id 
                        AND user_id = @userId;";

            await conn.ExecuteAsync(sql, new
            {
                id,
                userId
            });
        }

        public async Task DeleteReceiptExtractAsync(int id, int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE 
                        renda_pgto 
                    SET ativo = FALSE 
                    WHERE 1=1 
                        AND lcto_id = @id 
                        AND user_id = @userId;";

            await conn.ExecuteAsync(sql, new
            {
                id,
                userId
            });
        }

        #endregion

        #region Aditional
        public async Task UpdateMultipleBalanceAsync(IEnumerable<ExtractBalanceEntity> balances)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    UPDATE extrato AS e
                    SET saldo = NowDate.novo_saldo
                    FROM unnest(@ids, @balance) AS NowDate(id_lcto, novo_saldo)
                    WHERE e.id_lcto = NowDate.id_lcto;";

            var ids = balances.Select(x => x.Id).ToArray();
            var balance = balances.Select(x => x.Balance).ToArray();

            await conn.ExecuteAsync(sql, new { ids, balance });
        }
        #endregion
    }
}
