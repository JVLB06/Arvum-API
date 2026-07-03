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

            var results = await conn.QueryFirstOrDefaultAsync<ReceiptBaseModel>(
                sql,
            new { user_id = id });

            if (results is null)
                return null;

            return (IEnumerable<ReceiptDTO>)ReceiptMapper.ToInput(results);
        }
    }
}
