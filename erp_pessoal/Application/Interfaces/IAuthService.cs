using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(UserDTO newUser);
        Task<LoginEntity> LoginAsync(LoginDTO login);
        ConnectionEntity ValidateConnection(ConnectionDTO connection);
    }
}
