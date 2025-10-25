using AuthCore.Application;
using AuthCore.Domain.Commons.Settings;
using AuthCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations.Extensions
{
    public static class DependencyInjections
    {
        /// <summary>
        /// Registra os módulos principais da aplicação, incluindo camadas de domínio,
        /// aplicação e infraestrutura, além das configurações de contexto e serviços compartilhados.
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> utilizada para configurar os serviços.</param>
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
        /// Registra as configurações fortemente tipadas (Settings) no container de injeção de dependência.
        /// Cada seção do arquivo <c>appsettings.json</c> é vinculada à respectiva classe de configuração.
        /// </summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        /// <param name="config">Instância de <see cref="IConfiguration"/> para acesso às seções de configuração.</param>
        /// <returns>A própria instância de <see cref="IServiceCollection"/> para encadeamento de chamadas.</returns>
        private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApiSettings>(config.GetSection("Api"));
            services.Configure<DatabaseSettings>(config.GetSection("Database"));
            services.Configure<CorsSettings>(config.GetSection("Cors"));
            services.Configure<SecuritySettings>(config.GetSection("Security"));
            services.Configure<RedisSettings>(config.GetSection("Redis"));
            services.Configure<RabbitMqSettings>(config.GetSection("RabbitMQ"));
            

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApiSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<CorsSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SecuritySettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RedisSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);
            
            return services;
        }

        #endregion
    }
}
