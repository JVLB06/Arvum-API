using Application.DTOs;
using Dapper;
using Infrastructure.BaseModels;
using Infrastructure.BaseModels;
using Infrastructure.Repositories;
using Npgsql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Readers
{
    internal class AuthReader
    {
        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            using var conn = MainRepository.CreateConnection();

            const string sql = @"
                SELECT 
                    id AS Id, 
                    nome AS Nome, 
                    email AS Email 
                FROM usuarios";


            //Nome da coluna no Select deve ser igual ao nome da Model (mesma ordem também)
            var usuarios = await conn.QueryAsync<UserModel>(sql);

            return usuarios;
        }
    }
}
