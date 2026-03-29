using AuthCore.Api.HealthChecks;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>Configura health checks da aplicação.</summary>
    public static class HealthChecksExtensions
    {
        /// <summary>Operação para registrar health checks de infraestrutura.</summary>
        public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<PostgreSqlHealthCheck>("postgres")
                .AddCheck<RedisHealthCheck>("redis");
            return services;
        }
    }
}
