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
    [Route("thinking")]
    public class ThinkingController : ControllerBase
    {
        private readonly IThinkingService _service;

        public ThinkingController(IThinkingService service)
        {
            _service = service;
        }

        [HttpGet("indicadores")]
        public async Task<IActionResult> GetIndicators()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                return Ok(await _service.GeneratePreferencesAsync(int.Parse(userId!)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ler_preferencias")]
        public async Task<IActionResult> ReadPreferences()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                return Ok(await _service.GetPreferences(int.Parse(userId!)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("criar_preferencias")]
        public async Task<IActionResult> CreatePreferences([FromBody] PreferenceModel preference)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                await _service.CreatePreference(PreferencesMapper.ToInput(preference), int.Parse(userId!));
                return Ok(new { message = "Preferência criada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("deletar_preferencia/{id}")]
        public async Task<IActionResult> DeletePreference([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                await _service.DeletePreference(id, int.Parse(userId!));
                return Ok(new { message = "Preferência excluída com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}