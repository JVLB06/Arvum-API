using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class GeneralInvestmentsReader : IGeneralInvestmentsReader
    {
        public async Task<IEnumerable<InvestmentDTO>> ReadActivesInvestmentsAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    SELECT 
                        id_invest AS Id,
                        nome AS Name,
                        vlr AS Value,
                        juro AS Interest,
                        data_init AS InitialDate,
                        data_resgate AS ReceiveDate,
                        resgate AS ReceivedValue
                    FROM investimentos 
                    WHERE 1=1
                        AND user_id = @id
                        AND ativo = TRUE";

            var results = await conn.QueryAsync<InvestmentBaseModel>(sql, new { id });

            if (results is null || !results.Any())
                return Enumerable.Empty<InvestmentDTO>();

            return results.Select(item => InvestmentMapper.ToDTO(item));
        }

        public async Task<IEnumerable<InvestmentDTO>> ReadInactivesInvestmentsAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    SELECT 
                        id_invest AS Id,
                        nome AS Name,
                        vlr AS Value,
                        juro AS Interest,
                        data_init AS InitialDate,
                        data_resgate AS ReceiveDate,
                        resgate AS ReceivedValue
                    FROM investimentos 
                    WHERE 1=1
                        AND user_id = @id
                        AND ativo = FALSE 
                        AND resgate IS NOT NULL";

            var results = await conn.QueryAsync<InvestmentBaseModel>(sql, new { id });

            if (results is null || !results.Any())
                return Enumerable.Empty<InvestmentDTO>();

            return results.Select(item => InvestmentMapper.ToDTO(item));
        }
    }
}