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

            return services;
        }
    }
}
