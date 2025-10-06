using AuthCore.Domain.Interfaces.Http;
using AuthCore.Domain.ValueObjects;
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
                    return DeviceInfo.Create("unknown", "unknown", "unknown", "unknown");

                var ip = ResolveIp(context);
                var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
                var clientInfo = _parser.Parse(userAgent);
                var platform = clientInfo.OS.Family.Trim();
                var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}".Trim();
                var location = "Unknown";

                return DeviceInfo.Create(ip, platform, browser, location);
            }
        }

        #region Private Methods

        private static string ResolveIp(HttpContext context)
        {
            var request = context.Request;

            if (request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                return forwardedFor.ToString().Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        #endregion
    }
}
