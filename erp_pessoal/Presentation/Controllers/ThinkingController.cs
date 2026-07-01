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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.GeneratePreferencesAsync(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ler_preferencias")]
        public async Task<IActionResult> ReadPreferences()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                return Ok(await _service.GetPreferences(int.Parse(userId)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("criar_preferencias")]
        public async Task<IActionResult> CreatePreferences([FromBody] PreferenceModel preference)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                await _service.CreatePreference(PreferencesMapper.ToInput(preference), int.Parse(userId));
                return Ok("Preferência criada com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("deletar_preferencia/{id}")]
        public async Task<IActionResult> DeletePreference([FromQuery] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User Id Obtainement
            try
            {
                await _service.DeletePreference(id, int.Parse(userId));
                return Ok("Preferência excluída com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
