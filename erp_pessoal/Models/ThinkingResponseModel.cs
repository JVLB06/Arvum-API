namespace erp_pessoal.Models
{
    public class ThinkingResponseModel
    {
        public List<string> Pensamentos { get; set; } = new();

        public List<ReducaoModel> Reducoes { get; set; } = new();

        public List<ExclusaoModel> Exclusoes { get; set; } = new();

        public IndicadoresModel Indicadores { get; set; }
    }
}