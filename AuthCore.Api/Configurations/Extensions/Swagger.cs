using AuthCore.Domain.Commons.Settings;
using Microsoft.OpenApi.Models;

namespace AuthCore.Api.Configurations.Extensions
{
    public static class Swagger
    {
        /// <summary>
        /// Registra e configura o Swagger no container de serviços da aplicação,
        /// utilizando as informações de versão definidas em <see cref="ApiSettings"/>.
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
                foreach (var (versionKey, versionInfo) in settings.Versions)
                {
                    c.SwaggerDoc(versionKey, new OpenApiInfo
                    {
                        Title = versionInfo.Name,
                        Version = versionKey,
                        Description = versionInfo.Description
                    });
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

                // Oculta os modelos de schema e habilita credenciais
                c.DefaultModelsExpandDepth(-1);
                c.ConfigObject.AdditionalItems["withCredentials"] = true;
            });
        }
    }
}
