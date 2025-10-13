namespace AuthCore.Domain.Commons.Settings
{
    /// <summary>
    /// Representa as configurações de banco de dados utilizadas pela aplicação.
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Configurações de conexão com o banco de dados PostgreSQL.
        /// </summary>
        public PostgresSettings Postgres { get; set; } = new();

        /// <summary>
        /// Configurações de conexão com o Redis.
        /// </summary>
        public RedisSettings Redis { get; set; } = new();
    }

    /// <summary>
    /// Define as informações de conexão com o PostgreSQL.
    /// </summary>
    public class PostgresSettings
    {
        /// <summary>
        /// String de conexão com o banco de dados PostgreSQL.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }

    /// <summary>
    /// Define as informações de conexão com o Redis.
    /// </summary>
    public class RedisSettings
    {
        /// <summary>
        /// String de conexão com o servidor Redis.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Nome da instância Redis utilizada pela aplicação.
        /// </summary>
        public string InstanceName { get; set; } = string.Empty;
    }
}
