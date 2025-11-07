namespace erp_pessoal.Models
{
    public class IndicadoresModel
    {
        public float Endividamento { get; set; }
        public RelacaoGastosModel Gastos { get; set; }
        public SaudeRendaModel IndiceSaude { get; set; }
        public string AvaliacaoGastoMensal { get; set; }
        public IEnumerable<PrevisaoModel> PrevisaoDivida { get; set; }
        public IEnumerable<PrevisaoModel> PrevisaoMeta { get; set; }
    }
}
