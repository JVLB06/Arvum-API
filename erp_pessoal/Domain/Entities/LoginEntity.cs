using Domain.Helpers;

namespace Domain.Entities
{
    public class LoginEntity
    {
        public string Email { get; private set; }
        public string Token { get; private set; }

        public LoginEntity(int? id, string email, string password)
        {
            if (password.Length < 6)
                throw new Exception("Senha fraca");

            Email = email.Trim().ToLower();

            Token = AuthHelper.GenerateJwt(id, email);

        }
    }
}
