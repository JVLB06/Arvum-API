namespace erp_pessoal.Models.User_finan;

public class Usuario
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime Nasce { get; set; }
    public string Password { get; set; }
}

public class Gasto
{
    public string descricao { get; set; }
    public decimal vlr_min { get; set; }
    public decimal vlr_max { get; set; }
    public DateTime data { get; set; }
    public int prioridade { get; set; }
    public bool fixvar { get; set; }
}
public class Gasto_update
{
    public int id { get; set; }
    public string descricao { get; set; }
    public decimal vlr_min { get; set; }
    public decimal vlr_max { get; set; }
    public DateTime data { get; set; }
    public int prioridade { get; set; }
    public bool fixvar { get; set; }
}

public class Divida
{
    public string descricao { get; set; }
    public decimal vlr { get; set; }
    public DateTime data_venc { get; set; }
    public DateTime data_init { get; set; }
}

public class Divida_update
{
    public int id { get; set; }
    public string descricao { get; set; }
    public decimal vlr { get; set; }
    public DateTime data_venc { get; set; }
    public DateTime data_init { get; set; }
}

public class Metas
{
    public string descricao { get; set; }
    public decimal vlr { get; set; }
    public DateTime data_venc { get; set; }
}
public class Metas_update
{
    public int id { get; set; }
    public string descricao { get; set; }
    public decimal vlr { get; set; }
    public DateTime data_venc { get; set; }
    public decimal progresso { get; set; }
}

public class Investimento
{
    public string descricao { get; set; }
    public decimal vlr { get; set; }
    public DateTime data_init { get; set; }
    public decimal juro { get; set; }
}

public class Investimento_update
{
    public int id { get; set; }
    public string descricao { get; set; }
    public decimal vlr { get; set; }
    public DateTime data_init { get; set; }
    public decimal juro { get; set; }
}

public class Investimento_fim
{
    public int id { get; set; }
    public DateTime data_resgate { get; set; }
    public decimal vlr_resgate { get; set; }
}