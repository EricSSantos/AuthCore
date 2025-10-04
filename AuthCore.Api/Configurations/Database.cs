using AuthCore.Domain.Settings;
using AuthCore.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations
{
    public static class DataBase
    {
        public static void AddDatabases(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<DatabaseSettings>(
                builder.Configuration.GetSection("Database"));

            builder.AddPostgreSql();
            builder.AddRedis();
        }

        #region PostgreSQL
        private static void AddPostgreSql(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                var connectionString = settings.Postgres.ConnectionString;

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("A string de conexão do Postgres não foi definida.");

                options.UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
        }
        #endregion

        #region Redis
        private static void AddRedis(this WebApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                var redisSettings = builder.Configuration
                    .GetSection("Database:Redis")
                    .Get<RedisSettings>()!;

                options.InstanceName = redisSettings.InstanceName;
                options.Configuration = redisSettings.ConnectionString;
            });
        }
        #endregion
    }
}
