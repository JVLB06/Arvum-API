using System.Data;
using Npgsql;

namespace Infrastructure.Repositories
{
    public static class MainRepository
    {
        private static readonly string _connectionString =
            Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection"
            ) ?? throw new InvalidOperationException(
                "Connection string não configurada."
            );

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
