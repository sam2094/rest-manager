using Application.Interfaces;
using Application.Interfaces.Common;
using Infrastructure.Services;
using Infrastructure.Services.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureIoC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ILogger, Logger>();
            services.AddSingleton<IRestManager, RestManager>();
            return services;
        }
    }
}