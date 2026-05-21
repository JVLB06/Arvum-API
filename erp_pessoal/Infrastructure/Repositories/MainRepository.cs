using System.Data;
using Npgsql;

namespace Infrastructure.Repositories
{
    public static class MainRepository
    {
        // Nota: No futuro, essa string sairá do appsettings.json, mas manteremos aqui por enquanto.
        private static readonly string _connectionString = "Host=localhost;Port=5432;Database=erp;Username=postgres;Password=2006;SSL Mode=Disable;";

        /// <summary>
        /// Cria e retorna uma nova conexão com o PostgreSQL pronta para uso.
        /// </summary>
        public static IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }
    }
}