using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class GeneralReceiptsReader : IGeneralReceiptsReader
    {
        public async Task<IEnumerable<ReceiptDTO>> ReadReceiptsAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    id_renda AS Id,
                    nome AS Name,
                    vlr_min AS MinValue,
                    vlr_max AS MaxValue,
                    data_pag AS PaymentDate
                FROM rendas 
                WHERE 1=1
                    AND user_id = @user_id 
                    AND ativo = TRUE";

            var results = await conn.QueryAsync<ReceiptBaseModel>(
                sql,
                new { user_id = id });

            if (results is null)
                return Enumerable.Empty<ReceiptDTO>();

            return results.Select(item => ReceiptMapper.ToInput(item));
        }

        public async Task<IEnumerable<ReceiptDTO>> ReadReceiptsPerMonthAsync(int id)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    MIN(r.id_renda) AS Id,
                    MIN(r.nome) AS Name,
                    SUM(rp.vlr) AS MinValue,
                    SUM(rp.vlr) AS MaxValue,
                    DATE_TRUNC('month', rp.data) AS PaymentDate
                FROM rendas r
                    INNER JOIN renda_pgto rp ON rp.renda_id = r.id_renda
                WHERE r.user_id = @user_id 
                    AND r.ativo = TRUE
                    AND rp.data_pag >= DATE_TRUNC('month', CURRENT_DATE) - INTERVAL '12 months'
                GROUP BY DATE_TRUNC('month', rp.data_pag)
                ORDER BY PaymentDate DESC;";

            var results = await conn.QueryAsync<ReceiptBaseModel>(
                sql,
                new { user_id = id });

            if (results is null)
                return Enumerable.Empty<ReceiptDTO>();

            return results.Select(item => ReceiptMapper.ToInput(item));
        }
    }
}