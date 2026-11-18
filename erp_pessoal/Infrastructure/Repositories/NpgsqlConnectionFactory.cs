
using Application.Interfaces;
using Npgsql;
using System.Data;

namespace Infrastructure.Repositories;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IDbConnection CreateConnection()
    {
        return _dataSource.OpenConnection();
    }
}
