using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthWriter
    {
        Task CreateUserAsync(UserEntity user);
    }
}
