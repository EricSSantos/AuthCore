using AuthCore.Domain.Core.Settings;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>Configura as políticas de CORS da aplicação.</summary>
    public static class Cors
    {
        private const string DEV_POLICY = "DevPolicy";
        private const string PROD_POLICY = "ProdPolicy";

        /// <summary>Operação para adicionar políticas de CORS com base nas configurações.</summary>
        /// <param name="builder">Instância para configurar serviços.</param>
        public static void AddCorsPolicies(this WebApplicationBuilder builder)
        {
            var corsSettings = builder.Configuration
                .GetSection("Cors")
                .Get<CorsSettings>() ?? new CorsSettings();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(DEV_POLICY, policy =>
                    policy
                        .WithOrigins(corsSettings.DevelopmentOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());

                options.AddPolicy(PROD_POLICY, policy =>
                    policy
                        .WithOrigins(corsSettings.ProductionOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });
        }

        /// <summary>Operação para aplicar a política de CORS correta por ambiente.</summary>
        /// <param name="app">Instância para configurar o pipeline.</param>
        public static void UseCorsPolicy(this WebApplication app)
        {
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();
            app.UseCors(env.IsProduction() ? PROD_POLICY : DEV_POLICY);
        }
    }
}
