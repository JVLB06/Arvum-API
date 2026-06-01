using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Presentation.InputMappers;
using Presentation.WebModels;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("user_plan")]
    public class GeneralRegistersController : ControllerBase
    {
        private readonly IGeneralReceiptsService _receiptsService;
        private readonly IGeneralInvestmentsService _investmentsService;
        private readonly IGeneralDebtsService _debtsService;

        public GeneralRegistersController(
            IGeneralReceiptsService receiptsService, 
            IGeneralInvestmentsService investmentsService,
            IGeneralDebtsService debtsService)
        {
            _receiptsService = receiptsService;
            _investmentsService = investmentsService;
            _debtsService = debtsService;
        }

        #region Rendas
        [HttpGet("ler_renda")]
        public async Task<IActionResult> GetRenda()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            try
            {
                return Ok(await _receiptsService.GetReceiptsAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new {message  = ex.Message});
            }
        }
        [HttpPost("criar_renda")]
        public async Task<IActionResult> CreateReceipt([FromBody] RegisterReceiptModel receipt)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _receiptsService.CreateReceiptAsync(ReceiptMapper.ToDTO(receipt), int.Parse(userId));

                return Ok(new { message = "Renda cadastrada com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("atualizar_renda")]
        public async Task<IActionResult> AtualizarRenda([FromBody] RegisterReceiptModel receipt)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _receiptsService.UpdateReceiptAsync(ReceiptMapper.ToDTO(receipt), int.Parse(userId));

                return Ok(new { message = "Renda cadastrada com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("inativar_renda/{receiptId}")]
        public async Task<IActionResult> InativarRenda([FromRoute] string receiptId)
        {
            try
            {
                await _receiptsService.DeleteReceiptAsync(int.Parse(receiptId));

                return Ok(new { message = "Renda cadastrada com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
        #region Investimentos
        [HttpGet("ler_investimentos_ativos")]
        public async Task<IActionResult> GetInvestimentosAtivos()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _investmentsService.GetActiveInvestmentsAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("ler_investimentos_encerrados")]
        public async Task<IActionResult> GetInvestimentosEncerrados()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _investmentsService.GetInactiveInvestmentsAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("criar_investimento")]
        public async Task<IActionResult> CriarInvestimento([FromBody] RegisterInvestmentModel investment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _investmentsService.CreateInvestmentAsync(InvestmentMapper.ToDto(investment), int.Parse(userId));

                return Ok(new { message = "Investimento cadastrado com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("atualizar_investimento")]
        public async Task<IActionResult> AtualizarInvestimento([FromBody] RegisterInvestmentModel investment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _investmentsService.UpdateInvestmentAsync(InvestmentMapper.ToDto(investment), int.Parse(userId));

                return Ok(new { message = "Investimento atualizado com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("inativar_investimento/{investmentId}")]
        public async Task<IActionResult> InativarInvestimento([FromRoute] int investmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _investmentsService.DeleteInvestmentAsync(investmentId, int.Parse(userId));

                return Ok(new { message = "Investimento excluído com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("concluir_investimento")]
        public async Task<IActionResult> ConcluirInvestimento([FromBody] RegisterFinishInvestmentModel investment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            try
            {
                await _investmentsService.FinishInvestmentAsync(FinishInvestmentMapper.ToDTO(investment), int.Parse(userId));

                return Ok(new { message = "Investimento finalizado com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
        #region Dividas
        [HttpGet("ler_dividas")]
        public async Task<IActionResult> GetDividas()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 

            try
            {
                return Ok(await _debtsService.GetDebtsAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("criar_divida")]
        public async Task<IActionResult> CriarDivida([FromBody] RegisterDebtModel debt)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _debtsService.RegisterDebtAsync(DebtMapper.ToDTO(debt), int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("atualizar_divida")]
        public async Task<IActionResult> AtualizarDivida([FromBody] RegisterDebtModel debt)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _debtsService.UpdateDebtAsync(DebtMapper.ToDTO(debt), int.Parse(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("inativar_divida/{debtId}")]
        public async Task<IActionResult> InativarDivida([FromRoute] int debtId)
        {
            try
            {
                return Ok(await _debtsService.DeleteDebtAsync(debtId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("pagar_divida/{dbtId}")]
        public async Task<IActionResult> PagarDivida([FromRoute] int debtId)
        {
            try
            {
                return Ok(await _debtsService.PayDebtAsync(debtId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("ler_dividas_quitadas")]
        public async Task<IActionResult> GetDividasQuitadas()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _debtsService.GetPaidDebtsAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
        #region Metas
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
        public IActionResult CriarMeta([FromBody] MetasModel metaData)
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
        public IActionResult AtualizarMeta([FromBody] MetasUpdateModel metaData)
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
        [HttpDelete("inativar_meta/{metaData}")]
        public IActionResult InativarMeta([FromRoute] string metaData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inativação de meta
            var cmdDelete = new NpgsqlCommand("UPDATE meta SET ativo = FALSE WHERE id_meta = @id AND user_id = @user_id", conn);
            cmdDelete.Parameters.AddWithValue("@id", int.Parse(metaData));
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
        #endregion
        #region Gastos
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
        public IActionResult CriarGasto([FromBody] GastoModel gastoData)
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
        public IActionResult AtualizarGasto([FromBody] GastoUpdateModel gastoData)
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
        [HttpDelete("inativar_gasto/{gastoData}")]
        public IActionResult InativarGasto([FromRoute] string gastoData)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            // Inativação de gasto
            var cmdDelete = new NpgsqlCommand("UPDATE gastos SET ativo = FALSE WHERE id_gasto = @id AND user_id = @user_id", conn);
            cmdDelete.Parameters.AddWithValue("@id", int.Parse(gastoData));
            cmdDelete.Parameters.AddWithValue("@user_id", int.Parse(usuarioId));
            cmdDelete.ExecuteNonQuery();
            return Ok(new { message = "Gasto inativado com sucesso" });
        }
        #endregion
    }
}