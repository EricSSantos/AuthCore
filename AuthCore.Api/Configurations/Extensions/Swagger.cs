using AuthCore.Domain.Core.Settings;
using Microsoft.OpenApi.Models;

namespace AuthCore.Api.Configurations.Extensions
{
    public static class Swagger
    {
        /// <summary>
        /// Registra e configura o Swagger no container de serviços da aplicação,
        /// incluindo os comentários XML gerados automaticamente pelo compilador.
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> utilizada para configuração dos serviços.</param>
        /// <exception cref="InvalidOperationException">Lançada quando as configurações da API não estão definidas ou são inválidas.</exception>
        public static void AddSwaggerService(this WebApplicationBuilder builder)
        {
            var settings = builder.Services.BuildServiceProvider().GetRequiredService<ApiSettings>();
            if (settings is null || settings.Versions is null || settings.Versions.Count == 0)
                throw new InvalidOperationException("As configurações da API não foram definidas.");

            builder.Services.AddSwaggerGen(c =>
            {
                // Define as versões da documentação com base nas configurações
                foreach (var (versionKey, versionInfo) in settings.Versions)
                {
                    c.SwaggerDoc(versionKey, new OpenApiInfo
                    {
                        Title = versionInfo.Name,
                        Version = versionKey,
                        Description = versionInfo.Description
                    });
                }

                // Inclui comentários xml
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var xmlFile in xmlFiles)
                {
                    c.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
                }
            });
        }

        /// <summary>
        /// Aplica os middlewares de Swagger e Swagger UI ao pipeline da aplicação,
        /// gerando a interface de documentação interativa baseada nas versões configuradas.
        /// </summary>
        /// <param name="app">Instância do <see cref="WebApplication"/> utilizada para configuração do pipeline HTTP.</param>
        /// <exception cref="InvalidOperationException">Lançada quando as configurações de API não estão definidas ou são inválidas.</exception>
        public static void UseSwaggerDoc(this WebApplication app)
        {
            var settings = app.Services.GetRequiredService<ApiSettings>();
            if (settings is null || settings.Versions is null || settings.Versions.Count == 0)
                throw new InvalidOperationException("As configurações de API não foram definidas.");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var (versionKey, versionInfo) in settings.Versions)
                {
                    c.SwaggerEndpoint($"/swagger/{versionKey}/swagger.json", versionInfo.Name);
                }

                // Configura layout e aparência
                c.DefaultModelsExpandDepth(-1);
                c.RoutePrefix = string.Empty;
                c.ConfigObject.AdditionalItems["withCredentials"] = true;
                c.InjectStylesheet("/swagger/SwaggerStyle.css");
            });
        }
    }
}
