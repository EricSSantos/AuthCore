using MailCore.Service.Settings;
using MailCore.Service.Services;
using MailCore.Service.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailCore.Service;

/// <summary>Registra os serviços do MailCore.</summary>
public static class Injections
{
    /// <summary>Adiciona as dependências do MailCore.</summary>
    public static IServiceCollection AddMailCore(this IServiceCollection services, IConfiguration configuration)
    {
        AddValidatedOptions(services, configuration);

        services.AddSingleton<TemplateRenderer>();
        services.AddSingleton<ConfirmEmailTemplate>();
        services.AddSingleton<WelcomeEmailTemplate>();
        services.AddSingleton<ForgotPasswordEmailTemplate>();
        services.AddSingleton<EmailService>();
        services.AddSingleton<RabbitMqEmailConsumer>();
        return services;
    }

    /// <summary>Registra e valida configurações essenciais no startup.</summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <param name="configuration">Configuração da aplicação.</param>
    private static void AddValidatedOptions(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<ApplicationSettings>()
            .Bind(configuration.GetSection("Application"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<SmtpSettings>()
            .Bind(configuration.GetSection("Smtp"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<RabbitMqSettings>()
            .Bind(configuration.GetSection("RabbitMQ"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
