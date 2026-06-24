using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class SpecificRegistersReader : ISpecificRegistersReader
    {
        public async Task<IEnumerable<ExtractDTO>> ReadExtractByUser(int id, DateTime initialDate, DateTime endDate)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    e.id_lcto AS Id,
                    e.historico AS Name,
                    e.vlr AS Value,
                    e.data AS ExtractDate,
                    CASE 
                        WHEN p.lcto_id IS NOT NULL THEN 'gasto'
                        WHEN dp.lcto_id IS NOT NULL THEN 'divida'
                        WHEN mp.lcto_id IS NOT NULL THEN 'meta'
                        WHEN ip.lcto_id IS NOT NULL THEN 'investimento'
                        WHEN rp.lcto_id IS NOT NULL THEN 'renda'
                        ELSE 'desconhecido'
                    END AS Kind,
                    e.saldo AS Balance
                FROM 
                    extrato e
                LEFT JOIN 
                    pagamentos p ON p.lcto_id = e.id_lcto AND p.ativo = TRUE
                LEFT JOIN 
                    divida_pgto dp ON dp.lcto_id = e.id_lcto AND dp.ativo = TRUE
                LEFT JOIN 
                    meta_pgto mp ON mp.lcto_id = e.id_lcto AND mp.ativo = TRUE
                LEFT JOIN 
                    investimento_pgto ip ON ip.lcto_id = e.id_lcto AND ip.ativo = TRUE
                LEFT JOIN 
                    renda_pgto rp ON rp.lcto_id = e.id_lcto AND rp.ativo = TRUE
                WHERE 1=1
                    AND e.ativo = TRUE
                    AND e.data BETWEEN @initialDate AND @endDate
                    AND e.user_id = @userId;";

            var extract = await conn.QueryFirstOrDefaultAsync<ExtractBaseModel>(
                sql,
            new { userId = id,
                initialDate = initialDate,
                endDate = endDate});

            if (extract is null)
                return null;

            return (IEnumerable<ExtractDTO>)ExtractMapper.ToInput(extract);
        }
    }
}
