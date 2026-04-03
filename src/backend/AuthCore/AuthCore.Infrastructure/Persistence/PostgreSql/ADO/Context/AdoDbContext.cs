using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Diagnostics;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context
{
    /// <summary>Cria conexões Npgsql para acesso ao PostgreSQL via ADO.NET.</summary>
    public sealed class AdoDbContext
    {
        private readonly string _connectionString;
        private readonly PostgresSettings _settings;
        private readonly ILogger<AdoDbContext> _logger;

        /// <summary>Operação para inicializar o contexto com a connection string informada.</summary>
        /// <param name="options">Configurações de banco.</param>
        /// <param name="logger">Logger da aplicação.</param>
        public AdoDbContext(IOptions<DatabaseSettings> options, ILogger<AdoDbContext> logger)
        {
            var databaseSettings = options.Value;
            _settings = databaseSettings.Postgres ?? new PostgresSettings();
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_settings.ConnectionString))
                throw new ArgumentException("Connection string inválida.", nameof(options));

            _connectionString = _settings.ConnectionString;
        }

        /// <summary>Operação para criar uma nova conexão Npgsql.</summary>
        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        /// <summary>Operação para executar comando sem retorno.</summary>
        /// <param name="sql">Comando SQL.</param>
        /// <param name="configure">Configuração de parâmetros.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public async Task<int> ExecuteAsync(
            string sql,
            Action<NpgsqlCommand>? configure = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                await using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken);

                await using var command = new NpgsqlCommand(sql, connection)
                {
                    CommandTimeout = _settings.CommandTimeoutSeconds
                };

                configure?.Invoke(command);
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }, sql);
        }

        /// <summary>Operação para executar comando escalar.</summary>
        /// <param name="sql">Comando SQL.</param>
        /// <param name="configure">Configuração de parâmetros.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public async Task<object?> ExecuteScalarAsync(
            string sql,
            Action<NpgsqlCommand>? configure = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                await using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken);

                await using var command = new NpgsqlCommand(sql, connection)
                {
                    CommandTimeout = _settings.CommandTimeoutSeconds
                };

                configure?.Invoke(command);
                return await command.ExecuteScalarAsync(cancellationToken);
            }, sql);
        }

        /// <summary>Operação para executar consulta e mapear resultado.</summary>
        /// <param name="sql">Comando SQL.</param>
        /// <param name="configure">Configuração de parâmetros.</param>
        /// <param name="map">Mapeador do leitor.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public async Task<T?> QueryAsync<T>(
            string sql,
            Action<NpgsqlCommand>? configure,
            Func<NpgsqlDataReader, Task<T?>> map,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                await using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken);

                await using var command = new NpgsqlCommand(sql, connection)
                {
                    CommandTimeout = _settings.CommandTimeoutSeconds
                };

                configure?.Invoke(command);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                return await map(reader);
            }, sql);
        }

        /// <summary>Operação para executar transação.</summary>
        /// <param name="action">Ação com conexão e transação.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        public async Task ExecuteInTransactionAsync(
            Func<NpgsqlConnection, NpgsqlTransaction, Task> action,
            CancellationToken cancellationToken = default)
        {
            await ExecuteWithRetryAsync(async () =>
            {
                await using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken);
                await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

                await action(connection, transaction);
                await transaction.CommitAsync(cancellationToken);
                return 0;
            }, "transaction");
        }

        #region Helpers

        /// <summary>Operação para executar ação com retry e métricas.</summary>
        /// <param name="action">Ação a executar.</param>
        /// <param name="label">Rótulo de log.</param>
        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, string label)
        {
            var retries = Math.Max(0, _settings.RetryCount);
            var delay = Math.Max(0, _settings.RetryDelayMilliseconds);

            for (var attempt = 0; attempt <= retries; attempt++)
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    var result = await action();
                    stopwatch.Stop();
                    _logger.LogInformation("ADO OK ({Label}) em {Elapsed} ms.", label, stopwatch.ElapsedMilliseconds);
                    return result;
                }
                catch (Exception ex) when (IsTransient(ex) && attempt < retries)
                {
                    stopwatch.Stop();
                    _logger.LogWarning(ex, "ADO falhou ({Label}) na tentativa {Attempt}/{Max}.", label, attempt + 1, retries + 1);
                    var backoffDelay = delay * (1 << attempt) + Random.Shared.Next(0, 50);
                    await Task.Delay(backoffDelay);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogError(ex, "ADO erro ({Label}) após {Attempts} tentativa(s).", label, attempt + 1);
                    throw;
                }
            }

            throw new InvalidOperationException("Falha inesperada na execução ADO.");
        }

        /// <summary>Operação para identificar erro transitório.</summary>
        /// <param name="exception">Exceção capturada.</param>
        private static bool IsTransient(Exception exception)
        {
            if (exception is TimeoutException)
                return true;

            if (exception is NpgsqlException pg)
                return pg.SqlState is
                    "08000" or // connection_exception
                    "08006" or // connection_failure
                    "40001" or // serialization_failure (deadlock)
                    "53300" or // too_many_connections
                    "57P03";   // cannot_connect_now

            return false;
        }

        #endregion
    }
}
