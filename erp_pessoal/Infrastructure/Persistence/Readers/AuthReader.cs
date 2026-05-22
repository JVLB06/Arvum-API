using Application.DTOs;
using Infrastructure.BaseMappers;
using Application.Interfaces;
using Dapper;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Readers
{
    public class AuthReader : IAuthReader
    {
        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    id AS Id, 
                    nome AS UserName, 
                    email AS Email,
                    senha AS PasswordHash
                FROM usuarios";


            //Nome da coluna no Select deve ser igual ao nome da Model (mesma ordem também)
            var users = await conn.QueryAsync<UserBaseModel>(sql);

            return users.Select(UserMapper.ToInput);
        }
    }
}
