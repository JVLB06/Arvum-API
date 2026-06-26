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
        public async Task<IEnumerable<ExtractDTO>> ReadExtractByUser(int userId, DateTime initialDate, DateTime endDate)
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
            new
            {
                userId,
                initialDate,
                endDate
            });

            if (extract is null)
                return null;

            return (IEnumerable<ExtractDTO>)ExtractMapper.ToInput(extract);
        }

        public async Task<IEnumerable<SpecificGoalDTO>> ReadGoalEntryByUser(int userId, DateTime initialDate, DateTime endDate)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    e.id_lcto AS Id,
                    mp.id_pgto_meta AS SpecificId, 
                    e.data AS ExtractDate, 
                    e.historico AS Description, 
                    mp.vlr AS EntryValue, 
                    m.id_meta AS GoalId,
                    m.nome AS GoalName, 
                    m.vlr AS FullGoalValue, 
                    m.data_meta AS GoalDate, 
                    m.progresso AS Progress, 
                    e.saldo AS Balance
                FROM 
                    meta_pgto mp
                INNER JOIN 
                    extrato e ON e.id_lcto = mp.lcto_id
                INNER JOIN 
                    meta m ON m.id_meta = mp.meta_invest_id
                INNER JOIN 
                    usuarios u ON u.id = @userId
                WHERE 1=1 
                    AND mp.ativo = TRUE
                    AND e.ativo = TRUE
                    AND e.data BETWEEN @initialDate AND @endDate
                ORDER BY e.data desc";

            var extract = await conn.QueryFirstOrDefaultAsync<SpecificGoalBaseModel>(sql, new
            {
                userId,
                initialDate,
                endDate
            });

            if (extract is null)
                return null;

            return (IEnumerable<SpecificGoalDTO>)SpecificGoalMapper.ToDTO(extract);
        }

        public async Task<IEnumerable<SpecificExpensesDTO>> ReadExpenseEntryByUser(int userId, DateTime initialDate, DateTime endDate)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    e.id_lcto AS Id,
                    p.id_gasto_geral AS SpecificId, 
                    e.data AS ExtractDate, 
                    e.historico AS Description, 
                    p.vlr AS EntryValue, 
                    g.id_gasto AS ExpenseId,
                    g.nome AS ExpenseName, 
                    (g.vlr_min + g.vlr_max)/2 AS ExpenseValue, 
                    g.data_venc AS ExpenseDate, 
                    g.fixvar AS Variable, 
                    e.saldo AS Balance
                FROM 
                    pagamentos p
                INNER JOIN 
                    extrato e ON e.id_lcto = p.lcto_id
                INNER JOIN 
                    gastos g ON g.id_gasto = p.gasto_id 
                INNER JOIN 
                    usuarios u ON u.id = @userId
                WHERE 1=1 
                    AND p.ativo = TRUE
                    AND e.data BETWEEN @initialDate AND @endDate
                    AND e.ativo = TRUE
                ORDER BY e.data desc;";

            var extract = await conn.QueryFirstOrDefaultAsync<SpecificExpensesBaseModel>(sql, new
            {
                userId,
                initialDate,
                endDate
            });

            if (extract is null)
                return null;

            return (IEnumerable<SpecificExpensesDTO>)SpecificExpensesMapper.ToDTO(extract);
        }

        public async Task<IEnumerable<SpecificDebtDTO>> ReadDebtsEntryByUser(int userId, DateTime initialDate, DateTime endDate)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                    SELECT 
                        e.id_lcto AS Id
                        dp.id_pgto_divida AS SpecificId, 
                        e.data AS ExtractDate, 
                        e.historico AS Description, 
                        dp.vlr AS EntryValue, 
                        d.id_invest AS DebtId,
                        d.nome AS DebtName, 
                        d.vlr as DebtValue, 
                        d.data AS DebtDate, 
                        d.data_prev AS DebtEndDate, 
                        e.saldo AS Balance
                    FROM 
                        divida_pgto dp
                    INNER JOIN 
                        extrato e ON e.id_lcto = dp.lcto_id
                    INNER JOIN 
                        divida d ON d.id_invest = dp.divida_id
                    INNER JOIN 
                        usuarios u ON u.id = @userId 
                    WHERE 1=1 
                        AND dp.ativo = TRUE 
                        AND e.data BETWEEN @initialDate AND @endDate
                        AND e.ativo = TRUE 
                    ORDER BY e.data desc;";

            var extract = await conn.QueryFirstOrDefaultAsync<SpecificDebtBaseModel>(sql, new
            {
                userId,
                initialDate,
                endDate
            });

            if (extract is null)
                return null;

            return (IEnumerable<SpecificDebtDTO>)SpecificDebtMapper.ToDTO(extract);
        }

        public async Task<IEnumerable<SpecificReceiptDTO>> ReadReceiptsEntryByUser(int userId, DateTime initialDate, DateTime endDate)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    e.id_lcto AS Id
                    rp.id_renda AS SpecificId, 
                    e.data AS ExtractDate, 
                    e.historico AS Description, 
                    rp.vlr AS Value, 
                    r.id_renda AS ReceiptId,
                    r.nome, (r.vlr_min+ r.vlr_max)/2 AS ReceiptValue, 
                    r.data_pag AS ReceiptDate, 
                    e.saldo AS Balance
                FROM 
                    renda_pgto rp
                INNER JOIN 
                    rendas r ON r.id_renda = rp.renda_id
                INNER JOIN 
                    extrato e ON e.id_lcto = rp.lcto_id
                INNER JOIN 
                    usuarios u ON u.id = @user_id
                WHERE 1=1
                    AND rp.ativo = TRUE
                    AND e.data BETWEEN @initialDate AND @endDate
                    AND e.ativo = TRUE
                ORDER BY e.data desc;";

            var extract = await conn.QueryFirstOrDefaultAsync<SpecificReceiptBaseModel>(sql, new
            {
                userId,
                initialDate,
                endDate
            });

            if (extract is null)
                return null;

            return (IEnumerable<SpecificReceiptDTO>)SpecificReceiptMapper.ToDTO(extract);
        }

        public async Task<IEnumerable<SpecificInvestmentDTO>> ReadInvestmentsEntryByUser(int userId, DateTime initialDate, DateTime endDate)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    e.id_lcto AS Id
                    ip.id_invest AS SpecificId, 
                    e.data AS ExtractDate, 
                    e.historico AS Description, 
                    ip.vlr AS Value, 
                    i.id_invest as InvestId,
                    i.nome AS InvestName, 
                    i.vlr AS InvestValue, 
                    i.juro AS Interest, 
                    i.data_init AS InvestDate, 
                    e.saldo AS Balance
                FROM 
                    investimento_pgto ip
                INNER JOIN 
                    extrato e ON e.id_lcto = ip.lcto_id
                INNER JOIN 
                    investimentos i ON i.id_invest = ip.invest_id
                INNER JOIN 
                    usuarios u ON u.id = @userId
                WHERE 1=1
                    AND ip.ativo = TRUE
                    AND e.ativo = TRUE
                ORDER BY e.data desc;";

            var extract = await conn.QueryFirstOrDefaultAsync<SpecificInvestmentBaseModel>(sql, new
            {
                userId,
                initialDate,
                endDate
            });

            if (extract is null)
                return null;

            return (IEnumerable<SpecificInvestmentDTO>)SpecificInvestmentMapper.ToDTO(extract);
        }
    }
}
