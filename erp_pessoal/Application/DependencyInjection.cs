using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGeneralReceiptsService, GeneralReceiptsService>();
            services.AddScoped<IGeneralInvestmentsService, GeneralInvestmentsService>();
            services.AddScoped<IThinkingService, ThinkingService>();
            services.AddScoped<IGeneralDebtsService, GeneralDebtsService>();
            services.AddScoped<IGeneralGoalsService, GeneralGoalsService>();
            services.AddScoped<IGeneralExpensesService, GeneralExpensesService>();
            services.AddScoped<ISpecificRegistersService, SpecificRegistersService>();

            return services;
        }
    }
}
