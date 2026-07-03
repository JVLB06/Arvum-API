using Domain.Helpers;

namespace Domain.Entities
{
    public class SugestionsReponseEntity
    {
        public int UserId { get; private set; }
        public string FinancialHealth { get; private set; }
        public string DebtSituation { get; private set; }
        public string FixedExpensesSituation { get; private set; }
        public string VariableExpensesSituation { get; private set; }
        public IEnumerable<SugestionsEntity> ExclusionSugestions { get; private set; }
        public IEnumerable<SugestionsEntity> ReductionSugestions { get; private set; }

        public SugestionsReponseEntity(int userId, decimal totalDebts, decimal totalReceipts, decimal fixedExpenses, decimal variableExpenses, IEnumerable<SugestionsEntity> exclusions, IEnumerable<SugestionsEntity> reductions)
        {
            float health = ThinkingHelper.CalculateGenericIndicator(((fixedExpenses + variableExpenses) / 2),totalReceipts);

            UserId = userId;
            FinancialHealth = ThinkingHelper.GenerateFinancialHealthSugestions(health);
            DebtSituation = ThinkingHelper.ValidateDebtSituation(totalDebts);
            FixedExpensesSituation = ThinkingHelper.ValidateFixedExpensesSituation(fixedExpenses);
            VariableExpensesSituation = ThinkingHelper.ValidateVariableExpensesSituation(variableExpenses);
            ExclusionSugestions = ThinkingHelper.ProcessSugestions(exclusions, health);
            ReductionSugestions = ThinkingHelper.ProcessSugestions(reductions, health);
        }
    }
}
