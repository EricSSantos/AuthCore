using AuthCore.Application.Services;
using AuthCore.Application.Services.Interfaces;
using AuthCore.Application.UseCases.AuthCase;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Application.UseCases.SessionCase;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Application.UseCases.UserCase;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services
                .AddApplicationServices()
                .AddUseCases();

            return services;
        }

        #region Application

        private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Serviços de aplicação (cross use cases)
            services.AddScoped<IOwnership, Ownership>();

            return services;
        }

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            // Auth
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
    }
}
