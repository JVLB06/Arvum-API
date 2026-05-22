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
        public IActionResult AccessAccount([FromBody] LoginModel login)
        {

            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT id, senha, nome, email FROM usuarios WHERE email = @login AND ativo = TRUE", conn);
            cmd.Parameters.AddWithValue("@login", logIn.username);

            var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return Unauthorized(new { message = "Credenciais inválidas" });

            UsuarioModel user = new UsuarioModel
            {
                Id = reader.GetInt32(0),
                Password = reader.GetString(1),
                Username = reader.GetString(2),
                Email = reader.GetString(3)
            };

            reader.Close();

            if (!BCrypt.Net.BCrypt.Verify(logIn.password, user.Password))
                return Unauthorized(new { message = "Credenciais inválidas" });

            var token = Essentials.GerarJwt(user.Id, user.Username);
            return Ok(new { access_token = token, token_type = "bearer" });
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