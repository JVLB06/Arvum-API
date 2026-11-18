using Application.Interfaces;
using Infrastructure.Persistence.Readers;
using Infrastructure.Persistence.Writers;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthReader, AuthReader>();
            services.AddScoped<IAuthWriter, AuthWriter>();
            services.AddScoped<IGeneralReceiptsReader, GeneralReceiptsReader>();
            services.AddScoped<IGeneralReceiptsWriter, GeneralReceiptsWriter>();
            services.AddScoped<IGeneralInvestmentsReader, GeneralInvestmentsReader>();
            services.AddScoped<IGeneralInvestmentsWriter, GeneralInvestmentsWriter>();
            services.AddScoped<IGeneralDebtsReader, GeneralDebtsReader>();
            services.AddScoped<IGeneralDebtsWriter, GeneralDebtsWriter>();
            services.AddScoped<IGeneralGoalsReader, GeneralGoalsReader>();
            services.AddScoped<IGeneralGoalsWriter, GeneralGoalsWriter>();
            services.AddScoped<IGeneralExpensesReader, GeneralExpensesReader>();
            services.AddScoped<IGeneralExpensesWriter, GeneralExpensesWriter>();
            services.AddScoped<ISpecificRegistersReader, SpecificRegistersReader>();
            services.AddScoped<ISpecificRegistersWriter, SpecificRegistersWriter>();
            services.AddScoped<IThinkingReader, ThinkingReader>();
            services.AddScoped<IThinkingWriter, ThinkingWriter>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not configured"
                );
            }

            services.AddSingleton(
                NpgsqlDataSource.Create(connectionString)
            );

            services.AddScoped<
                IDbConnectionFactory,
                NpgsqlConnectionFactory
            >();

            return services;
        }
    }
}
