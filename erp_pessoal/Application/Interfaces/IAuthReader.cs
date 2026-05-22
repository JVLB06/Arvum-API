using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuthReader
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
    }
}
