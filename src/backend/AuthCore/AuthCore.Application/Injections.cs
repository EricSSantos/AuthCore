using AuthCore.Application.UseCases.Auth;
using AuthCore.Application.UseCases.Auth.Interfaces;
using AuthCore.Application.UseCases.Notifications;
using AuthCore.Application.UseCases.Notifications.Interfaces;
using AuthCore.Application.UseCases.Sessions;
using AuthCore.Application.UseCases.Sessions.Interfaces;
using AuthCore.Application.UseCases.Users;
using AuthCore.Application.UseCases.Users.Interfaces;
using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Aggregates.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Policies;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Aggregates.Notifications.Policies;
using AuthCore.Domain.Aggregates.ConfirmCodes.Policies;
using AuthCore.Domain.Aggregates.Users.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCore.Application
{
    /// <summary>Representa registro de casos de uso da aplicação.</summary>
    public static class Injections
    {
        /// <summary>Operação para registrar os casos de uso da aplicação.</summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services.AddUseCases();
        }

        /// <summary>Operação para registrar os casos de uso e serviços da camada de aplicação.</summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<ISignIn, SignIn>();
            services.AddScoped<ISignOut, SignOut>();
            services.AddScoped<IRefresh, Refresh>();
            services.AddScoped<ISendNotification, SendNotification>();
            services.AddScoped<IGetSessions, GetSessions>();
            services.AddScoped<IRevokeSession, RevokeSessions>();
            services.AddScoped<IAddUser, AddUser>();
            services.AddScoped<IGetCurrentUser, GetCurrentUser>();
            services.AddScoped<IConfirmEmail, ConfirmEmail>();
            services.AddScoped<IResetPassword, ResetPassword>();
            services.AddScoped<IChangePassword, ChangePassword>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISessionPolicy, DefaultSessionPolicy>();
            services.AddScoped<IUserAuthenticationPolicy, DefaultUserAuthenticationPolicy>();
            services.AddScoped<INotificationPolicy, DefaultNotificationPolicy>();
            services.AddScoped<IConfirmCodePolicy, DefaultConfirmCodePolicy>();
            return services;
        }
    }
}
