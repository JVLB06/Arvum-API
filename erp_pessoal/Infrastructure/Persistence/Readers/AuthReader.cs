using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseMappers;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class AuthReader : IAuthReader
    {
        public async Task<IEnumerable<UserDTO>> GetUserByEmailAsync(string email)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    id AS Id, 
                    nome AS UserName, 
                    email AS Email,
                    senha AS PasswordHash
                FROM usuarios
                WHERE email = @Email";

            var user = await conn.QueryFirstOrDefaultAsync<UserBaseModel>(
                sql,
            new { Email = email });

            if (user is null)
                return null;

            return (IEnumerable<UserDTO>)UserMapper.ToInput(user);
        }
    }
}