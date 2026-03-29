using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace AuthCore.Api.HealthChecks
{
    /// <summary>Verifica a disponibilidade do Redis.</summary>
    public sealed class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _connection;

        /// <summary>Operação para criar instância do health check de Redis.</summary>
        /// <param name="connection">Conexão multiplexada com Redis.</param>
        public RedisHealthCheck(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        /// <summary>Operação para verificar se o Redis está disponível para operações básicas.</summary>
        /// <param name="context">Contexto da execução do health check.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var db = _connection.GetDatabase();
                await db.PingAsync();
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
