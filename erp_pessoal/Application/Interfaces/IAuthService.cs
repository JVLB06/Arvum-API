using Application.DTOs;
using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(UserDTO newUser);
        Task<LoginEntity> LoginAsync(LoginDTO login);
    }
}
