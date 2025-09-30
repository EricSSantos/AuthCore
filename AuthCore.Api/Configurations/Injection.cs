using AuthCore.Domain.Settings;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations
{
    public static class Injection
    {
        public static void AddInjections(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddSettings(builder.Configuration);
            services.AddServices();
            services.AddRepositories();
            services.AddHttpContextAccessor();
        }

        private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApiSettings>(config.GetSection("Api"));
            services.Configure<AuthSettings>(config.GetSection("Auth"));
            services.Configure<DatabaseSettings>(config.GetSection("Database"));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApiSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AuthSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services;
        }
    }
}
