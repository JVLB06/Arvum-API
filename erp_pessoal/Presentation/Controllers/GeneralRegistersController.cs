using Application.Interfaces;
using Infrastructure.BaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Presentation.InputMappers;
using Presentation.WebModels;
using System.Security.Claims;
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
        private readonly IGeneralGoalsService _goalsService;
        private readonly IGeneralExpensesService _expensesService;

        public GeneralRegistersController(
            IGeneralReceiptsService receiptsService, 
            IGeneralInvestmentsService investmentsService,
            IGeneralDebtsService debtsService,
            IGeneralGoalsService goalsService,
            IGeneralExpensesService expensesService)
        {
            _receiptsService = receiptsService;
            _investmentsService = investmentsService;
            _debtsService = debtsService;
            _goalsService = goalsService;
            _expensesService = expensesService;
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
                await _investmentsService.CreateInvestmentAsync(InvestmentMapper.ToDTO(investment), int.Parse(userId));

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
                await _investmentsService.UpdateInvestmentAsync(InvestmentMapper.ToDTO(investment), int.Parse(userId));

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
                await _debtsService.RegisterDebtAsync(DebtMapper.ToDTO(debt), int.Parse(userId));
                return Ok(new { message = "Dívida criada com sucesso" });
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
                await _debtsService.UpdateDebtAsync(DebtMapper.ToDTO(debt), int.Parse(userId));
                return Ok(new { message = "Dívida atualizada com sucesso" });
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
                await _debtsService.DeleteDebtAsync(debtId);
                return Ok(new { message = "Dívida cancelada com sucesso" });
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
                await _debtsService.PayDebtAsync(debtId);
                return Ok(new { message = "Dívida paga com sucesso" });
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
        public async Task<IActionResult> GetMetas()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _goalsService.GetActiveGoalsAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("criar_meta")]
        public async Task<IActionResult> CriarMeta([FromBody] RegisterGoalModel goal)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _goalsService.RegisterGoalAsync(GoalMapper.ToDTO(goal), int.Parse(userId));
                return Ok(new { message = "Meta criada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("atualizar_meta")]
        public async Task<IActionResult> AtualizarMeta([FromBody] RegisterGoalModel goal)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _goalsService.UpdateGoalAsync(GoalMapper.ToDTO(goal), int.Parse(userId));
                return Ok(new { message = "Meta atualizada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("inativar_meta/{goalId}")]
        public async Task<IActionResult> InativarMeta([FromRoute] int goalId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _goalsService.DeleteGoalAsync(goalId);
                return Ok(new { message = "Meta cancelada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("concluir_meta/{goalId}")]
        public async Task<IActionResult> ConcluirMeta([FromRoute] int goalId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _goalsService.EndGoalAsync(goalId);
                return Ok(new { message = "Meta concluída com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("ler_metas_concluidas")]
        public async Task<IActionResult> GetMetasConcluidas()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _goalsService.GetDoneGoalsAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
        #region Gastos
        //Gastos
        [HttpGet("ler_gastos")]
        public async Task<IActionResult> GetGastos()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                return Ok(await _expensesService.GetExpensesAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("criar_gasto")]
        public async Task<IActionResult> CriarGasto([FromBody] RegisterExpenseModel expense)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
            
            try {                 
                await _expensesService.RegisterExpenseAsync(ExpenseMapper.ToDTO(expense), int.Parse(userId));
                return Ok(new { message = "Gasto cadastrado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("atualizar_gasto")]
        public async Task<IActionResult> AtualizarGasto([FromBody] RegisterExpenseModel expense)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _expensesService.UpdateExpenseAsync(ExpenseMapper.ToDTO(expense), int.Parse(userId));
                return Ok(new { message = "Gasto atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("inativar_gasto/{expenseId}")]
        public async Task<IActionResult> InativarGasto([FromRoute] int expenseId)
        {
            try
            {
                await _expensesService.DeleteExpenseAsync(expenseId);
                return Ok(new { message = "Gasto inativado com sucesso" });
            }   
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
    }
}