using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using erp_pessoal.Models;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;
namespace erp_pessoal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("extrato")]
    public class ExtratoController : ControllerBase
    {
        //Recálculo de saldos
        private async Task AtualizarSaldosAsync(NpgsqlConnection conn, int userId, DateTime data)
        {
            decimal saldoAtual = 0;

            // 1. Obter saldo anterior
            var cmdSaldoAnterior = new NpgsqlCommand(
                @"SELECT saldo FROM extrato
                WHERE user_id = @user_id AND data < @data AND ativo = TRUE
                ORDER BY data DESC, id_lcto DESC
                LIMIT 1;", conn);

            cmdSaldoAnterior.Parameters.AddWithValue("@user_id", userId);
            cmdSaldoAnterior.Parameters.AddWithValue("@data", data);

            var saldoAnteriorObj = await cmdSaldoAnterior.ExecuteScalarAsync();
            saldoAtual = saldoAnteriorObj != null ? Convert.ToDecimal(saldoAnteriorObj) : 0;

            // 2. Buscar lançamentos futuros (inclusive o novo)
            var cmdLancamentos = new NpgsqlCommand(
                @"SELECT id_lcto, vlr FROM extrato
                WHERE user_id = @user_id AND data >= @data AND ativo = TRUE
                ORDER BY data ASC, id_lcto ASC;", conn);

            cmdLancamentos.Parameters.AddWithValue("@user_id", userId);
            cmdLancamentos.Parameters.AddWithValue("@data", data);

            var atualizacoes = new List<(int id_lcto, decimal novoSaldo)>();

            await using (var reader = await cmdLancamentos.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("id_lcto"));
                    decimal valor = reader.GetDecimal(reader.GetOrdinal("vlr"));
                    saldoAtual += valor;
                    atualizacoes.Add((id, saldoAtual));
                }
            }

            // 3. Verificar se há algo para atualizar
            if (!atualizacoes.Any())
            {
                Console.WriteLine("Nenhum lançamento futuro encontrado para atualizar.");
                return;
            }

            // 4. Atualizar saldos
            foreach (var (id, novoSaldo) in atualizacoes)
            {
                var cmdUpdate = new NpgsqlCommand(
                    "UPDATE extrato SET saldo = @saldo WHERE id_lcto = @id_lcto;", conn);
                cmdUpdate.Parameters.AddWithValue("@saldo", novoSaldo);
                cmdUpdate.Parameters.AddWithValue("@id_lcto", id);
                await cmdUpdate.ExecuteNonQueryAsync();
            }
        }

        //Inclusão de informações de extrato
        [HttpPost("incluir_lancamento")]
        public async Task<IActionResult> IncluirLcto([FromBody] ExtratoModel extData)
        {
            // Obtendo ID do usuário
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized("Usuário não autenticado");

            // Estabelecendo conexão com o banco de dados
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            await conn.OpenAsync();

            // Inserir lançamento no extrato com saldo temporário 0
            var cmdInsert = new NpgsqlCommand(
                @"INSERT INTO extrato (data, historico, vlr, saldo, ativo, user_id)
                VALUES (@data, @historico, @vlr, 0, TRUE, @usuario_id)
                RETURNING id_lcto;", conn);

            cmdInsert.Parameters.AddWithValue("@data", extData.data);
            cmdInsert.Parameters.AddWithValue("@historico", extData.historico);
            cmdInsert.Parameters.AddWithValue("@vlr", extData.valor);
            cmdInsert.Parameters.AddWithValue("@usuario_id", int.Parse(usuarioId));

            var idLcto = await cmdInsert.ExecuteScalarAsync();
            if (idLcto == null)
                return BadRequest("Erro ao incluir lançamento no extrato.");
            await AtualizarSaldosAsync(conn, int.Parse(usuarioId), extData.data);

            //Inclusão do pagamento associado ao lançamento
            switch (extData.tipo)
            {
                case "gasto":
                    var gastoInsert = new NpgsqlCommand(
                        "INSERT INTO pagamentos (historico, vlr, data, gasto_id, user_id) " +
                        "VALUES (@historico, @vlr, @data, @gasto_id, @user_id) " +
                        "RETURNING id_gasto_geral;", conn);
                    gastoInsert.Parameters.AddWithValue("@historico", extData.historico);
                    gastoInsert.Parameters.AddWithValue("@vlr", extData.valor);
                    gastoInsert.Parameters.AddWithValue("@data", extData.data);
                    gastoInsert.Parameters.AddWithValue("@gasto_id", extData.id_ref);
                    gastoInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    var idGasto = await gastoInsert.ExecuteScalarAsync();
                    if (idGasto == null)
                        return BadRequest("Erro ao incluir gasto.");
                    else
                        return Ok(idGasto);
                case "divida":
                    var dividaInsert = new NpgsqlCommand(
                        "INSERT INTO pagamentos (historico, vlr, data, divida_id, user_id) " +
                        "VALUES (@historico, @vlr, @data, @divida_id, @user_id) " +
                        "RETURNING id_pgto_divida;", conn);
                    dividaInsert.Parameters.AddWithValue("@historico", extData.historico);
                    dividaInsert.Parameters.AddWithValue("@vlr", extData.valor);
                    dividaInsert.Parameters.AddWithValue("@data", extData.data);
                    dividaInsert.Parameters.AddWithValue("@divida_id", extData.id_ref);
                    dividaInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    var idDivida = await dividaInsert.ExecuteScalarAsync();
                    if (idDivida == null)
                        return BadRequest("Erro ao incluir pagamento de dívida.");
                    else
                        return Ok(idDivida);
                case "meta":
                    var metaInsert = new NpgsqlCommand(
                        "INSERT INTO meta_pgto (historico, vlr, data, meta_invest_id, user_id) " +
                        "VALUES (@historico, @vlr, @data, @meta_id, @user_id) " +
                        "RETURNING id_pgto_meta;", conn);
                    metaInsert.Parameters.AddWithValue("@historico", extData.historico);
                    metaInsert.Parameters.AddWithValue("@vlr", extData.valor);
                    metaInsert.Parameters.AddWithValue("@data", extData.data);
                    metaInsert.Parameters.AddWithValue("@meta_id", extData.id_ref);
                    metaInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    var idMeta = await metaInsert.ExecuteScalarAsync();
                    if (idMeta == null)
                        return BadRequest("Erro ao incluir pagamento de meta.");
                    else
                        return Ok(idMeta);
                case "investimento":
                    var investimentoInsert = new NpgsqlCommand(
                        "INSERT INTO investimento_pgto (historico, vlr, data, invest_id, user_id) " +
                        "VALUES (@historico, @vlr, @data, @invest_id, @user_id) " +
                        "RETURNING id_invest;", conn);
                    investimentoInsert.Parameters.AddWithValue("@historico", extData.historico);
                    investimentoInsert.Parameters.AddWithValue("@vlr", extData.valor);
                    investimentoInsert.Parameters.AddWithValue("@data", extData.data);
                    investimentoInsert.Parameters.AddWithValue("@invest_id", extData.id_ref);
                    investimentoInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    var idInvestimento = await investimentoInsert.ExecuteScalarAsync();
                    if (idInvestimento == null)
                        return BadRequest("Erro ao incluir pagamento de investimento.");
                    else
                        return Ok(idInvestimento);
                case "renda":
                    var rendaInsert = new NpgsqlCommand(
                        "INSERT INTO renda_pgto (historico, vlr, data, renda_id, user_id) " +
                        "VALUES (@historico, @vlr, @data, @renda_id, @user_id) " +
                        "RETURNING id_renda;", conn);
                    rendaInsert.Parameters.AddWithValue("@historico", extData.historico);
                    rendaInsert.Parameters.AddWithValue("@vlr", extData.valor);
                    rendaInsert.Parameters.AddWithValue("@data", extData.data);
                    rendaInsert.Parameters.AddWithValue("@renda_id", extData.id_ref);
                    rendaInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    var idRenda = await rendaInsert.ExecuteScalarAsync();
                    if (idRenda == null)
                        return BadRequest("Erro ao incluir pagamento de renda.");
                    else
                        return Ok(idRenda);
                default:
                    return BadRequest("Tipo de lançamento inválido.");
            }
        }

        [HttpPut("atualizar_lancamento")]
        public async Task<IActionResult> AtualizarLcto([FromBody] ExtratoUpdateModel extData)
        {
            // Obtendo ID do usuário
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized("Usuário não autenticado");
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            await conn.OpenAsync();
            // Atualizar lançamento no extrato
            var cmdUpdate = new NpgsqlCommand(
                @"UPDATE extrato SET data = @data, historico = @historico, vlr = @vlr
                WHERE id_lcto = @id_lcto AND user_id = @usuario_id AND ativo = TRUE;", conn);
            cmdUpdate.Parameters.AddWithValue("@data", extData.data);
            cmdUpdate.Parameters.AddWithValue("@historico", extData.historico);
            cmdUpdate.Parameters.AddWithValue("@vlr", extData.valor);
            cmdUpdate.Parameters.AddWithValue("@id_lcto", extData.id);
            cmdUpdate.Parameters.AddWithValue("@usuario_id", int.Parse(usuarioId));
            var rowsAffected = await cmdUpdate.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound("Lançamento não encontrado ou não pertence ao usuário.");
            await AtualizarSaldosAsync(conn, int.Parse(usuarioId), extData.data);
            return Ok("Lançamento atualizado com sucesso.");
        }

        [HttpDelete("remover_lancamento")]
        public async Task<IActionResult> RemoverLcto([FromBody] ExtratoDeleteModel extData)
        {
            // Obtendo ID do usuário
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized("Usuário não autenticado");
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            await conn.OpenAsync();
            // Marcar lançamento como inativo
            var cmdDelete = new NpgsqlCommand(
                @"UPDATE extrato SET ativo = FALSE WHERE id_lcto = @id_lcto AND user_id = @usuario_id;", conn);
            cmdDelete.Parameters.AddWithValue("@id_lcto", id);
            cmdDelete.Parameters.AddWithValue("@usuario_id", int.Parse(usuarioId));
            var rowsAffected = await cmdDelete.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound("Lançamento não encontrado ou não pertence ao usuário.");
            return Ok("Lançamento removido com sucesso.");
        }
        
        //Visualização com os joins
        //Meta_pgto
        [HttpGet("obter_meta_pgto")]
        public async Task<IActionResult> GetMeta()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand(
                "SELECT mp.id_pgto_meta, e.data, e.historico, mp.vlr, m.id_meta," +
                "m.nome, m.vlr as meta_valor, m.data_meta, m.progresso, e.saldo" +
                "FROM meta_pgto mp" +
                "JOIN extrato e ON e.id_lcto = mp.lcto_id" +
                "JOIN meta m ON m.id_meta = mp.meta_invest_id" +
                "JOIN usuarios u ON u.id = @user_id" +
                "WHERE mp.ativo = TRUE" +
                "AND e.ativo = TRUE" +
                "AND u.id = 2" +
                "ORDER BY e.data desc;", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            await cmdSelect.PrepareAsync();
            var reader = await cmdSelect.ExecuteReaderAsync();
            var meta = new List<object>();
            while (await reader.ReadAsync())
            {
                meta.Add(new
                {
                    id_pgto_meta = reader.GetInt32(reader.GetOrdinal("id_pgto_meta")),
                    data = reader.GetDateTime(reader.GetOrdinal("data")),
                    historico = reader.GetString(reader.GetOrdinal("historico")),
                    vlr_pagamento = reader.GetDouble(reader.GetOrdinal("vlr")),
                    meta = new
                    {
                        id_meta = reader.GetInt32(reader.GetOrdinal("id_meta")),
                        nome = reader.GetString(reader.GetOrdinal("nome")),
                        valor = reader.GetDouble(reader.GetOrdinal("meta_valor")),
                        data_meta = reader.GetDateTime(reader.GetOrdinal("data_meta")),
                        progresso = reader.GetDouble(reader.GetOrdinal("progresso"))
                    },
                    saldo_extrato = reader.GetDouble(reader.GetOrdinal("saldo"))
                });
            }
            return Ok(new { meta });
        }

        //Pagamentos (gastos geral)
        [HttpGet("obter_gastos_pgto")]
        public async Task<IActionResult> GetGastos()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand(
                "SELECT p.id_gasto_geral, e.data, e.historico, p.vlr, g.id_gasto," +
                "g.nome, (g.vlr_min + g.vlr_max)/2 as gasto_valor, g.data_venc, g.fixvar, e.saldo" +
                "FROM pagamentos p" +
                "JOIN extrato e ON e.id_lcto = p.lcto_id" + 
                "JOIN gastos g ON g.id_gasto = p.gasto_id" + 
                "JOIN usuarios u ON u.id = @user_id" +
                "where p.ativo = TRUE" +
                "AND e.ativo = TRUE" +
                "AND u.id = 2" +
                "ORDER BY e.data desc;", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            await cmdSelect.PrepareAsync();
            var reader = await cmdSelect.ExecuteReaderAsync();
            var gastos = new List<object>();
            while (await reader.ReadAsync())
            {
                gastos.Add(new
                {
                    id_gasto_geral = reader.GetInt32(reader.GetOrdinal("id_gasto_geral")),
                    data = reader.GetDateTime(reader.GetOrdinal("data")),
                    historico = reader.GetString(reader.GetOrdinal("historico")),
                    vlr_pagamento = reader.GetDouble(reader.GetOrdinal("vlr")),
                    gasto = new
                    {
                        id_gasto = reader.GetInt32(reader.GetOrdinal("id_gasto")),
                        nome = reader.GetString(reader.GetOrdinal("nome")),
                        valor = reader.GetDouble(reader.GetOrdinal("gasto_valor")),
                        data_venc = reader.GetDateTime(reader.GetOrdinal("data_venc")),
                        progresso = reader.GetBoolean(reader.GetOrdinal("fixvar"))
                    },
                    saldo_extrato = reader.GetDouble(reader.GetOrdinal("saldo"))
                });
            }
            return Ok(new { gastos });
        }

        //Divida_pgto
        [HttpGet("obter_divida_pgto")]
        public async Task<IActionResult> GetDivida()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand(
                "SELECT dp.id_pgto_divida, e.data, e.historico, dp.vlr, d.id_invest," +
                "d.nome, d.vlr as divida_valor, d.data , d.data_prev, e.saldo" +
                "FROM divida_pgto dp" +
                "JOIN extrato e ON e.id_lcto = dp.lcto_id" +
                "JOIN divida d ON d.id_invest = dp.divida_id" +
                "JOIN usuarios u ON u.id = @user_id" +
                "where dp.ativo = TRUE" +
                "AND e.ativo = TRUE" +
                "AND u.id = 2" +
                "ORDER BY e.data desc;", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            await cmdSelect.PrepareAsync();
            var reader = await cmdSelect.ExecuteReaderAsync();
            var dividas = new List<object>();
            while (await reader.ReadAsync())
            {
                dividas.Add(new
                {
                    id_divida = reader.GetInt32(reader.GetOrdinal("id_divida")),
                    data = reader.GetDateTime(reader.GetOrdinal("data")),
                    historico = reader.GetString(reader.GetOrdinal("historico")),
                    vlr_pagamento = reader.GetDouble(reader.GetOrdinal("vlr")),
                    divida_item = new
                    {
                        id_divida_item = reader.GetInt32(reader.GetOrdinal("id_pgto_divida")),
                        nome = reader.GetString(reader.GetOrdinal("nome")),
                        valor = reader.GetDouble(reader.GetOrdinal("divida_valor")),
                        data_init = reader.GetDateTime(reader.GetOrdinal("data")),
                        data_fim = reader.GetDateTime(reader.GetOrdinal("data_prev"))
                    },
                    saldo_extrato = reader.GetDouble(reader.GetOrdinal("saldo"))
                });
            }
            return Ok(new { dividas });
        }
        
        //Renda_pgto
        [HttpGet("obter_renda_pgto")]
        public async Task<IActionResult> GetRenda()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand(
                "SELECT rp.id_renda, e.data, e.historico, rp.vlr, r.id_renda as renda_id, " +
                "r.nome, (r.vlr_min+ r.vlr_max)/2 as renda_valor, r.data_pag, e.saldo" + 
                "FROM renda_pgto rp" +
                "JOIN rendas r ON r.id_renda = rp.renda_id" +
                "JOIN extrato e ON e.id_lcto = rp.lcto_id" +
                "JOIN usuarios u ON u.id = @user_id" +
                "where rp.ativo = TRUE" +
                "AND e.ativo = TRUE" +
                "ORDER BY e.data desc;", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            await cmdSelect.PrepareAsync();
            var reader = await cmdSelect.ExecuteReaderAsync();
            var rendas = new List<object>();
            while (await reader.ReadAsync())
            {
                rendas.Add(new
                {
                    id_divida = reader.GetInt32(reader.GetOrdinal("id_renda")),
                    data = reader.GetDateTime(reader.GetOrdinal("data")),
                    historico = reader.GetString(reader.GetOrdinal("historico")),
                    vlr_pagamento = reader.GetDouble(reader.GetOrdinal("vlr")),
                    divida_item = new
                    {
                        id_divida_item = reader.GetInt32(reader.GetOrdinal("renda_id")),
                        nome = reader.GetString(reader.GetOrdinal("nome")),
                        valor = reader.GetDouble(reader.GetOrdinal("renda_valor")),
                        data_init = reader.GetDateTime(reader.GetOrdinal("data_pag"))
                    },
                    saldo_extrato = reader.GetDouble(reader.GetOrdinal("saldo"))
                });
            }
            return Ok(new { rendas });
        }
        
        //Investimento_pgto
        [HttpGet("obter_investimento_pgto")]
        public async Task<IActionResult> GetInvestimento()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Realização do select
            var cmdSelect = new NpgsqlCommand(
                "SELECT ip.id_invest, e.data, e.historico, ip.vlr, i.id_invest as invest_id," +
                "i.nome, i.vlr as invest_valor, i.juro, i.data_init, e.saldo" +
                "FROM investimento_pgto ip" +
                "JOIN extrato e ON e.id_lcto = ip.lcto_id" +
                "JOIN investimentos i ON i.id_invest = ip.invest_id" +
                "JOIN usuarios u ON u.id = @user_id" +
                "where ip.ativo = TRUE" +
                "AND e.ativo = TRUE" +
                "AND u.id = 2" +
                "ORDER BY e.data desc;", conn);
            cmdSelect.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            await cmdSelect.PrepareAsync();
            var reader = await cmdSelect.ExecuteReaderAsync();
            var investimentos = new List<object>();
            while (await reader.ReadAsync())
            {
                investimentos.Add(new
                {
                    id_invest_pgto = reader.GetInt32(reader.GetOrdinal("id_invest")),
                    data = reader.GetDateTime(reader.GetOrdinal("data")),
                    historico = reader.GetString(reader.GetOrdinal("historico")),
                    vlr_pagamento = reader.GetDouble(reader.GetOrdinal("vlr")),
                    invest_item = new
                    {
                        id_invest_item = reader.GetInt32(reader.GetOrdinal("invest_id")),
                        nome = reader.GetString(reader.GetOrdinal("nome")),
                        valor = reader.GetDouble(reader.GetOrdinal("invest_valor")),
                        juro = reader.GetDouble(reader.GetOrdinal("juro")),
                        data_init = reader.GetDateTime(reader.GetOrdinal("data_init"))
                    },
                    saldo_extrato = reader.GetDouble(reader.GetOrdinal("saldo"))
                });
            }
            return Ok(new { investimentos });
        }
    }
}