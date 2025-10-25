using AuthCore.Domain.Commons.Settings;

namespace AuthCore.Api.Configurations.Extensions
{
    public static class Cors
    {
        #region Constants

        private const string DEV_POLICY = "DevPolicy";
        private const string PROD_POLICY = "ProdPolicy";

        #endregion

        /// <summary>
        /// Registra as políticas de CORS no container de injeção de dependência,
        /// utilizando as origens definidas no arquivo de configuração (appsettings.json).
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> utilizada para configuração dos serviços da aplicação.</param>
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

        /// <summary>
        /// Aplica o redirecionamento HTTPS, habilita o roteamento
        /// e define a política de CORS correspondente ao ambiente atual.
        /// </summary>
        /// <param name="app">Instância do <see cref="WebApplication"/> utilizada para configuração do pipeline HTTP.</param>
        public static void UseCorsAndHttps(this WebApplication app)
        {
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(env.IsProduction() ? PROD_POLICY : DEV_POLICY);
        }
    }
}
