using AuthCore.Domain.Commons.Settings;
using Microsoft.OpenApi.Models;

namespace AuthCore.Api.Configurations
{
    public static class SwaggerConfig
    {
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
            });
        }
    }
}
