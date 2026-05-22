using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuthEntity
    {
        public string UserName { get; private set; }
        public DateTime? BornDate { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public AuthEntity(string userName,string email,string password, DateTime? bornDate)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Exception("Usuário inválido");

            if (!email.Contains("@"))
                throw new Exception("Email inválido");

            if (password.Length < 6)
                throw new Exception("Senha fraca");

            UserName = userName.Trim();

            Email = email.Trim().ToLower();

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                
            BornDate = bornDate;
        }
    }
}
