using AuthCore.Domain.Commons.Settings;
using AuthCore.Infrastructure.Persistence.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthCore.Api.Configurations
{
    public static class DataBase
    {
        public static void AddDatabases(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));
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
            var settings = builder.Configuration.GetSection("Database:Redis").Get<RedisSettings>()!;
            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("A string de conexão do Redis não foi definida.");

            var multiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect(settings.ConnectionString);
            builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(multiplexer);

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = settings.InstanceName;
                options.Configuration = settings.ConnectionString;
            });
        }

        #endregion
    }
}
