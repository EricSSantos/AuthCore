using AuthCore.Domain.Core.Settings;
using Microsoft.OpenApi.Models;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>
    /// Configura e ativa o Swagger na aplicação.
    /// </summary>
    public static class Swagger
    {
        /// <summary>
        /// Adiciona o Swagger aos serviços da aplicação.
        /// </summary>
        /// <param name="builder">Instância usada para configurar os serviços da aplicação.</param>
        /// <exception cref="InvalidOperationException">Lançada quando as configurações da API estão ausentes ou inválidas.</exception>
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

                // Inclui comentários XML do projeto
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var xmlFile in xmlFiles)
                {
                    c.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
                }
            });
        }

        /// <summary>
        /// Ativa o Swagger e a interface interativa Swagger UI.
        /// </summary>
        /// <param name="app">Instância usada para configurar o pipeline HTTP.</param>
        /// <exception cref="InvalidOperationException">Lançada quando as configurações da API estão ausentes ou inválidas.</exception>
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

                c.DefaultModelsExpandDepth(-1);
                c.RoutePrefix = string.Empty;
                c.ConfigObject.AdditionalItems["withCredentials"] = true;
                c.InjectStylesheet("/swagger/SwaggerStyle.css");
            });
        }
    }
}
