namespace erp_pessoal.Models
{
    public class SugestoesModel
    {
        public string GastoId { get; set; }
        public string GastoNome { get; set; }
        public float VlrMin { get; set; }
        public float VlrMax { get; set; }
        public int Prioridade { get; set; }
    }
}
