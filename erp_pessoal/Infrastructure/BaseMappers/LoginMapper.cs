using Application.DTOs;
using Infrastructure.BaseModels;

namespace Infrastructure.BaseMappers
{
    public static class LoginMapper
    {
        public static LoginDTO ToInput(LoginBaseModel model)
        {
            if (model == null) return null;

            return new LoginDTO
            {
                Id = model.Id,
                Email = model.Username,
                Password = model.Password
            };
        }
    }
}
