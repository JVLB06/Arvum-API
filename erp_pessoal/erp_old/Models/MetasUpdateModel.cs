namespace erp_pessoal.Models
{
    public class MetasUpdateModel
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public decimal vlr { get; set; }
        public DateTime data_venc { get; set; }
        public decimal progresso { get; set; }
    }
}
