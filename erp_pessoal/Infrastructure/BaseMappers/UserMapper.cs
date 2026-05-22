using Infrastructure.BaseModels;
using Application.DTOs;

namespace Infrastructure.BaseMappers
{
    public static class UserMapper
    {
        public static UserDTO ToInput(UserBaseModel user)
        {
            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Password = user.PasswordHash
            };
        }
    }
}
