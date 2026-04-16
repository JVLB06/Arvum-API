using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using erp_pessoal.Models;
using erp_pessoal.Controllers;
namespace erp_pessoal.Controllers
{
    [ApiController]
    [Route("thinking")]
    public class ThinkingController : ControllerBase
    {
        private readonly ThinkingUtils _thinkingUtils;

        [HttpGet("indicadores/{id}")]
        public IActionResult GetIndicadores([FromRoute] string id)
        {
            Tuple<EstruturaSugestaoModel, IndicadoresModel> indicadores = _thinkingUtils.GerarSugestoes(id);
            return Ok(indicadores);
        }

        [HttpPost("criar_preferencias")]
        public IActionResult CriarPreferencias([FromBody] PreferenciasUsuarioModel preferencias)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var preferencias_cmd = new NpgsqlCommand(@"INSERT INTO RESTRICOES_USUARIO (
                USER_ID, PREFERENCIA_ID, GASTO_ID, EXCLUIR, REDUZIR, BLOQUEADO, ATIVO)
                VALUES (@user, @preferencia, @gasto, @excluir, @reduzir, @block, TRUE)", conn);

            preferencias_cmd.Parameters.AddWithValue("@user", preferencias.IdUsuario);
            preferencias_cmd.Parameters.AddWithValue("@preferencia", preferencias.IdPreferencia);
            preferencias_cmd.Parameters.AddWithValue("@gasto", preferencias.IdGasto);
            preferencias_cmd.Parameters.AddWithValue("@excluir", preferencias.Excluir);
            preferencias_cmd.Parameters.AddWithValue("@reduzir", preferencias.Reduzir);
            preferencias_cmd.Parameters.AddWithValue("@block", preferencias.Bloqueado);

            preferencias_cmd.ExecuteNonQuery();

            return Ok("Preferencias incluídas com sucesso");
        }

        [HttpPut("atualizar_preferencias")]
        public IActionResult AtualizarPreferencias([FromBody] PreferenciasUsuarioModel preferencias)
        {
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();

            var preferencias_cmd = new NpgsqlCommand(@"UPDATE RESTRICOES_USUARIO SET
                EXCLUIR = @excluir, REDUZIR = @reduzir, 
                BLOQUEADO = @block WHERE USER_ID = @user 
                AND PREFERENCIA_ID = @preferencia 
                AND GASTO_ID = @gasto ", conn);

            preferencias_cmd.Parameters.AddWithValue("@user", preferencias.IdUsuario);
            preferencias_cmd.Parameters.AddWithValue("@preferencia", preferencias.IdPreferencia);
            preferencias_cmd.Parameters.AddWithValue("@gasto", preferencias.IdGasto);
            preferencias_cmd.Parameters.AddWithValue("@excluir", preferencias.Excluir);
            preferencias_cmd.Parameters.AddWithValue("@reduzir", preferencias.Reduzir);
            preferencias_cmd.Parameters.AddWithValue("@block", preferencias.Bloqueado);

            preferencias_cmd.ExecuteNonQuery();

            return Ok("Preferencias atualizadas com sucesso");
        }
        [HttpGet("ler_preferencias/{id}")]
        public IActionResult LerPreferencias([FromRoute] string id)
        {
            List<PreferenciasUsuarioModel> preferenciasList = new List<PreferenciasUsuarioModel>();
            using var conn = new NpgsqlConnection(Essentials._connectionString);
            conn.Open();
            var preferencias_cmd = new NpgsqlCommand(@"SELECT USER_ID, PREFERENCIA_ID, GASTO_ID, EXCLUIR, REDUZIR, BLOQUEADO 
                FROM RESTRICOES_USUARIO WHERE USER_ID = @user AND ATIVO = TRUE", conn);
            preferencias_cmd.Parameters.AddWithValue("@user", id);
            var reader = preferencias_cmd.ExecuteReader();
            while (reader.Read())
            {
                PreferenciasUsuarioModel preferencias = new PreferenciasUsuarioModel
                {
                    IdUsuario = reader.GetString(0),
                    IdPreferencia = reader.GetString(1),
                    IdGasto = reader.GetString(2),
                    Excluir = reader.GetBoolean(3),
                    Reduzir = reader.GetBoolean(4),
                    Bloqueado = reader.GetBoolean(5)
                };
                preferenciasList.Add(preferencias);
            }
            return Ok(preferenciasList);
        }
    }
}
