using AuthCore.Domain.Settings;
using AuthCore.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations
{
    public static class DataBase
    {
        public static void AddDatabases(this WebApplicationBuilder builder)
        {
            builder.AddPostgreSql();
        }

        private static void AddPostgreSql(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                var connectionString = settings.Postgres.ConnectionString;

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("String de conexão não foi definida.");

                options.UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
        }
    }
}
