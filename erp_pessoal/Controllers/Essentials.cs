using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace erp_pessoal.Controllers
{

    public static class Essentials
    {
        public static readonly string _connectionString = "Host=192.168.0.200;Port=5433;Database=erp;Username=postgres;Password=2006;";
        public static readonly string _jwtSecret = "DFr9@27!KmLp38_ZxYQwErTyUiOp12345"; // mesma do Program.cs

        // Função auxiliar para gerar o JWT
        public static string GerarJwt(int idUsuario, string nomeUsuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                    new Claim(ClaimTypes.Name, nomeUsuario)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}