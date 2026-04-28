namespace erp_pessoal.Models
{
    public class RendaUpdateModel
    {
        public int id_renda {  get; set; }
        public string descricao { get; set; }
        public decimal vlr_min { get; set; }
        public decimal vlr_max { get; set; }
        public DateTime data { get; set; }
    }
}