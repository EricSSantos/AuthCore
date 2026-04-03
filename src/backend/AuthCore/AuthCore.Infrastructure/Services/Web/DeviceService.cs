using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Web;
using Microsoft.AspNetCore.Http;
using System.Net;
using UAParser;

namespace AuthCore.Infrastructure.Services.Web
{
    /// <summary>Representa serviço de identificação de dispositivo.</summary>
    public sealed class DeviceService : IDevice
    {
        private readonly IHttpContextAccessor _http;
        private readonly Parser _parser;

        /// <summary>Operação para criar instância do serviço de identificação de dispositivo.</summary>
        /// <param name="http">Acessor de contexto HTTP.</param>
        public DeviceService(IHttpContextAccessor http)
        {
            _http = http;
            _parser = Parser.GetDefault();
        }

        /// <summary>Operação para obter informações do dispositivo atual.</summary>
        public DeviceInfo Get()
        {
            HttpContext? context = _http.HttpContext;
            if (context is null)
                return DeviceInfo.Create("unknown", "unknown", "unknown");

            string userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "unknown";
            string ip = GetIp(context);
            ClientInfo clientInfo = _parser.Parse(userAgent);
            string platform = clientInfo.OS.Family.Trim();
            string browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}".Trim();

            return DeviceInfo.Create(ip, platform, browser);
        }

        /// <summary>Operação para obter o endereço IP da requisição atual.</summary>
        /// <param name="context">Contexto HTTP atual.</param>
        /// <remarks>
        /// Usa apenas RemoteIpAddress para evitar confiar em cabeçalhos injetáveis pelo cliente.
        /// Em cenários com proxy reverso, o pipeline deve normalizar isso via ForwardedHeaders.
        /// </remarks>
        private static string GetIp(HttpContext context)
        {
            IPAddress? ip = context.Connection.RemoteIpAddress;
            if (ip is null)
                return "unknown";

            if (IPAddress.IsLoopback(ip))
                return "127.0.0.1";

            if (ip.IsIPv4MappedToIPv6)
                return ip.MapToIPv4().ToString();

            return ip.ToString();
        }
    }
}
