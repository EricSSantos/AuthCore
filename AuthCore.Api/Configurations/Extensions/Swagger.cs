using AuthCore.Domain.Core.Settings;
using Microsoft.OpenApi.Models;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>Configura o Swagger da aplicação.</summary>
    public static class Swagger
    {
        /// <summary>Operação para registrar o Swagger nos serviços.</summary>
        /// <param name="builder">Instância usada para configurar os serviços.</param>
        public static void AddSwaggerService(this WebApplicationBuilder builder)
        {
            var settings = builder.Configuration.GetSection("Api").Get<ApiSettings>();
            if (settings is null || settings.Versions is null || settings.Versions.Count == 0)
                throw new InvalidOperationException("As configurações da API não foram definidas.");

            builder.Services.AddSwaggerGen(c =>
            {
                foreach (var (versionKey, versionInfo) in settings.Versions)
                {
                    c.SwaggerDoc(versionKey, new OpenApiInfo
                    {
                        Title = versionInfo.Name,
                        Version = versionKey,
                        Description = versionInfo.Description
                    });
                }

                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var xml in xmlFiles)
                {
                    c.IncludeXmlComments(xml, includeControllerXmlComments: true);
                }
            });
        }

        /// <summary>Operação para ativar o Swagger e a interface Swagger UI.</summary>
        /// <param name="app">Instância usada para configurar o pipeline HTTP.</param>
        public static void UseSwaggerDoc(this WebApplication app)
        {
            var settings = app.Services.GetRequiredService<ApiSettings>();
            if (settings is null || settings.Versions is null || settings.Versions.Count == 0)
                throw new InvalidOperationException("As configurações da API não foram definidas.");

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
