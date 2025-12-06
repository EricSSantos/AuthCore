using Npgsql;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context
{
    /// <summary>
    /// Cria conexões Npgsql para acesso ao PostgreSQL via ADO.NET.
    /// </summary>
    public sealed class AdoDbContext
    {
        private readonly string _connectionString;

        /// <summary>
        /// Inicializa o contexto com a connection string informada.
        /// </summary>
        public AdoDbContext(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string inválida.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Cria uma nova conexão Npgsql.
        /// </summary>
        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
