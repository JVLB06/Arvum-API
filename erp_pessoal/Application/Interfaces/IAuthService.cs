using Application.DTOs;
using Application.Models;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(UserDTO newUser);
    }
}
