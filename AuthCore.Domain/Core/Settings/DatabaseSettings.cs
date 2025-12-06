namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define configurações de conexão com os bancos utilizados.
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Define conexão com o PostgreSQL.
        /// </summary>
        public PostgresSettings Postgres { get; set; } = new();

        /// <summary>
        /// Define conexão com o Redis.
        /// </summary>
        public RedisSettings Redis { get; set; } = new();
    }

    /// <summary>
    /// Define parâmetros de conexão para PostgreSQL.
    /// </summary>
    public class PostgresSettings
    {
        /// <summary>
        /// Define a string de conexão do PostgreSQL.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }

    /// <summary>
    /// Define parâmetros de conexão para Redis.
    /// </summary>
    public class RedisSettings
    {
        /// <summary>
        /// Define a string de conexão do Redis.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }
}
