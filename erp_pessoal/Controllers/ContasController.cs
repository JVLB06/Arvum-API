using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using erp_pessoal.Models;
namespace erp_pessoal.Controllers
{
    [ApiController]
    [Route("contas")]
    public class ContasController : ControllerBase
    {

        [HttpPost("cadastro")]
        public IActionResult CriarConta([FromBody] SignUpModel signIn)
        {

            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var cmdCheck = new NpgsqlCommand("SELECT id FROM usuarios WHERE email = @nome", conn);
            cmdCheck.Parameters.AddWithValue("@nome", signIn.email);

            var reader = cmdCheck.ExecuteReader();
            if (reader.HasRows)
                return BadRequest(new { message = "Usuário já existe" });

            reader.Close();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(signIn.password);

            var cmdInsert = new NpgsqlCommand("INSERT INTO usuarios (nome, senha, nascimento, email, tipo, ativo) VALUES (@nome, @senha, @nasce, @email, 'user', TRUE)", conn);
            cmdInsert.Parameters.AddWithValue("@nome", signIn.username);
            cmdInsert.Parameters.AddWithValue("@senha", hashedPassword);
            cmdInsert.Parameters.AddWithValue("@nasce", signIn.nasce);
            cmdInsert.Parameters.AddWithValue("@email", signIn.email);

            cmdInsert.ExecuteNonQuery();
            return Ok(new { message = "Usuário cadastrado com sucesso" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] SignInModel logIn)
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
        public IActionResult VerificarConexao()
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
        }
    }
}
