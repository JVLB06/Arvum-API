using Application.Interfaces;
using Infrastructure.Persistence.Readers;
using Infrastructure.Persistence.Writers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthReader, AuthReader>();
            services.AddScoped<IAuthWriter, AuthWriter>();
            services.AddScoped<IGeneralRegistersReader, GeneralRegistersReader>();

            return services;
        }
    }
}
