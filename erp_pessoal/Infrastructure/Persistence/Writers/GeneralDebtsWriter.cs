using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Repositories;
using Npgsql;
using System.Security.Claims;

namespace Infrastructure.Persistence.Writers
{
    public class GeneralDebtsWriter : IGeneralDebtsWriter
    {
        public async Task CreateDebtAsync(DebtEntity debt)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"INSERT INTO 
                                    divida 
                                    (user_id, nome, vlr, data, data_prev, ativo, quitada) 
                                VALUES 
                                    (@UserId, @Name, @Value, @InitialDate, @ReceiveDate, TRUE, FALSE);";

            await conn.ExecuteAsync(sql, new
            {
                debt.UserId,
                debt.Name,
                debt.Value,
                debt.InitialDate,
                debt.ReceiveDate
            });
        }

        public async Task UpdateDebtAsync(DebtEntity debt)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE divida 
                                SET nome = @Name, vlr = @Value, data = @InitialDate, data_prev = @ReceiveDate 
                                WHERE id_invest = @Id AND user_id = @UserId;";

            await conn.ExecuteAsync(sql, new
            {
                debt.Id,
                debt.UserId,
                debt.Name,
                debt.Value,
                debt.InitialDate,
                debt.ReceiveDate
            });
        }
        
        public async Task InactivateDebtAsync(int debtId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE divida 
                                SET ativo = FALSE 
                                WHERE id_invest = @DebtId;";

            await conn.ExecuteAsync(sql, new { DebtId = debtId});
        }
        
        public async Task PayDebtAsync(int debtId)
        {
            using var conn = MainRepository.CreateConnection();
            
            const string sql = @"UPDATE divida 
                                SET ativo = FALSE, quitada = TRUE 
                                WHERE id_invest = @DebtId;";

            await conn.ExecuteAsync(sql, new { DebtId = debtId});
        }
    }
}
