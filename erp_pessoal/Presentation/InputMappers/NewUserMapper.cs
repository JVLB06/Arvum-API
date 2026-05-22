using Application.DTOs;
using Presentation.WebModels;

namespace Presentation.InputMappers
{
    public class NewUserMapper
    {
        public static UserDTO ToDTO(NewUserModel model)
        {
            return new UserDTO
            {
                UserName = model.UserName,
                Email = model.Email,
                BornDate = model.BornDate,
                PasswordHash = model.Password
            };
        }
    }
}
