using Application.Interfaces;
using Infrastructure.Persistence.Readers;
using Infrastructure.Persistence.Writers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
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

            return services;
        }
    }
}
