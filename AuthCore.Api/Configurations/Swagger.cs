using AuthCore.Domain.Commons.Settings;
using Microsoft.OpenApi.Models;

namespace AuthCore.Api.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerService(this WebApplicationBuilder builder)
        {
            var apiSettings = builder.Services
                .BuildServiceProvider()
                .GetRequiredService<ApiSettings>();

            builder.Services.AddSwaggerGen(c =>
            {
                foreach (var (versionKey, versionInfo) in apiSettings.Versions)
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

        public static void UseSwaggerDocumentation(this WebApplication app)
        {
            var appSettings = app.Services.GetRequiredService<ApiSettings>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var (versionKey, versionInfo) in appSettings.Versions)
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
