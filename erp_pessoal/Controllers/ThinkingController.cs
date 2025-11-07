using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using erp_pessoal.Models;
namespace erp_pessoal.Controllers
{
    [ApiController]
    [Route("thinking")]
    public class ThinkingController : ControllerBase
    {
        [HttpGet("indicadores{id}")]
        //Obter todos os indicadores
        public IActionResult GetIndicadores([FromQuery] string id)
        {
            var indicadores = new
            {
                produtividade = 85,
                eficiencia = 90,
                satisfacao = 75
            };
            return Ok(indicadores);
        }
        //Incluir preferencias do usuário (impede que certas sugestões sejam feitas)
        //Atualizar preferencias do usuário
        //Visualizar preferencias do usuário
    }
}
