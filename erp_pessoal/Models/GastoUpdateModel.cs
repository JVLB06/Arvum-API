namespace erp_pessoal.Models
{
    public class GastoUpdateModel
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public decimal vlr_min { get; set; }
        public decimal vlr_max { get; set; }
        public DateTime data { get; set; }
        public int prioridade { get; set; }
        public bool fixvar { get; set; }
    }
}
