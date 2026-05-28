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

            var results = await conn.QueryFirstOrDefaultAsync<DebtBaseModel>(
                sql, new { Id = id });

            if (results is null)
                return null;

            return (IEnumerable<DebtDTO>)DebtMapper.ToDTO(results);
        }
    }
}
