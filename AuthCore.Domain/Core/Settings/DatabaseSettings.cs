namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Representa as configurações de conexão com os bancos de dados utilizados pela aplicação.
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Configurações de conexão com o PostgreSQL.
        /// </summary>
        public PostgresSettings Postgres { get; set; } = new();

        /// <summary>
        /// Configurações de conexão com o Redis.
        /// </summary>
        public RedisSettings Redis { get; set; } = new();
    }

    /// <summary>
    /// Define os parâmetros de conexão com o PostgreSQL.
    /// </summary>
    public class PostgresSettings
    {
        /// <summary>
        /// String de conexão com o banco de dados PostgreSQL.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }

    /// <summary>
    /// Define os parâmetros de conexão com o Redis.
    /// </summary>
    public class RedisSettings
    {
        /// <summary>
        /// String de conexão com o servidor Redis.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }
}
