namespace AuthCore.Domain.Settings
{
    public class DatabaseSettings
    {
        public PostgresSettings Postgres { get; set; } = new();
    }

    public class PostgresSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
}
