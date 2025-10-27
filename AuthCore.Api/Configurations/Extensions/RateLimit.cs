using System.Threading.RateLimiting;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>
    /// Responsável por configurar o middleware de Rate Limiting na aplicação.
    /// Controla o número máximo de requisições por IP dentro de uma janela de tempo fixa.
    /// </summary>
    public static class RateLimit
    {
        /// <summary>
        /// Registra o Rate Limiter no container, aplicando uma política global baseada no endereço IP.
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> usada para configurar os serviços.</param>
        public static void AddRateLimiting(this WebApplicationBuilder builder)
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString()!,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }
                    );
                });
            });
        }
    }
}
