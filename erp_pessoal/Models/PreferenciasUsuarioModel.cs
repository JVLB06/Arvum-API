namespace erp_pessoal.Models
{
    public class PreferenciasUsuarioModel
    {
        public string IdUsuario { get; set; }
        public string IdPreferencia { get; set; }
        public string IdGasto { get; set; }
        public bool Excluir { get; set; }
        public bool Reduzir { get; set; }
        public bool Bloqueado { get; set; }
    }
}
