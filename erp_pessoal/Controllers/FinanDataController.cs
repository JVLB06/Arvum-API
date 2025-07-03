using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using erp_pessoal.Models.User_finan;
namespace erp_pessoal.Controllers {
    [Authorize]
    [ApiController]
    [Route("user_plan")]
    public class FinanDataController : ControllerBase
    {
        //Renda
        [HttpGet("ler_renda")]
        public async Task<IActionResult> GetRenda()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM rendas WHERE user_id = @user_id AND ativo = TRUE", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            await cmdSelect.PrepareAsync();
            var reader = await cmdSelect.ExecuteReaderAsync();
            var rendas = new List<object>();
            while (await reader.ReadAsync())
            {
                rendas.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_renda")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr_min = reader.GetDecimal(reader.GetOrdinal("vlr_min")),
                    vlr_max = reader.GetDecimal(reader.GetOrdinal("vlr_max")),
                    data = reader.GetDateTime(reader.GetOrdinal("data_pag"))
                });
            }
            return Ok(new { rendas });
        }
        [HttpPost("criar_renda")]
        public IActionResult CriarRenda([FromBody] Dictionary<string, string> rendaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inserção de nova renda 
            var cmdInsert = new NpgsqlCommand("INSERT INTO rendas (user_id, nome, vlr_min, vlr_max, data_pag, ativo) VALUES (@user_id, @descricao, @vlr_min, @vlr_max, @data, TRUE)", conn);
            cmdInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdInsert.Parameters.AddWithValue("@descricao", rendaData["descricao"]);
            cmdInsert.Parameters.AddWithValue("@vlr_min", decimal.Parse(rendaData["vlr_min"]));
            cmdInsert.Parameters.AddWithValue("@vlr_max", decimal.Parse(rendaData["vlr_max"]));
            cmdInsert.Parameters.AddWithValue("@data", DateTime.Parse(rendaData["data"]));
            cmdInsert.ExecuteNonQuery();
            return Ok(new { message = "Renda criada com sucesso" });
        }
        [HttpPut("atualizar_renda")]
        public IActionResult AtualizarRenda([FromBody] Dictionary<string, string> rendaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Atualização de renda
            var cmdUpdate = new NpgsqlCommand("UPDATE rendas SET nome = @descricao, vlr_min = @vlr_min, vlr_max = @vlr_max, data_pag = @data WHERE id_renda = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", int.Parse(rendaData["id_renda"]));
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.Parameters.AddWithValue("@descricao", rendaData["descricao"]);
            cmdUpdate.Parameters.AddWithValue("@vlr_min", decimal.Parse(rendaData["vlr_min"]));
            cmdUpdate.Parameters.AddWithValue("@vlr_max", decimal.Parse(rendaData["vlr_max"]));
            cmdUpdate.Parameters.AddWithValue("@data", DateTime.Parse(rendaData["data"]));
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Renda atualizada com sucesso" });
        }
        [HttpDelete("inativar_renda")]
        public IActionResult InativarRenda([FromBody] Dictionary<string, string> rendaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inativação de renda
            var cmdDelete = new NpgsqlCommand("UPDATE rendas SET ativo = FALSE WHERE id_renda = @id AND user_id = @user_id", conn);
            cmdDelete.Parameters.AddWithValue("@id", int.Parse(rendaData["id_renda"]));
            cmdDelete.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdDelete.ExecuteNonQuery();
            return Ok(new { message = "Renda inativada com sucesso" });
        }

        //Investimentos
        [HttpGet("ler_investimentos_ativos")]
        public IActionResult GetInvestimentosAtivos()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM investimentos WHERE user_id = @user_id AND ativo = TRUE", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            var reader = cmdSelect.ExecuteReader();
            var invest = new List<object>();
            while (reader.Read())
            {
                // Se a coluna "data_resgate" for nula, atribui data padrão
                var data_resgate = reader.IsDBNull(reader.GetOrdinal("data_resgate"))
                    ? "00/00/0000"
                    : reader.GetDateTime(reader.GetOrdinal("data_resgate")).ToString("dd/MM/yyyy");
                // Se a coluna "resgate" for nula, atribui 0
                var res = reader.IsDBNull(reader.GetOrdinal("resgate"))
                    ? 0
                    : reader.GetDecimal(reader.GetOrdinal("resgate"));
                invest.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_invest")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr = reader.GetDecimal(reader.GetOrdinal("vlr")),
                    juro = reader.GetDecimal(reader.GetOrdinal("juro")),
                    data_init = reader.GetDateTime(reader.GetOrdinal("data_init")),
                    data_fim = data_resgate,
                    resgate = res
                });
            }
            return Ok(new { invest });
        }
        [HttpGet("ler_investimentos_encerrados")]
        public IActionResult GetInvestimentosEncerrados()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM investimentos WHERE user_id = @user_id AND ativo = FALSE AND resgate IS NOT NULL", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            var reader = cmdSelect.ExecuteReader();
            var invest = new List<object>();
            while (reader.Read())
            {
                // Se a coluna "data_resgate" for nula, atribui data padrão
                var data_resgate = reader.IsDBNull(reader.GetOrdinal("data_resgate"))
                    ? "00/00/0000"
                    : reader.GetDateTime(reader.GetOrdinal("data_resgate")).ToString("dd/MM/yyyy");
                // Se a coluna "resgate" for nula, atribui 0
                var res = reader.IsDBNull(reader.GetOrdinal("resgate"))
                    ? 0
                    : reader.GetDecimal(reader.GetOrdinal("resgate"));
                invest.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_invest")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr = reader.GetDecimal(reader.GetOrdinal("vlr")),
                    juro = reader.GetDecimal(reader.GetOrdinal("juro")),
                    data_init = reader.GetDateTime(reader.GetOrdinal("data_init")),
                    data_fim = data_resgate,
                    resgate = res
                });
            }
            return Ok(new { invest });
        }
        [HttpPost("criar_investimento")]
        public IActionResult CriarInvestimento([FromBody] Investimento investimentoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inserção de novo investimento 
            var cmdInsert = new NpgsqlCommand("INSERT INTO investimentos (user_id, nome, vlr, data_init, juro, ativo) VALUES (@user_id, @descricao, @vlr, @data_init, @juro, TRUE)", conn);
            cmdInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdInsert.Parameters.AddWithValue("@descricao", investimentoData.descricao);
            cmdInsert.Parameters.AddWithValue("@vlr", investimentoData.vlr);
            cmdInsert.Parameters.AddWithValue("@data_init", investimentoData.data_init);
            cmdInsert.Parameters.AddWithValue("@juro", investimentoData.juro);
            cmdInsert.ExecuteNonQuery();
            return Ok(new { message = "Investimento criado com sucesso" });
        }
        [HttpPut("atualizar_investimento")]
        public IActionResult AtualizarInvestimento([FromBody] Investimento_update investimentoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Atualização de investimento
            var cmdUpdate = new NpgsqlCommand("UPDATE investimentos SET nome = @descricao, vlr = @vlr, data_init = @data_init, juro = @juro WHERE id_invest = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", investimentoData.id);
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.Parameters.AddWithValue("@descricao", investimentoData.descricao);
            cmdUpdate.Parameters.AddWithValue("@vlr", investimentoData.vlr);
            cmdUpdate.Parameters.AddWithValue("@data_init", investimentoData.data_init);
            cmdUpdate.Parameters.AddWithValue("@juro", investimentoData.juro);
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Investimento atualizado com sucesso" });
        }
        [HttpDelete("inativar_investimento")]
        public IActionResult InativarInvestimento([FromBody] Dictionary<string, string> investimentoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inativação de investimento
            var cmdDelete = new NpgsqlCommand("UPDATE investimentos SET ativo = FALSE WHERE id_invest = @id AND user_id = @user_id", conn);
            cmdDelete.Parameters.AddWithValue("@id", int.Parse(investimentoData["id_invest"]));
            cmdDelete.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdDelete.ExecuteNonQuery();
            return Ok(new { message = "Investimento inativado com sucesso" });
        }
        [HttpPut("concluir_investimento")]
        public IActionResult ConcluirInvestimento([FromBody] Investimento_fim investimentoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Conclusão de investimento
            var cmdUpdate = new NpgsqlCommand("UPDATE investimentos SET ativo = FALSE, data_resgate = @data_resgate, resgate = @vlr_fim WHERE id_invest = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", investimentoData.id);
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.Parameters.AddWithValue("@data_resgate", investimentoData.data_resgate);
            cmdUpdate.Parameters.AddWithValue("@vlr_fim", investimentoData.vlr_resgate);
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Investimento concluído com sucesso" });
        }

        //Dividas
        [HttpGet("ler_dividas")]
        public IActionResult GetDividas()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM divida WHERE user_id = @user_id AND ativo = TRUE", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            var reader = cmdSelect.ExecuteReader();
            var divida = new List<object>();
            while (reader.Read())
            {
                divida.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_invest")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr = reader.GetDouble(reader.GetOrdinal("vlr")),
                    data_init = reader.GetDateTime(reader.GetOrdinal("data")),
                    data_fim = reader.GetDateTime(reader.GetOrdinal("data_prev")),
                    resgate = reader.GetBoolean(reader.GetOrdinal("quitada"))
                });
            }
            return Ok(new { divida});
        }
        [HttpPost("criar_divida")]
        public IActionResult CriarDivida([FromBody] Divida dividaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inserção de nova dívida 
            var cmdInsert = new NpgsqlCommand("INSERT INTO divida (user_id, nome, vlr, data, data_prev, ativo, quitada) VALUES (@user_id, @descricao, @vlr, @data_init, @data_venc, TRUE, FALSE)", conn);
            cmdInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdInsert.Parameters.AddWithValue("@descricao", dividaData.descricao);
            cmdInsert.Parameters.AddWithValue("@vlr", dividaData.vlr);
            cmdInsert.Parameters.AddWithValue("@data_venc", dividaData.data_venc);
            cmdInsert.Parameters.AddWithValue("@data_init", dividaData.data_init);
            cmdInsert.ExecuteNonQuery();
            return Ok(new { message = "Dívida criada com sucesso" });
        }
        [HttpPut("atualizar_divida")]
        public IActionResult AtualizarDivida([FromBody] Divida_update dividaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Atualização de dívida
            var cmdUpdate = new NpgsqlCommand("UPDATE divida SET nome = @descricao, vlr = @vlr, data = @data_init, data_prev = @data_venc WHERE id_invest = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", dividaData.id);
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.Parameters.AddWithValue("@descricao", dividaData.descricao);
            cmdUpdate.Parameters.AddWithValue("@vlr", dividaData.vlr);
            cmdUpdate.Parameters.AddWithValue("@data_init", dividaData.data_init);
            cmdUpdate.Parameters.AddWithValue("@data_venc", dividaData.data_venc);
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Dívida atualizada com sucesso" });
        }
        [HttpDelete("inativar_divida")]
        public IActionResult InativarDivida([FromBody] Dictionary<string, string> dividaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inativação de dívida
            var cmdDelete = new NpgsqlCommand("UPDATE divida SET ativo = FALSE WHERE id_invest = @id AND user_id = @user_id", conn);
            cmdDelete.Parameters.AddWithValue("@id", int.Parse(dividaData["id_divida"]));
            cmdDelete.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdDelete.ExecuteNonQuery();
            return Ok(new { message = "Dívida inativada com sucesso" });
        }
        [HttpPut("pagar_divida")]
        public IActionResult PagarDivida([FromBody] Dictionary<string, string> dividaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Pagamento de dívida
            var cmdUpdate = new NpgsqlCommand("UPDATE divida SET ativo = FALSE, quitada = TRUE WHERE id_invest = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", int.Parse(dividaData["id_divida"]));
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Dívida paga com sucesso" });
        }
        [HttpGet("ler_dividas_quitadas")]
        public IActionResult GetDividasQuitadas()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM divida WHERE user_id = @user_id AND ativo = FALSE AND quitada = TRUE", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            var reader = cmdSelect.ExecuteReader();
            var divida = new List<object>();
            while (reader.Read())
            {
                divida.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_invest")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr = reader.GetDouble(reader.GetOrdinal("vlr")),
                    data_init = reader.GetDateTime(reader.GetOrdinal("data")),
                    data_fim = reader.GetDateTime(reader.GetOrdinal("data_prev")),
                    resgate = reader.GetBoolean(reader.GetOrdinal("quitada"))
                });
            }
            return Ok(new { divida });
        }

        //Metas
        [HttpGet("ler_metas")]
        public IActionResult GetMetas()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM meta WHERE user_id = @user_id AND ativo = TRUE", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            var reader = cmdSelect.ExecuteReader();
            var meta = new List<object>();
            while (reader.Read())
            {
                meta.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_meta")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr = reader.GetDecimal(reader.GetOrdinal("vlr")),
                    data_init = reader.GetDateTime(reader.GetOrdinal("data_meta")),
                    data_fim = reader.GetDecimal(reader.GetOrdinal("progresso"))
                });
            }
            return Ok(new { meta });
        }
        [HttpPost("criar_meta")]
        public IActionResult CriarMeta([FromBody] Metas metaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inserção de nova meta 
            var cmdInsert = new NpgsqlCommand("INSERT INTO meta (user_id, nome, vlr, data_meta, progresso, ativo) VALUES (@user_id, @descricao, @vlr, @data_prev, 0, TRUE)", conn);
            cmdInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdInsert.Parameters.AddWithValue("@descricao", metaData.descricao);
            cmdInsert.Parameters.AddWithValue("@vlr", metaData.vlr);
            cmdInsert.Parameters.AddWithValue("@data_prev", metaData.data_venc);
            cmdInsert.ExecuteNonQuery();
            return Ok(new { message = "Meta criada com sucesso" });
        }
        [HttpPut("atualizar_meta")]
        public IActionResult AtualizarMeta([FromBody] Metas_update metaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Atualização de meta
            var cmdUpdate = new NpgsqlCommand("UPDATE meta SET nome = @descricao, vlr = @vlr, data_meta = @data_prev, progresso = @progresso WHERE id_meta = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", metaData.id);
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.Parameters.AddWithValue("@descricao", metaData.descricao);
            cmdUpdate.Parameters.AddWithValue("@vlr", metaData.vlr);
            cmdUpdate.Parameters.AddWithValue("@data_prev", metaData.data_venc);
            cmdUpdate.Parameters.AddWithValue("@progresso", metaData.progresso);
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Meta atualizada com sucesso" });
        }
        [HttpDelete("inativar_meta")]
        public IActionResult InativarMeta([FromBody] Dictionary<string, string> metaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inativação de meta
            var cmdDelete = new NpgsqlCommand("UPDATE meta SET ativo = FALSE WHERE id_meta = @id AND user_id = @user_id", conn);
            cmdDelete.Parameters.AddWithValue("@id", int.Parse(metaData["id_meta"]));
            cmdDelete.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdDelete.ExecuteNonQuery();
            return Ok(new { message = "Meta inativada com sucesso" });
        }
        [HttpPut("concluir_meta")]
        public IActionResult ConcluirMeta([FromBody] Dictionary<string, string> metaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Conclusão de meta
            var cmdUpdate = new NpgsqlCommand("UPDATE meta SET ativo = FALSE, progresso = 100 WHERE id_meta = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", int.Parse(metaData["id_meta"]));
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Meta concluída com sucesso" });
        }
        [HttpGet("ler_metas_concluidas")]
        public IActionResult GetMetasConcluidas()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM meta WHERE user_id = @user_id AND ativo = FALSE AND progresso = 100", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            var reader = cmdSelect.ExecuteReader();
            var meta = new List<object>();
            while (reader.Read())
            {
                meta.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_meta")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr = reader.GetDecimal(reader.GetOrdinal("vlr")),
                    data_init = reader.GetDateTime(reader.GetOrdinal("data_meta")),
                    data_fim = reader.GetDecimal(reader.GetOrdinal("progresso"))
                });
            }
            return Ok(new { meta });
        }

        //Gastos
        [HttpGet("ler_gastos")]
        public IActionResult GetGastos()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand("SELECT * FROM gastos WHERE user_id = @user_id AND ativo = TRUE", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            var reader = cmdSelect.ExecuteReader();
            var gasto = new List<object>();
            while (reader.Read())
            {
                gasto.Add(new
                {
                    id = reader.GetInt32(reader.GetOrdinal("id_gasto")),
                    descricao = reader.GetString(reader.GetOrdinal("nome")),
                    vlr_min = reader.GetDecimal(reader.GetOrdinal("vlr_min")),
                    vlr_max = reader.GetDecimal(reader.GetOrdinal("vlr_max")),
                    prioridade = reader.GetInt32(reader.GetOrdinal("prioridade")),
                    data_init = reader.GetDateTime(reader.GetOrdinal("data_venc")),
                    fix_var = reader.GetBoolean(reader.GetOrdinal("fixvar"))
                });
            }
            return Ok(new { gasto });
        }
        [HttpPost("criar_gasto")]
        public IActionResult CriarGasto([FromBody] Gasto gastoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inserção de novo gasto 
            var cmdInsert = new NpgsqlCommand("INSERT INTO gastos (user_id, nome, vlr_min, vlr_max, data_venc, prioridade, fixvar, ativo) VALUES (@user_id, @descricao, @vlr_min, @vlr_max, @data_venc, @prioridade, @fixvar, TRUE)", conn);
            cmdInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdInsert.Parameters.AddWithValue("@descricao", gastoData.descricao);
            cmdInsert.Parameters.AddWithValue("@vlr_min", gastoData.vlr_min);
            cmdInsert.Parameters.AddWithValue("@vlr_max", gastoData.vlr_max);
            cmdInsert.Parameters.AddWithValue("@data_venc", gastoData.data);
            cmdInsert.Parameters.AddWithValue("@prioridade", gastoData.prioridade);
            cmdInsert.Parameters.AddWithValue("@fixvar", gastoData.fixvar);
            cmdInsert.ExecuteNonQuery();
            return Ok(new { message = "Gasto criado com sucesso" });
        }
        [HttpPut("atualizar_gasto")]
        public IActionResult AtualizarGasto([FromBody] Gasto_update gastoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Atualização de gasto
            var cmdUpdate = new NpgsqlCommand("UPDATE gastos SET nome = @descricao, vlr_min = @vlr_min, vlr_max = @vlr_max, data_venc = @data_venc, prioridade = @prioridade, fixvar = @fixvar WHERE id_gasto = @id AND user_id = @user_id", conn);
            cmdUpdate.Parameters.AddWithValue("@id", gastoData.id);
            cmdUpdate.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdUpdate.Parameters.AddWithValue("@descricao", gastoData.descricao);
            cmdUpdate.Parameters.AddWithValue("@vlr_min", gastoData.vlr_min);
            cmdUpdate.Parameters.AddWithValue("@vlr_max", gastoData.vlr_max);
            cmdUpdate.Parameters.AddWithValue("@data_venc", gastoData.data);
            cmdUpdate.Parameters.AddWithValue("@prioridade", gastoData.prioridade);
            cmdUpdate.Parameters.AddWithValue("@fixvar", gastoData.fixvar);
            cmdUpdate.ExecuteNonQuery();
            return Ok(new { message = "Gasto atualizado com sucesso" });
        }
        [HttpDelete("inativar_gasto")]
        public IActionResult InativarGasto([FromBody] Dictionary<string, string> gastoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inativação de gasto
            var cmdDelete = new NpgsqlCommand("UPDATE gastos SET ativo = FALSE WHERE id_gasto = @id AND user_id = @user_id", conn);
            cmdDelete.Parameters.AddWithValue("@id", int.Parse(gastoData["id_gasto"]));
            cmdDelete.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdDelete.ExecuteNonQuery();
            return Ok(new { message = "Gasto inativado com sucesso" });
        }
    }
}