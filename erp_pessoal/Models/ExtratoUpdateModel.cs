namespace erp_pessoal.Models
{
    public class ExtratoUpdateModel
    {
        public int id { get; set; }
        public string historico { get; set; }
        public decimal valor { get; set; }
        public string tipo { get; set; }
        public DateTime data { get; set; }
        public int id_ref { get; set; }
    }
}
