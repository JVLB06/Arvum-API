using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.InputMappers;
using Presentation.WebModels;
using System.Security.Claims;

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

        [HttpGet("ler_extrato")]
        public async Task<IActionResult> ReadExtract([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.GetExtractAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("incluir_lancamento")]
        public async Task<IActionResult> CreateEntry([FromBody] NewExtractModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.SetExtractAsync(NewExtractMapper.ToInput(model), int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("atualizar_lancamento")]
        public async Task<IActionResult> UpdateEntry([FromBody] NewExtractModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                await _service.UpdateExtractAsync(NewExtractMapper.ToInput(model), int.Parse(userId));

                return Ok(new { message = "Extrato atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("remover_lancamento")]
        public async Task<IActionResult> RemoveEntry([FromBody] ExtractDeleteModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                await _service.DeleteExtractAsync(ExtractDeleteMapper.ToInput(model), int.Parse(userId));

                return Ok(new { message = "Extrato atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("obter_meta_pgto")]
        public async Task<IActionResult> GetGoalPayments([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.GetGoalPaymentsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("obter_gastos_pgto")]
        public async Task<IActionResult> GetExpenses([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.GetExpensePayementsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("obter_divida_pgto")]
        public async Task<IActionResult> GetDebts([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.GetDebtPaymentsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("obter_renda_pgto")]
        public async Task<IActionResult> GetReceipts([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.GetReceiptPaymentsAsync(int.Parse(userId), extract.InitialDate, extract.EndDate));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("obter_investimento_pgto")]
        public async Task<IActionResult> GetInvestiments([FromQuery] GetExtractModel extract)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
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
