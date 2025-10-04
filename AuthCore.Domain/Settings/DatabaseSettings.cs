namespace AuthCore.Domain.Settings
{
    public class DatabaseSettings
    {
        public PostgresSettings Postgres { get; set; } = new();
        public RedisSettings Redis { get; set; } = new();
    }

    public class PostgresSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
    }
}
