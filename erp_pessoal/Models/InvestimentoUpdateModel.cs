namespace erp_pessoal.Models
{
    public class InvestimentoUpdateModel
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public decimal vlr { get; set; }
        public DateTime data_init { get; set; }
        public decimal juro { get; set; }
    }
}
