using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OldPhone.Infrastructure.Redis;
using OldPhone.Infrastructure.Repositories;
using StackExchange.Redis;

namespace OldPhone.Infrastructure
{
    /// <summary>
    /// Extension methods for configuring Infrastructure services
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Infrastructure services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            
            services.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));

            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var redisOptions = provider.GetRequiredService<IOptions<RedisOptions>>().Value;
                return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
            });

            services.AddScoped(typeof(IRepository<,>), typeof(RedisRepository<,>));

            return services;
        }
    }
} 