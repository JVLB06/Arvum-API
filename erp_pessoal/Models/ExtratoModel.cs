namespace erp_pessoal.Models
{
    public class ExtratoModel
    {
        public int ?extrato_id { get; set; }
        public string historico { get; set; }
        public decimal valor { get; set; }
        public string tipo { get; set; }
        public DateTime data { get; set; }
        public int id_ref { get; set; }
        public decimal ?saldo { get; set; }
    }
}
