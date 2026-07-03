using Domain.Entities;

namespace Domain.Helpers
{
    public class ThinkingHelper
    {
        public static float CalculateGenericIndicator(decimal debt, decimal receipt)
        {
            if (receipt == 0)
            {
                return 0;
            }

            return (float)((debt / receipt) * 100);
        }
        public static string GenerateFinancialHealthSugestions(float healthPercentage)
        {
            if (healthPercentage >= 100)
                return "Sua saúde financeira está comprometida. Confira urgentemente nossas sugestões.";
            else if (healthPercentage >= 70)
                return "Você está próximo do limite saudável de gastos. Verifique nossas sugestões de redução e corte de gastos.";
            else if (healthPercentage >= 55)
                return "Sua saúde financeira está bem controlada. Parabéns!";
            else
                return "Não foi possível obter informações sobre sua saúde financeira";
        }

        public static string ValidateDebtSituation(decimal debt)
        {
            if (debt >= 50)
                return "Seu índice de endividamento está alto.";
            else
                return "Seu índice de endividamento está controlado.";
        }

        public static string ValidateFixedExpensesSituation(decimal expense)
        {
            if (expense >= 70)
                return "Seus gastos fixos estão muito elevados.";
            else
                return "Seus gastos estão controlados.";
        }

        public static string ValidateVariableExpensesSituation(decimal expense)
        {
            if (expense >= 30)
                return "Seus gastos variáveis estão muito elevados.";
            else
                return "Seus gastos estão controlados.";
        }

        public static IEnumerable<SugestionsEntity> ProcessSugestions(IEnumerable<SugestionsEntity> raw, float healthIndicator)
        {
            int maxItems;
            if (healthIndicator >= 100) maxItems = 15;     
            else if (healthIndicator >= 70) maxItems = 10; 
            else if (healthIndicator >= 55) maxItems = 5;  
            else maxItems = 2;                             

            return (raw
                .OrderBy(x => Guid.NewGuid())
                .Take(maxItems)
                .ToList());
        }
    }
}
