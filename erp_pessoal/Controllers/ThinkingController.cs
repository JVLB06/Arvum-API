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

            var resultado = _thinkingUtils.GerarSugestoes(int.Parse(usuarioId));

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

            cmd.Parameters.AddWithValue("@user", int.Parse(usuarioId));

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
            [FromBody] NovaPreferenciaModel preferencias)
        {

            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized("Usuário não autenticado");

            List<PreferenciasUsuarioModel> preferenciasList = new();

            using var conn = new NpgsqlConnection(Essentials._connectionString);

            conn.Open();
            
            var readCmd = new NpgsqlCommand(@"
                SELECT
                    USER_ID,
                    ID_PREF,
                    GASTO_ID,
                    EXCLUIR,
                    REDUZIR,
                    BLOQUEADO
                FROM RESTRICOES_USUARIO
                WHERE USER_ID = @user
                AND GASTO_ID = @gasto
                LIMIT 1
            ", conn);

            var insertCmd = new NpgsqlCommand(@"
                INSERT INTO RESTRICOES_USUARIO
                (
                    USER_ID,
                    GASTO_ID,
                    EXCLUIR,
                    REDUZIR,
                    ATIVO
                )
                VALUES
                (
                    @user,
                    @gasto,
                    @excluir,
                    @reduzir,
                    TRUE
                )
            ", conn);

            var updateCmd = new NpgsqlCommand(@"
                UPDATE RESTRICOES_USUARIO
                SET
                    EXCLUIR = @excluir,
                    REDUZIR = @reduzir,
                    ATIVO = TRUE
                WHERE
                    USER_ID = @user
                    AND PREFERENCIA_ID = @preferencia
                    AND GASTO_ID = @gasto
            ", conn);

            readCmd.Parameters.AddWithValue("@user", int.Parse(usuarioId));
            readCmd.Parameters.AddWithValue("@gasto", int.Parse(preferencias.IdGasto));

            PreferenciasUsuarioModel? preferenciaExistente = null;

            using var reader = readCmd.ExecuteReader();

            if (reader.Read())
            {
                preferenciaExistente = new PreferenciasUsuarioModel
                {
                    IdUsuario = reader["USER_ID"].ToString(),
                    IdPreferencia = reader["PREFERENCIA_ID"].ToString(),
                    IdGasto = reader["GASTO_ID"].ToString(),
                    Excluir = Convert.ToBoolean(reader["EXCLUIR"]),
                    Reduzir = Convert.ToBoolean(reader["REDUZIR"]),
                    Bloqueado = Convert.ToBoolean(reader["BLOQUEADO"])
                };
            }

            if (preferenciaExistente == null)
            {
                insertCmd.Parameters.AddWithValue("@user", int.Parse(usuarioId));
                insertCmd.Parameters.AddWithValue("@gasto", int.Parse(preferencias.IdGasto));
                insertCmd.Parameters.AddWithValue("@excluir", preferencias.Excluir ? true : false); //Se verdadeiro exclusão bloqueada
                insertCmd.Parameters.AddWithValue("@reduzir", preferencias.Excluir ? false : true); //Se falso redução bloqueada
                insertCmd.ExecuteNonQuery();
            }
            else
            {
                updateCmd.Parameters.AddWithValue("@user", int.Parse(usuarioId));
                updateCmd.Parameters.AddWithValue("@preferencia", preferenciaExistente.IdPreferencia);
                updateCmd.Parameters.AddWithValue("@gasto", int.Parse(preferencias.IdGasto));
                updateCmd.Parameters.AddWithValue("@excluir", preferencias.Excluir ? true : false); //Se verdadeiro exclusão bloqueada
                updateCmd.Parameters.AddWithValue("@reduzir", preferencias.Excluir ? false : true); //Se falso redução bloqueada
                updateCmd.ExecuteNonQuery();
            }

            return Ok(new
            {
                mensagem = "Preferência adicionada com sucesso"
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
