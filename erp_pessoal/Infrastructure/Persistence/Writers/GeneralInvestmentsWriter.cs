using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Writers
{
    public class GeneralInvestmentsWriter : IGeneralInvestmentsWriter
    {
        public async Task CreateInvestmentAsync(InvestmentEntity investment)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"INSERT INTO 
                                investimentos (user_id, nome, vlr, data_init, juro, ativo) 
                                VALUES (@UserId, @Description, @Value, @InitialDate, @Interest, TRUE)";

            await conn.ExecuteAsync(sql, new
            {
                investment.UserId,
                investment.Description,
                investment.Value,
                investment.InitialDate,
                investment.Interest
            });
        }

        public async Task UpdateInvestmentAsync(InvestmentEntity investment)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE 
                                    investimentos 
                                SET 
                                    nome = @Description, 
                                    vlr = @Value, 
                                    data_init = @InitialDate, 
                                    juro = @Interest 
                                WHERE 
                                    id_invest = @Id 
                                    AND user_id = @UserId";

            await conn.ExecuteAsync(sql, new
            {
                investment.Id,
                investment.UserId,
                investment.Description,
                investment.Value,
                investment.InitialDate,
                investment.Interest
            });
        }

        public async Task DeleteInvestmentAsync(DeleteInvestmentEntity investment)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE 
                                    investimentos 
                                SET 
                                    ativo = FALSE 
                                    resgate = NULL
                                WHERE 1=1 
                                    AND id_invest = @Id 
                                    AND user_id = @UserId";

            await conn.ExecuteAsync(sql, new { investment.Id, investment.UserId });
        }

        public async Task FinishInvestmentAsync(FinishInvestmentEntity investment)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"UPDATE 
                                    investimentos 
                                SET 
                                    ativo = FALSE, 
                                    data_resgate = @ReceiveDate, 
                                    resgate = @ReceivedValue 
                                WHERE 1=1 
                                    AND id_invest = @Id 
                                    AND user_id = @UserId";

            await conn.ExecuteAsync(sql, new
            {
                investment.Id,
                investment.UserId,
                investment.ReceiveDate,
                investment.ReceivedValue
            });
        }
    }
}
