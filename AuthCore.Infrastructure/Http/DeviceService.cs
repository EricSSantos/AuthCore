using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Core.Interfaces.Http;
using Microsoft.AspNetCore.Http;
using UAParser;

namespace AuthCore.Infrastructure.Http
{
    public sealed class DeviceService : IDevice
    {
        private readonly IHttpContextAccessor _http;
        private readonly Parser _parser;

        public DeviceService(IHttpContextAccessor http)
        {
            _http = http;
            _parser = Parser.GetDefault();
        }

        public DeviceInfo Device
        {
            get
            {
                var context = _http.HttpContext;
                if (context is null)
                    return DeviceInfo.Create("unknown", "unknown", "unknown");

                var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";

                var ip = GetIp(context);
                var clientInfo = _parser.Parse(userAgent);
                var platform = clientInfo.OS.Family.Trim();
                var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}".Trim();

                return DeviceInfo.Create(ip, platform, browser);
            }
        }

        #region Helpers

        private static string GetIp(HttpContext context)
        {
            var request = context.Request;

            if (request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                return forwardedFor.ToString().Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        #endregion
    }
}
