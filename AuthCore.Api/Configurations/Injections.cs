using AuthCore.Application.Services;
using AuthCore.Application.Services.Interfaces;
using AuthCore.Application.UseCases.AuthCase;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Application.UseCases.SessionCase;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Application.UseCases.UserCase;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Messaging;
using AuthCore.Domain.Commons.Interfaces.Persistence;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;
using AuthCore.Domain.Commons.Interfaces.Security.Signing;
using AuthCore.Domain.Commons.Settings;
using AuthCore.Infrastructure.Http;
using AuthCore.Infrastructure.Messaging.RabbitMq;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Repositories;
using AuthCore.Infrastructure.Security.Hashing;
using AuthCore.Infrastructure.Security.Jwt;
using AuthCore.Infrastructure.Security.Signing;
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
            // Auth
            services.AddScoped<IOwnership, Ownership>();
            services.AddScoped<ISignIn, SignIn>();
            services.AddScoped<ISignOut, SignOut>();
            services.AddScoped<IRefresh, Refresh>();
            // Session
            services.AddScoped<IGetSessions, GetSessions>();
            services.AddScoped<IRevokeSession, RevokeSessions>();
            // User
            services.AddScoped<IAddUser, AddUser>();
            services.AddScoped<IGetCurrentUser, GetCurrentUser>();
            services.AddScoped<IForgotPassword, ForgotPassword>();
            services.AddScoped<IResetPassword, ResetPassword>();
            services.AddScoped<IChangePassword, ChangePassword>();

            return services;
        }
        #endregion

        #region Infrastructure
        private static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Http
            services.AddScoped<ICookie, CookieService>();
            services.AddScoped<IDevice, DeviceService>();
            // Security
            services.AddScoped<IBCrypt, BCryptService>();
            services.AddScoped<IHmac, HmacService>();
            services.AddScoped<IEcdsaSigner, EcdsaService>();
            services.AddScoped<IEcdsaProvider, EcdsaService>();
            services.AddScoped<IEntropy, EntropyService>();
            services.AddScoped<IAccessToken, JwtService>();
            // Messaging
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }
        #endregion

        #region Repositories
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // PostgreSQL Repositories
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            // Redis Repositories
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IConfirmCodeRepository, ConfirmCodeRepository>();

            return services;
        }
        #endregion
    }
}
