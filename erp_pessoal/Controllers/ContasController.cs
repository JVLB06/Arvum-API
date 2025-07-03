using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace erp_pessoal.Controllers
{
    [ApiController]
    [Route("contas")]
    public class ContasController : ControllerBase
    {

        [HttpPost("cadastro")]
        public IActionResult CriarConta([FromBody] Dictionary<string, string> signIn)
        {
            string username = signIn["username"];
            string password = signIn["password"];
            string nasceStr = signIn["nasce"];
            DateTime nasce = DateTime.Parse(nasceStr);
            string email = signIn["email"];

            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var cmdCheck = new NpgsqlCommand("SELECT id FROM usuarios WHERE email = @nome", conn);
            cmdCheck.Parameters.AddWithValue("@nome", email);

            var reader = cmdCheck.ExecuteReader();
            if (reader.HasRows)
                return BadRequest(new { message = "Usuário já existe" });

            reader.Close();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var cmdInsert = new NpgsqlCommand("INSERT INTO usuarios (nome, senha, nascimento, email, tipo, ativo) VALUES (@nome, @senha, @nasce, @email, 'user', TRUE)", conn);
            cmdInsert.Parameters.AddWithValue("@nome", username);
            cmdInsert.Parameters.AddWithValue("@senha", hashedPassword);
            cmdInsert.Parameters.AddWithValue("@nasce", nasce);
            cmdInsert.Parameters.AddWithValue("@email", email);

            cmdInsert.ExecuteNonQuery();
            return Ok(new { message = "Usuário cadastrado com sucesso" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Dictionary<string, string> logIn)
        {
            string username = logIn["username"];
            string password = logIn["password"];

            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT id, senha, nome, email FROM usuarios WHERE email = @login AND ativo = TRUE", conn);
            cmd.Parameters.AddWithValue("@login", username);

            var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return Unauthorized(new { message = "Credenciais inválidas" });

            int id = reader.GetInt32(0);
            string senhaHash = reader.GetString(1);
            string nome = reader.GetString(2);

            reader.Close();

            if (!BCrypt.Net.BCrypt.Verify(password, senhaHash))
                return Unauthorized(new { message = "Credenciais inválidas" });

            var token = Essentials.GerarJwt(id, nome);
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
