using AuthCore.Application.UseCases.AuthCase;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Application.UseCases.EmailCase;
using AuthCore.Application.UseCases.EmailCase.Interface;
using AuthCore.Application.UseCases.SessionCase;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Application.UseCases.UserCase;
using AuthCore.Application.UseCases.UserCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCore.Application
{
    public static class Injections
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services.AddUseCases();
        }

        private static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<ISignIn, SignIn>();
            services.AddScoped<ISignOut, SignOut>();
            services.AddScoped<IRefresh, Refresh>();
            services.AddScoped<ISendEmail, SendEmail>();
            services.AddScoped<IGetSessions, GetOtherSessions>();
            services.AddScoped<IRevokeSession, RevokeSessions>();
            services.AddScoped<IAddUser, AddUser>();
            services.AddScoped<IGetCurrentUser, GetCurrentUser>();
            services.AddScoped<IConfirmEmail, ConfirmEmail>();
            services.AddScoped<IResetPassword, ResetPassword>();
            services.AddScoped<IChangePassword, ChangePassword>();
            return services;
        }
    }
}
