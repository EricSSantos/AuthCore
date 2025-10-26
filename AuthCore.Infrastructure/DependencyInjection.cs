using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Interfaces.Http;
using AuthCore.Domain.Core.Interfaces.Messaging;
using AuthCore.Domain.Core.Interfaces.Persistence;
using AuthCore.Domain.Core.Interfaces.Security;
using AuthCore.Infrastructure.Http;
using AuthCore.Infrastructure.Messaging.RabbitMq;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Repositories;
using AuthCore.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services.AddInfrastructureServices()
                           .AddRepositories();
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Http
            services.AddScoped<ICookie, CookieService>();
            services.AddScoped<IDevice, DeviceService>();

            // Security
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISecureKeyGenerator, SecureKeyGenerator>();
            services.AddScoped<IEcdsaSigner, EcdsaSigner>();
            services.AddScoped<IEcdsaProvider, EcdsaSigner>();
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<ISessionState, SessionState>();

            // Messaging
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IEmailPublisher, EmailPublisher>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // PostgreSQL
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            // Redis
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IConfirmCodeRepository, ConfirmCodeRepository>();

            return services;
        }
    }
}
