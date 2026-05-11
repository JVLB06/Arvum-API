using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Npgsql;
using erp_pessoal.Models;

namespace erp_pessoal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("thinking")]
    public class ThinkingController : ControllerBase
    {
        private readonly ThinkingUtils _thinkingUtils = new ThinkingUtils();

        [HttpGet("indicadores")]
        public IActionResult GetIndicadores()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized("Usuário não autenticado");

            var resultado = _thinkingUtils.GerarSugestoes(usuarioId);

            return Ok(resultado);
        }

        [HttpGet("ler_preferencias")]
        public IActionResult LerPreferencias()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized("Usuário não autenticado");

            List<PreferenciasUsuarioModel> preferenciasList = new();

            using var conn = new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var cmd = new NpgsqlCommand(@"
                SELECT 
                    USER_ID,
                    ID_PREF,
                    GASTO_ID,
                    EXCLUIR,
                    REDUZIR,
                    BLOQUEADO
                FROM RESTRICOES_USUARIO
                WHERE USER_ID = @user
                AND ATIVO = TRUE
            ", conn);

            cmd.Parameters.AddWithValue("@user", usuarioId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                preferenciasList.Add(new PreferenciasUsuarioModel
                {
                    IdUsuario = reader.GetString(0),
                    IdPreferencia = reader.GetString(1),
                    IdGasto = reader.GetString(2),
                    Excluir = reader.GetBoolean(3),
                    Reduzir = reader.GetBoolean(4),
                    Bloqueado = reader.GetBoolean(5)
                });
            }

            return Ok(preferenciasList);
        }

        [HttpPost("criar_preferencias")]
        public IActionResult CriarPreferencias(
            [FromBody] PreferenciasUsuarioModel preferencias)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var cmd = new NpgsqlCommand(@"
                INSERT INTO RESTRICOES_USUARIO
                (
                    USER_ID,
                    PREFERENCIA_ID,
                    GASTO_ID,
                    EXCLUIR,
                    REDUZIR,
                    BLOQUEADO,
                    ATIVO
                )
                VALUES
                (
                    @user,
                    @preferencia,
                    @gasto,
                    @excluir,
                    @reduzir,
                    @bloqueado,
                    TRUE
                )
            ", conn);

            cmd.Parameters.AddWithValue("@user", preferencias.IdUsuario);
            cmd.Parameters.AddWithValue("@preferencia", preferencias.IdPreferencia);
            cmd.Parameters.AddWithValue("@gasto", preferencias.IdGasto);
            cmd.Parameters.AddWithValue("@excluir", preferencias.Excluir);
            cmd.Parameters.AddWithValue("@reduzir", preferencias.Reduzir);
            cmd.Parameters.AddWithValue("@bloqueado", preferencias.Bloqueado);

            cmd.ExecuteNonQuery();

            return Ok(new
            {
                mensagem = "Preferência criada com sucesso"
            });
        }

        [HttpPut("atualizar_preferencias")]
        public IActionResult AtualizarPreferencias(
            [FromBody] PreferenciasUsuarioModel preferencias)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var cmd = new NpgsqlCommand(@"
                UPDATE RESTRICOES_USUARIO
                SET
                    EXCLUIR = @excluir,
                    REDUZIR = @reduzir,
                    BLOQUEADO = @bloqueado
                WHERE
                    USER_ID = @user
                    AND PREFERENCIA_ID = @preferencia
                    AND GASTO_ID = @gasto
            ", conn);

            cmd.Parameters.AddWithValue("@user", preferencias.IdUsuario);
            cmd.Parameters.AddWithValue("@preferencia", preferencias.IdPreferencia);
            cmd.Parameters.AddWithValue("@gasto", preferencias.IdGasto);
            cmd.Parameters.AddWithValue("@excluir", preferencias.Excluir);
            cmd.Parameters.AddWithValue("@reduzir", preferencias.Reduzir);
            cmd.Parameters.AddWithValue("@bloqueado", preferencias.Bloqueado);

            cmd.ExecuteNonQuery();

            return Ok(new
            {
                mensagem = "Preferência atualizada com sucesso"
            });
        }

        [HttpDelete("deletar_preferencia")]
        public IActionResult DeletarPreferencia(
            [FromQuery] string gastoId)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized();

            using var conn = new NpgsqlConnection(Essentials._connectionString);

            conn.Open();

            var cmd = new NpgsqlCommand(@"
                UPDATE RESTRICOES_USUARIO
                SET ATIVO = FALSE
                WHERE USER_ID = @user
                AND GASTO_ID = @gasto
            ", conn);

            cmd.Parameters.AddWithValue("@user", usuarioId);
            cmd.Parameters.AddWithValue("@gasto", gastoId);

            cmd.ExecuteNonQuery();

            return Ok(new
            {
                mensagem = "Preferência removida com sucesso"
            });
        }
    }
}