using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class GeneralExpensesReader : IGeneralExpensesReader
    {
        public async Task<IEnumerable<ExpenseDTO>> ReadExpensesAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                            SELECT 
                                id_gasto AS Id,
                                nome AS Description,        
                                vlr_min AS MinValue,
                                vlr_max AS MaxValue,
                                prioridade AS Priority,
                                data_venc AS DueDate,
                                fixvar AS IsFixed
                            FROM 
                                gastos 
                            WHERE 1=1 
                                AND user_id = @userId
                                AND ativo = TRUE;";

            var results = await conn.QueryAsync<ExpenseBaseModel>(
                sql,
                new { userId });

            if (results is null || !results.Any())
                return Enumerable.Empty<ExpenseDTO>();

            return results.Select(item => ExpenseMapper.ToDTO(item));
        }

    public async Task<IEnumerable<ProcessedExpenseDTO>> ReadExpensesComparativeAsync(int userId)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                            SELECT 
                                DATE_TRUNC('month', e.data) AS Month,
                                -- Soma apenas os lançamentos classificados como 'gasto'
                                SUM(e.vlr) FILTER (
                                    WHERE p.lcto_id IS NOT NULL
                                ) AS TotalExpenses,
                                -- Soma apenas os lançamentos classificados como 'renda'
                                SUM(e.vlr) FILTER (
                                    WHERE rp.lcto_id IS NOT NULL
                                ) AS TotalIncomes,
                                SUM(e.vlr) FILTER (
                                    WHERE rp.lcto_id IS NOT NULL
                                ) / 100 * 70 AS ExpensesLimit
                            FROM extrato e
                            LEFT JOIN pagamentos p ON p.lcto_id = e.id_lcto AND p.ativo = TRUE
                            LEFT JOIN divida_pgto dp ON dp.lcto_id = e.id_lcto AND dp.ativo = TRUE
                            LEFT JOIN meta_pgto mp ON mp.lcto_id = e.id_lcto AND mp.ativo = TRUE
                            LEFT JOIN investimento_pgto ip ON ip.lcto_id = e.id_lcto AND ip.ativo = TRUE
                            LEFT JOIN renda_pgto rp ON rp.lcto_id = e.id_lcto AND rp.ativo = TRUE
                            WHERE e.ativo = TRUE
                              AND e.user_id = @userId
                              AND e.data >= DATE_TRUNC('month', CURRENT_DATE) - INTERVAL '12 months'
                            GROUP BY DATE_TRUNC('month', e.data)
                            ORDER BY Month DESC;";

            var results = await conn.QueryAsync<ProcessedExpenseBaseModel>(
                sql,
                new { userId });

            if (results is null || !results.Any())
                return Enumerable.Empty<ProcessedExpenseDTO>();

            return results.Select(item => ProcessedExpenseMapper.ToDTO(item));
        }
    }
}