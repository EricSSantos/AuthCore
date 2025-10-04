using AuthCore.Application.UseCases.AuthCase.SignIn;
using AuthCore.Application.UseCases.UserCase.Add;
using AuthCore.Domain.Interfaces.Adapters.Http;
using AuthCore.Domain.Interfaces.Adapters.Security.Cripto;
using AuthCore.Domain.Interfaces.Adapters.Security.Tokens;
using AuthCore.Domain.Interfaces.Adapters.Sessions;
using AuthCore.Domain.Interfaces.Repositories;
using AuthCore.Domain.Settings;
using AuthCore.Infrastructure.Adapters.Http;
using AuthCore.Infrastructure.Adapters.Security.Cripto;
using AuthCore.Infrastructure.Adapters.Security.Tokens;
using AuthCore.Infrastructure.Adapters.Sessions;
using AuthCore.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations
{
    public static class Injections
    {
        public static void AddInjections(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            services.AddSettings(builder.Configuration);
            services.AddUseCases();
            services.AddAdapters();
            services.AddRepositories();
            services.AddHttpContextAccessor();
        }

        #region Settings
        private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApiSettings>(config.GetSection("Api"));
            services.Configure<DatabaseSettings>(config.GetSection("Database"));
            services.Configure<SecuritySettings>(config.GetSection("Security"));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApiSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SecuritySettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RedisSettings>>().Value);

            return services;
        }
        #endregion

        #region UseCases
        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<ISigIn, SigIn>();
            services.AddScoped<IAddUser, AddUser>();

            return services;
        }
        #endregion

        #region Adapters
        private static IServiceCollection AddAdapters(this IServiceCollection services)
        {
            services.AddScoped<ICookieAdapter, CookieAdapter>();
            services.AddScoped<IDeviceAdapter, DeviceAdapter>();
            services.AddScoped<IBCryptAdapter, BCryptAdapter>();
            services.AddScoped<IRsaAdapter, RsaAdapter>();
            services.AddScoped<IAccessTokenAdapter, AccessTokenAdapter>();
            services.AddScoped<IRefreshTokenAdapter, RefreshTokenAdapter>();
            services.AddScoped<ISessionAdapter, SessionAdapter>();

            return services;
        }
        #endregion

        #region Repositories
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
        #endregion
    }
}
