using Application.Models;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<IEnumerable<UserModel>> GetUsersAsync();
    }
}
