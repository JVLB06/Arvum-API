using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Presentation.WebModels;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("extrato")]
    public class SpecificRegistersController : ControllerBase
    {
        private readonly ISpecificRegistersService _service;

        public SpecificRegistersController(ISpecificRegistersService service)
        {
            _service = service;
        }

        //Tratamento de valores do extrato
        private decimal NormalizarValor(decimal valor, string tipo)
        {
            // garante valor positivo de base
            valor = Math.Abs(valor);

            return tipo switch
            {
                "gasto" => -valor,
                "divida" => -valor,
                "meta" => -valor,
                "investimento" => -valor,
                "renda" => valor,
                _ => valor
            };
        }

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

        [HttpGet("ler_extrato")]
        public async Task<IActionResult> ReadExtract([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            try
            {
                return Ok(await _service.GetExtractAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
            cmdInsert.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
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
                        "INSERT INTO pagamentos (historico, vlr, data, gasto_id, user_id, lcto_id) " +
                        "VALUES (@historico, @vlr, @data, @gasto_id, @user_id, @lcto_id) " +
                        "RETURNING id_gasto_geral;", conn);
                    gastoInsert.Parameters.AddWithValue("@historico", extData.historico);
                    gastoInsert.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
                    gastoInsert.Parameters.AddWithValue("@data", extData.data);
                    gastoInsert.Parameters.AddWithValue("@gasto_id", extData.id_ref);
                    gastoInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    gastoInsert.Parameters.AddWithValue("@lcto_id", idLcto);
                    var idGasto = await gastoInsert.ExecuteScalarAsync();
                    if (idGasto == null)
                        return BadRequest("Erro ao incluir gasto.");
                    else
                        return Ok(idGasto);
                case "divida":
                    var dividaInsert = new NpgsqlCommand(
                        "INSERT INTO pagamentos (historico, vlr, data, divida_id, user_id, lcto_id) " +
                        "VALUES (@historico, @vlr, @data, @divida_id, @user_id, @lcto_id) " +
                        "RETURNING id_pgto_divida;", conn);
                    dividaInsert.Parameters.AddWithValue("@historico", extData.historico);
                    dividaInsert.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
                    dividaInsert.Parameters.AddWithValue("@data", extData.data);
                    dividaInsert.Parameters.AddWithValue("@divida_id", extData.id_ref);
                    dividaInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    dividaInsert.Parameters.AddWithValue("@lcto_id", idLcto);
                    var idDivida = await dividaInsert.ExecuteScalarAsync();
                    if (idDivida == null)
                        return BadRequest("Erro ao incluir pagamento de dívida.");
                    else
                        return Ok(idDivida);
                case "meta":
                    var metaInsert = new NpgsqlCommand(
                        "INSERT INTO meta_pgto (historico, vlr, data, meta_invest_id, user_id, lcto_id) " +
                        "VALUES (@historico, @vlr, @data, @meta_id, @user_id, @lcto_id) " +
                        "RETURNING id_pgto_meta;", conn);
                    metaInsert.Parameters.AddWithValue("@historico", extData.historico);
                    metaInsert.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
                    metaInsert.Parameters.AddWithValue("@data", extData.data);
                    metaInsert.Parameters.AddWithValue("@meta_id", extData.id_ref);
                    metaInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    metaInsert.Parameters.AddWithValue("@lcto_id", idLcto);
                    var idMeta = await metaInsert.ExecuteScalarAsync();
                    if (idMeta == null)
                        return BadRequest("Erro ao incluir pagamento de meta.");
                    else
                        return Ok(idMeta);
                case "investimento":
                    var investimentoInsert = new NpgsqlCommand(
                        "INSERT INTO investimento_pgto (historico, vlr, data, invest_id, user_id, lcto_id) " +
                        "VALUES (@historico, @vlr, @data, @invest_id, @user_id, @lcto_id) " +
                        "RETURNING id_invest;", conn);
                    investimentoInsert.Parameters.AddWithValue("@historico", extData.historico);
                    investimentoInsert.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
                    investimentoInsert.Parameters.AddWithValue("@data", extData.data);
                    investimentoInsert.Parameters.AddWithValue("@invest_id", extData.id_ref);
                    investimentoInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    investimentoInsert.Parameters.AddWithValue("@lcto_id", idLcto);
                    var idInvestimento = await investimentoInsert.ExecuteScalarAsync();
                    if (idInvestimento == null)
                        return BadRequest("Erro ao incluir pagamento de investimento.");
                    else
                        return Ok(idInvestimento);
                case "renda":
                    var rendaInsert = new NpgsqlCommand(
                        "INSERT INTO renda_pgto (historico, vlr, data, renda_id, user_id, lcto_id) " +
                        "VALUES (@historico, @vlr, @data, @renda_id, @user_id, @lcto_id) " +
                        "RETURNING id_renda;", conn);
                    rendaInsert.Parameters.AddWithValue("@historico", extData.historico);
                    rendaInsert.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
                    rendaInsert.Parameters.AddWithValue("@data", extData.data);
                    rendaInsert.Parameters.AddWithValue("@renda_id", extData.id_ref);
                    rendaInsert.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
                    rendaInsert.Parameters.AddWithValue("@lcto_id", idLcto);
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
            cmdUpdate.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
            cmdUpdate.Parameters.AddWithValue("@id_lcto", extData.id);
            cmdUpdate.Parameters.AddWithValue("@usuario_id", int.Parse(usuarioId));
            var rowsAffected = await cmdUpdate.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound("Lançamento não encontrado ou não pertence ao usuário.");
            await AtualizarSaldosAsync(conn, int.Parse(usuarioId), extData.data);
            string sql;

            switch (extData.tipo)
            {
                case "gasto":
                    sql = @"UPDATE pagamentos 
                SET historico = @historico, vlr = @vlr, data = @data 
                WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;

                case "divida":
                    sql = @"UPDATE divida_pgto 
                SET historico = @historico, vlr = @vlr, data = @data 
                WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;

                case "meta":
                    sql = @"UPDATE meta_pgto 
                    SET historico = @historico, vlr = @vlr, data = @data 
                    WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;
                case "investimento":
                    sql = @"UPDATE investimento_pgto 
                    SET historico = @historico, vlr = @vlr, data = @data 
                    WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;
                case "renda":
                    sql = @"UPDATE renda_pgto 
                    SET historico = @historico, vlr = @vlr, data = @data 
                    WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;

                default:
                    return BadRequest("Tipo inválido para atualização.");
            }
            var updt_especific = new NpgsqlCommand(sql, conn);
            updt_especific.Parameters.AddWithValue("@historico", extData.historico);
            updt_especific.Parameters.AddWithValue("@vlr", NormalizarValor(extData.valor, extData.tipo));
            updt_especific.Parameters.AddWithValue("@data", extData.data);
            updt_especific.Parameters.AddWithValue("@id_lcto", extData.id);
            updt_especific.Parameters.AddWithValue("@usuario_id", int.Parse(usuarioId));
            var rowsAffectedEspecific = await updt_especific.ExecuteNonQueryAsync();
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
            cmdDelete.Parameters.AddWithValue("@id_lcto", extData.id);
            cmdDelete.Parameters.AddWithValue("@usuario_id", int.Parse(usuarioId));
            var rowsAffected = await cmdDelete.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound("Lançamento não encontrado ou não pertence ao usuário.");
            string sql;
            switch (extData.tipo)
            {
                case "gasto":
                    sql = @"UPDATE pagamentos SET ativo = FALSE WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;
                case "divida":
                    sql = @"UPDATE divida_pgto SET ativo = FALSE WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;
                case "meta":
                    sql = @"UPDATE meta_pgto SET ativo = FALSE WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;
                case "investimento":
                    sql = @"UPDATE investimento_pgto SET ativo = FALSE WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;
                case "renda":
                    sql = @"UPDATE renda_pgto SET ativo = FALSE WHERE lcto_id = @id_lcto AND user_id = @usuario_id;";
                    break;
                default:
                    return BadRequest("Tipo de lançamento inválido.");
            }
            var cmdDeleteall = new NpgsqlCommand(sql, conn);
            cmdDeleteall.Parameters.AddWithValue("@id_lcto", extData.id);
            cmdDeleteall.Parameters.AddWithValue("@usuario_id", int.Parse(usuarioId));
            await cmdDeleteall.ExecuteNonQueryAsync();
            return Ok("Lançamento removido com sucesso.");
        }
        
        [HttpGet("obter_meta_pgto")]
        public async Task<IActionResult> GetGoalPayments([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            try
            {
                return Ok(await _service.GetGoalPaymentsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Pagamentos (gastos geral)
        [HttpGet("obter_gastos_pgto")]
        public async Task<IActionResult> GetExpenses([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            try
            {
                return Ok(await _service.GetExpensePayementsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Divida_pgto
        [HttpGet("obter_divida_pgto")]
        public async Task<IActionResult> GetDebts([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            try
            {
                return Ok(await _service.GetDebtPaymentsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Renda_pgto
        [HttpGet("obter_renda_pgto")]
        public async Task<IActionResult> GetReceipts([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            try
            {
                return Ok(await _service.GetReceiptPaymentsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Investimento_pgto
        [HttpGet("obter_investimento_pgto")]
        public async Task<IActionResult> GetInvestiments([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            try
            {
                return Ok(await _service.GetInvestmentPaymentsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
