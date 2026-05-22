using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Presentation.WebModels;
using Application.Interfaces;
using Presentation.InputMappers;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("contas")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> CreateAccount([FromBody] NewUserModel newUser)
        {

            try
            {
                await _service.RegisterAsync(NewUserMapper.ToDTO(newUser));

                return Ok(new { message = "Usuário cadastrado com sucesso" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }   
        }

        [HttpPost("login")]
        public async IActionResult AccessAccount([FromBody] LoginModel login)
        {
            try
            {
                return Ok(await _service.LoginAsync(LoginMapper.ToDTO(login)));
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }
        //        [HttpPost("recuperar-senha")]

        // Rota de verificação de conexão
        [HttpGet("verificar-conexao")]
        [Authorize]
        public IActionResult VerifyConection()
        {
            // Recupera o ID e nome do usuário do token
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nome = User.Identity?.Name;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(new { message = "Usuário não autenticado" });
            }
            return Ok(new
            {
                autenticado = true,
                usuario_id = usuarioId,
                user = nome
            });
        };
    };
}