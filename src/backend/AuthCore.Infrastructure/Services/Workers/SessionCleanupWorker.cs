using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthCore.Infrastructure.Services.Workers
{
    /// <summary>Worker responsável por remover sessões expiradas e revogadas antigas.</summary>
    public sealed class SessionCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PostgresCleanupSettings _settings;
        private readonly ILogger<SessionCleanupWorker> _logger;

        /// <summary>Operação para criar instância do worker de limpeza de sessões.</summary>
        /// <param name="scopeFactory">Fábrica de escopos de DI.</param>
        /// <param name="options">Configurações de banco.</param>
        /// <param name="logger">Serviço de logging.</param>
        public SessionCleanupWorker(
            IServiceScopeFactory scopeFactory,
            IOptions<DatabaseSettings> options,
            ILogger<SessionCleanupWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _settings = options.Value.Postgres.Cleanup;
            _logger = logger;
        }

        /// <summary>Operação principal do worker em background.</summary>
        /// <param name="stoppingToken">Token de cancelamento.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation("Limpeza de sessões desabilitada.");
                return;
            }

            TimeSpan interval = TimeSpan.FromMinutes(Math.Max(1, _settings.IntervalMinutes));
            _logger.LogInformation(
                "Limpeza de sessões agendada para executar a cada {IntervalMinutes} minuto(s).",
                interval.TotalMinutes);

            using PeriodicTimer timer = new(interval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        await using var scope = _scopeFactory.CreateAsyncScope();
                        var repository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
                        await repository.DeleteExpiredAsync(DateTime.UtcNow, _settings.RevokedRetentionDays);
                        _logger.LogDebug("Limpeza de sessões executada com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Falha na limpeza de sessões.");
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker de limpeza de sessões finalizado.");
            }
        }
    }
}
