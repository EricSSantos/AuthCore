using AuthCore.Application;
using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>
    /// Configura a injeção de dependências da aplicação.
    /// </summary>
    public static class DependencyInjections
    {
        /// <summary>
        /// Registra módulos e serviços principais.
        /// </summary>
        /// <param name="builder">Instância para configurar serviços.</param>
        public static void AddDependencyInjections(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            services.AddSettings(builder.Configuration);
            services.AddApplication();
            services.AddInfrastructure();
            services.AddHttpContextAccessor();
        }

        #region Settings

        /// <summary>
        /// Registra configurações tipadas da aplicação.
        /// </summary>
        /// <param name="services">Coleção de serviços.</param>
        /// <param name="config">Configurações do appsettings.</param>
        /// <returns>Instância atualizada de serviços.</returns>
        private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApiSettings>(config.GetSection("Api"));
            services.Configure<DatabaseSettings>(config.GetSection("Database"));
            services.Configure<CorsSettings>(config.GetSection("Cors"));
            services.Configure<SecuritySettings>(config.GetSection("Security"));
            services.Configure<RabbitMqSettings>(config.GetSection("RabbitMQ"));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApiSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<CorsSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SecuritySettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);

            return services;
        }

        #endregion
    }
}
