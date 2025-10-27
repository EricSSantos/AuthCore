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
        /// Registra os módulos principais da aplicação e suas dependências.
        /// </summary>
        /// <param name="builder">Instância usada para configurar os serviços da aplicação.</param>
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
        /// Registra as configurações tipadas da aplicação.
        /// </summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        /// <param name="config">Instância de configuração usada para mapear as seções do appsettings.</param>
        /// <returns>O próprio <see cref="IServiceCollection"/> para encadeamento.</returns>
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
