namespace Domain.Helpers
{
    public class ExtractHelper
    {
        public static decimal NormalizeValueByKind(decimal value, string kind)
        {
            value = Math.Abs(value);

            return kind switch
            {
                "gasto" => -value,
                "divida" => -value,
                "meta" => -value,
                "investimento" => -value,
                "renda" => value,
                _ => value
            };
        }
    }
}
