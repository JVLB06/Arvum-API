using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Writers
{
    public class AuthWriter : IAuthWriter
    {
        public async Task CreateUserAsync(AuthEntity user)
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                INSERT INTO usuarios
                (
                    nome,
                    senha,
                    nascimento,
                    email,
                    tipo,
                    ativo
                )
                VALUES
                (
                    @UserName,
                    @PasswordHash,
                    @BornDate,
                    @Email,
                    'user',
                    TRUE
                )";

            await conn.ExecuteAsync(sql, new
            {
                user.UserName,
                user.PasswordHash,
                user.BornDate,
                user.Email
            });
        }
    }
}
