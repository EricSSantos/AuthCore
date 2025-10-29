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
            return services.AddUseCases();
        }

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            // Auth
            services.AddScoped<ISignIn, SignIn>();
            services.AddScoped<ISignOut, SignOut>();
            services.AddScoped<IRefresh, Refresh>();

            // Session
            services.AddScoped<IGetSessions, GetOtherSessions>();
            services.AddScoped<IRevokeSession, RevokeSessions>();

            // User
            services.AddScoped<IAddUser, AddUser>();
            services.AddScoped<IGetCurrentUser, GetCurrentUser>();
            services.AddScoped<IForgotPassword, ForgotPassword>();
            services.AddScoped<IResetPassword, ResetPassword>();
            services.AddScoped<IChangePassword, ChangePassword>();
            services.AddScoped<IConfirmEmail, ConfirmEmail>();

            return services;
        }
    }
}
