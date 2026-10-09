using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;

namespace Application.Services
{
    public class ThinkingService : IThinkingService
    {
        private readonly IThinkingReader _reader;
        private readonly IThinkingWriter _writer;

        public ThinkingService(IThinkingReader reader, IThinkingWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public async Task<IEnumerable<PreferenceEntity>> GetPreferences(int userId)
        {
            var connect = await _reader.ReadPreferencesAsync(userId);

            return connect.Select(preference => new PreferenceEntity(
                preference.UserId,
                preference.Id,
                preference.ExternalId,
                preference.Exclude,
                preference.Reduce,
                preference.Block,
                preference.Name));
        }

        public async Task CreatePreference(PreferenceDTO preference, int userId)
        {
            var connect = await _reader.ReadPreferenceAsync(userId, (int)(preference.Id is null ? 0 : preference.Id));

            PreferenceEntity input = new PreferenceEntity(
                    preference.UserId,
                    preference.Id,
                    preference.ExternalId,
                    preference.Exclude,
                    preference.Reduce,
                    preference.Block,
                    null);

            if (connect is null)
            {
                await _writer.SetPreferenceAsync(input);
            }
            else
            {
                await _writer.PutPreferenceAsync(input);
            }
        }

        public async Task DeletePreference(int id, int userId)
        {
            await _writer.DeletePreferenceAsync(id, userId);
        }

        /// <summary>
        /// Retorna os indicadores de saúde financeira, pensamentos personalizados, sugestões de redução/corte
        /// e comparativo entre gastos e renda — estruturado exatamente como o frontend espera
        /// (pensamentos, reducoes[{gastoId,nome,valorAtual,valorSugerido}], exclusoes, comparativo).
        /// </summary>
        public async Task<object> GeneratePreferencesAsync(int userId)
        {
            var debts = await _reader.ReadDebtTotalAsync(userId);
            var receipts = await _reader.ReadReceiptTotalAsync(userId);
            var expenses = await _reader.ReadExpensesTotalAsync(userId);
            var exclusions = await _reader.ReadExclusionsAsync(userId);
            var reductions = await _reader.ReadReductionsAsync(userId);

            var totalDebts = debts.FirstOrDefault()?.Total ?? 0;
            var totalReceipts = receipts.FirstOrDefault()?.Total ?? 0;
            var fixedExpenses = expenses.FirstOrDefault(x => x.Kind == "Fix")?.Total ?? 0;
            var variableExpenses = expenses.FirstOrDefault(x => x.Kind == "Var")?.Total ?? 0;
            var totalExpenses = fixedExpenses + variableExpenses;
            var balance = totalReceipts - totalExpenses;
            double expenseRatio = totalReceipts > 0 ? (double)((totalExpenses / totalReceipts) * 100) : 0.0;

            var health = ThinkingHelper.CalculateGenericIndicator(totalExpenses, totalReceipts);
            var mensagensPensamento = GerarPensamentos(health, totalDebts, totalExpenses, totalReceipts, balance, expenseRatio);

            var reducoes = reductions.Select(dto => new
            {
                gastoId = dto.Id,
                nome = dto.Name,
                valorAtual = Math.Round((decimal)((dto.MinValue + dto.MaxValue) / 2), 2),
                valorSugerido = Math.Round((decimal)((dto.MinValue + dto.MaxValue) / 2 * 0.85m), 2)
            });

            var exclusoes = exclusions.Select(dto => new
            {
                gastoId = dto.Id,
                nome = dto.Name,
                valorAtual = Math.Round((decimal)((dto.MinValue + dto.MaxValue) / 2), 2),
                valorSugerido = 0m
            });

            return new
            {
                pensamentos = mensagensPensamento,
                reducoes = reducoes,
                exclusoes = exclusoes,
                comparativo = new
                {
                    renda = totalReceipts,
                    gastos = totalExpenses,
                    gastosFixos = fixedExpenses,
                    gastosVariaveis = variableExpenses,
                    saldoPositivo = Math.Round(balance, 2),
                    razaoGastosRenda = Math.Round(expenseRatio, 1),
                    mensagem = balance < 0
                        ? "Atenção: seus gastos superaram sua renda. Revise suas despesas variáveis."
                        : expenseRatio >= 70
                        ? "Seus gastos consomem mais de 70% da sua renda. Revise as sugestões de redução."
                        : "Relação gasto/renda dentro do esperado. Continue monitorando."
                }
            };
        }

        private static IEnumerable<string> GerarPensamentos(float health, decimal totalDebts, decimal totalExpenses, decimal totalReceipts, decimal balance, double expenseRatio)
        {
            var basePensamentos = new[]
            {
                "Pequenos ajustes diários têm um impacto enorme no longo prazo.",
                "Automatize suas economias: pague-se primeiro, antes de gastar.",
                "Separe o que é necessidade do que é desejo antes de cada compra.",
                "Revisar suas despesas mensalmente é o maior segredo do controle financeiro.",
                "O valor que você não gasta hoje é o seu futuro mais seguro.",
                "Tenha uma reserva de emergência antes de buscar investimentos de risco.",
                "Metas financeiras escritas tornam as decisões do dia a dia muito mais fáceis.",
                "Organizar as finanças é o alicerce para qualquer conquista grande."
            };

            var insightsEspecificos = new List<string>();
            if (balance < 0)
                insightsEspecificos.Add($"Cuidado: neste período você gastou R$ {totalExpenses.ToString("F2")} e recebeu R$ {totalReceipts.ToString("F2")}. Cada real cortado de gastos variáveis volta para o seu saldo.");
            else if (expenseRatio >= 90)
                insightsEspecificos.Add("Quase todo o seu dinheiro está saindo. Identifique a maior categoria de gasto e estabeleça um teto para ela.");
            else if (expenseRatio >= 70)
                insightsEspecificos.Add("Seus gastos consomem boa parte da sua renda. Pequenos ajustes aqui equilibram todo o seu planejamento.");
            else if (expenseRatio >= 55)
                insightsEspecificos.Add("Sua relação gasto/renda está razoável. Mantenha o controle para não escalar o consumo conforme a renda cresce.");
            else
                insightsEspecificos.Add("Excelente: seus gastos estão sob controle em relação à sua renda. Aproveite para fortalecer sua reserva de emergência.");

            if (totalDebts > 0 && health >= 100)
                insightsEspecificos.Add($"Você tem R$ {totalDebts.ToString("F2")} de dívidas registradas. Antes de investir mais, defina uma meta clara para quitar as de juros mais altos.");
            else if (totalDebts == 0 && health < 70)
                insightsEspecificos.Add("Sem dívidas registradas, use essa vantagem para construir patrimônio, não para aumentar o consumo.");

            var todas = basePensamentos
                .Concat(insightsEspecificos)
                .Distinct()
                .ToList();

            // Seleciona 5 a 8 pensamentos com variação determinística por dia (evita flutuar a cada refresh)
            var seed = DateTime.Today.DayOfYear;
            var random = new Random(seed);
            todas = todas.OrderBy(x => random.Next()).ToList();

            return todas.Skip(0).Take(Math.Min(8, Math.Max(5, todas.Count))).ToList();
        }
    }
}
