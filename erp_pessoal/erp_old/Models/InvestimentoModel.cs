namespace erp_pessoal.Models
{
    public class InvestimentoModel
    {
        public string descricao { get; set; }
        public decimal vlr { get; set; }
        public DateTime data_init { get; set; }
        public decimal juro { get; set; }
    }
}
