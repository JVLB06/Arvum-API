using Domain.Helpers;
using System;

namespace Domain.Entities
{
    public class LoginEntity
    {
        public string Email { get; private set; }
        public string Token { get; private set; }

        public LoginEntity(int? id, string email, string passwordReceived, string passwordRegistered)
        {
            if (BCrypt.Net.BCrypt.Verify(passwordReceived, passwordRegistered))
            {
                Email = email.Trim().ToLower();

                Token = AuthHelper.GenerateJwt(id, email);
            }
            else
            {
                throw new Exception("Email ou senha inválidos");
            }
        }
    }
}
