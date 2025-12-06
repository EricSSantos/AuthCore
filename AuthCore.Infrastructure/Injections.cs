using AuthCore.Domain.Aggregates.ConfirmCodeAggregate.Interfaces;
using AuthCore.Domain.Aggregates.MessagingAggregate.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Http;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Messaging;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Persistence;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Infrastructure.Http;
using AuthCore.Infrastructure.Messaging.RabbitMq;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Repositories;
using AuthCore.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCore.Infrastructure
{
    public static class Injections
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services.AddRepositories()
                           .AddInfrastructureServices();
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IConfirmCodeRepository, ConfirmCodeRepository>();
            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ICookie, CookieService>();
            services.AddScoped<IDevice, DeviceService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISecureKeyGenerator, SecureKeyGenerator>();
            services.AddScoped<IEcdsaSigner, EcdsaSigner>();
            services.AddScoped<IEcdsaProvider, EcdsaSigner>();
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<ISessionState, SessionState>();
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IEmailPublisher, EmailPublisher>();
            return services;
        }
    }
}
