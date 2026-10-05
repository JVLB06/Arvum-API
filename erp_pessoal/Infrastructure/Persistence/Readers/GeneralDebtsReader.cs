using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class GeneralDebtsReader : IGeneralDebtsReader
    {
        public async Task<IEnumerable<DebtDTO>> ReadDebtsAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"SELECT 
                                    id_invest AS Id, 
                                    nome AS Name,       
                                    vlr AS Value,
                                    data AS InitialDate,
                                    data_prev AS ReceiveDate,
                                    quitada AS Paid    
                                FROM divida 
                                WHERE 1=1
                                    AND user_id = @Id 
                                    AND ativo = TRUE";

            var results = await conn.QueryAsync<DebtBaseModel>(
                sql, new { Id = id });

            if (results is null || !results.Any())
                return Enumerable.Empty<DebtDTO>();

            return results.Select(item => DebtMapper.ToDTO(item));
        }

        public async Task<IEnumerable<DebtDTO>> ReadInactiveDebtsAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"SELECT 
                                    id_invest AS Id, 
                                    nome AS Name,       
                                    vlr AS Value,
                                    data AS InitialDate,
                                    data_prev AS ReceiveDate,
                                    quitada AS Paid    
                                FROM divida 
                                WHERE 1=1
                                    AND user_id = @Id 
                                    AND ativo = FALSE
                                    AND quitada = TRUE";

            var results = await conn.QueryAsync<DebtBaseModel>(
                sql, new { Id = id });

            if (results is null || !results.Any())
                return Enumerable.Empty<DebtDTO>();

            return results.Select(item => DebtMapper.ToDTO(item));
        }

        public async Task<IEnumerable<DebtDTO>> ReadDebtsProgressAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"SELECT 
                                    d.id_invest AS Id, 
                                    d.nome AS Name,       
                                    d.vlr AS Value,
                                    d.data AS InitialDate,
                                    d.data_prev AS ReceiveDate,
                                    SUM(dp.vlr) AS DebtPaid    
                                FROM divida d
                                INNER JOIN divida_pgto dp ON d.id_invest = dp.divida_id
                                WHERE 1=1
                                    AND user_id = @Id 
                                    AND ativo = TRUE
                                GROUP BY d.id_invest";

            var results = await conn.QueryAsync<DebtBaseModel>(
                sql, new { Id = id });

            if (results is null || !results.Any())
                return Enumerable.Empty<DebtDTO>();

            return results.Select(item => DebtMapper.ToDTO(item));
        }
    }
}