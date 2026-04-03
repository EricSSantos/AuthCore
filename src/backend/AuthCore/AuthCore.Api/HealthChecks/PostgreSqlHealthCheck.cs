using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuthCore.Api.HealthChecks
{
    /// <summary>Verifica a disponibilidade do PostgreSQL.</summary>
    public sealed class PostgreSqlHealthCheck : IHealthCheck
    {
        private readonly AdoDbContext _dbContext;

        /// <summary>Operação para criar instância do health check de PostgreSQL.</summary>
        /// <param name="dbContext">Contexto ADO responsável pela conexão com PostgreSQL.</param>
        public PostgreSqlHealthCheck(AdoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Operação para verificar se o PostgreSQL está disponível para consultas.</summary>
        /// <param name="context">Contexto da execução do health check.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.ExecuteScalarAsync("SELECT 1", cancellationToken: cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
