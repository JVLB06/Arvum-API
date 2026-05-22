using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class LoginMapper
    {
        public static LoginDTO ToDTO(LoginModel model)
        {
            return new LoginDTO
            {
                Email = model.Username,
                Password = model.Password
            };
        }
    }
}
