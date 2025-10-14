using AuthCore.Application.Services;
using AuthCore.Application.Services.Interfaces;
using AuthCore.Application.UseCases.AuthCase;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Application.UseCases.SessionCase;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Application.UseCases.UserCase;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Interfaces.Helpers;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Repositories;
using AuthCore.Domain.Commons.Interfaces.Security;
using AuthCore.Domain.Commons.Settings;
using AuthCore.Infrastructure.Helpers;
using AuthCore.Infrastructure.Http;
using AuthCore.Infrastructure.Messaging.RabbitMq;
using AuthCore.Infrastructure.Persistence.Database.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Repositories;
using AuthCore.Infrastructure.Security.Cryptography;
using AuthCore.Infrastructure.Security.Tokens;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations
{
    public static class Injections
    {
        public static void AddInjections(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddSettings(builder.Configuration);
            services.AddApplication();
            services.AddInfrastructure();
            services.AddRepositories();

            services.AddHttpContextAccessor();
        }

        #region Settings
        private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApiSettings>(config.GetSection("Api"));
            services.Configure<DatabaseSettings>(config.GetSection("Database"));
            services.Configure<SecuritySettings>(config.GetSection("Security"));
            services.Configure<RedisSettings>(config.GetSection("Redis"));
            services.Configure<RabbitMqSettings>(config.GetSection("RabbitMQ"));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApiSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SecuritySettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RedisSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);

            return services;
        }
        #endregion

        #region Application
        private static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOwnership, Ownership>();
            services.AddScoped<ISignIn, SignIn>();
            services.AddScoped<ISignOut, SignOut>();
            services.AddScoped<IRefresh, Refresh>();
            services.AddScoped<IGetSessions, GetSessions>();
            services.AddScoped<IRevokeSession, RevokeSessions>();
            services.AddScoped<IAddUser, AddUser>();
            services.AddScoped<IGetCurrentUser, GetCurrentUser>();

            return services;
        }
        #endregion

        #region Infrastructure
        private static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICookie, CookieService>();
            services.AddScoped<IDevice, DeviceService>();
            services.AddScoped<IBCrypt, BCryptService>();
            services.AddScoped<IHmac, HmacService>();
            services.AddScoped<IEcdsaSigner, EcdsaService>();
            services.AddScoped<IEcdsaProvider, EcdsaService>();
            services.AddScoped<IEntropy, EntropyService>();
            services.AddScoped<IAccessToken, JwtService>();
            services.AddScoped<IJsonSerializer, JsonSerializer>();
            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }
        #endregion

        #region Repositories
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();

            return services;
        }
        #endregion
    }
}
