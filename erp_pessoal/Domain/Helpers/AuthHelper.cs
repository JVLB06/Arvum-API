using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Domain.Helpers
{
    public class AuthHelper
    {
        public static readonly string _jwtSecret =
            Environment.GetEnvironmentVariable("Jwt__Secret")
            ?? throw new InvalidOperationException("Jwt__Secret não configurada.");
        
        // Função auxiliar para gerar o JWT
        public static string GenerateJwt(int? idUsuario, string nomeUsuario)
        {
            if (idUsuario is null)
            {
                throw new Exception("Usuário não encontrado");
            }
            
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
