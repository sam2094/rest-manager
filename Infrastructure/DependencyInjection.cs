using Application.Interfaces;
using Application.Interfaces.Common;
using Domain.Models;
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
            var tableConfig = configuration.GetSection("RestaurantConfig:Tables").Get<Dictionary<int, int>>();

            var tables = tableConfig.SelectMany(kv =>
                Enumerable.Range(0, kv.Value).Select(_ => new Table { Size = kv.Key })
            ).ToList();

            services.AddSingleton<IRestManager>(_ => new RestManager(tables));
            return services;
        }
    }
}