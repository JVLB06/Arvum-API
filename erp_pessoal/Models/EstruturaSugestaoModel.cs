namespace erp_pessoal.Models
{
    public class EstruturaSugestaoModel
    {
        public string Endividamento { get; set; }
        public string GastosFixos { get; set; }
        public string GastosVariaveis { get; set; }
        public string IndiceSaude { get; set; }
        public string GastoMensal { get; set; }
        public IEnumerable<string> PrevisaoDivida { get; set; }
        public IEnumerable<string> PrevisaoMeta { get; set; }
        public string Reducao { get; set; }
    }
}
