namespace AuthCore.Domain.Core.Settings
{
    /// <summary>Representa configurações de conexão com bancos.</summary>
    public class DatabaseSettings
    {
        public PostgresSettings Postgres { get; set; } = new();

        public RedisSettings Redis { get; set; } = new();
    }

    /// <summary>Representa parâmetros de conexão do PostgreSQL.</summary>
    public class PostgresSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 200;
        public PostgresCleanupSettings Cleanup { get; set; } = new();
    }

    /// <summary>Representa parâmetros de limpeza operacional do PostgreSQL.</summary>
    public class PostgresCleanupSettings
    {
        public bool Enabled { get; set; } = true;
        public int IntervalMinutes { get; set; } = 60;
        public int RevokedRetentionDays { get; set; } = 30;
    }

    /// <summary>Representa parâmetros de conexão do Redis.</summary>
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string KeyPrefix { get; set; } = "authcore";
    }
}
