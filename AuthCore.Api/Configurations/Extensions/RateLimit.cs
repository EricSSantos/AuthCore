using System.Threading.RateLimiting;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>
    /// Configura o middleware global de Rate Limiting.
    /// </summary>
    public static class RateLimit
    {
        /// <summary>
        /// Registra o Rate Limiter baseado no IP do cliente.
        /// </summary>
        /// <param name="builder">Instância usada para configurar os serviços.</param>
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
