using Application.DTOs;
using Infrastructure.BaseModels;
using System.Reflection;

namespace Infrastructure.BaseMappers
{
    public static class UserMapper
    {

        public static UserDTO ToInput(UserBaseModel user)
        {
            if (user == null) return null;

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
