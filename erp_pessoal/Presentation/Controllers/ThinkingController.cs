using Application.Interfaces;
using erp_pessoal.Models;
using Infrastructure.BaseMappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Presentation.InputMappers;
using Presentation.WebModels;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("thinking")]
    public class ThinkingController : ControllerBase
    {
        private readonly IPreferencesService _service;

        public ThinkingController(IPreferencesService service)
        {
            _service = service;
        }

        [HttpGet("indicadores")]
        public IActionResult GetIndicadores()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized("Usuário não autenticado");

            var resultado = _thinkingUtils.GerarSugestoes(int.Parse(usuarioId));

            return Ok(resultado);
        }

        [HttpGet("ler_preferencias")]
        public async Task<IActionResult> ReadPreferences()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
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

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Obtendo ID do usuário
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
