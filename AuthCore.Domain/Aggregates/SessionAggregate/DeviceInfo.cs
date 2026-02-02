using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Net;

namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    /// <summary>
    /// Representa os dados do dispositivo associado à sessão.
    /// </summary>
    public sealed class DeviceInfo : IValueObject
    {
        public string Ip { get; private set; } = null!;
        public string Platform { get; private set; } = null!;
        public string Browser { get; private set; } = null!;

        private DeviceInfo(string ip, string platform, string browser)
        {
            Ip = ip;
            Platform = platform;
            Browser = browser;
        }

        public static DeviceInfo Create(string ip, string? platform, string? browser)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new BadRequestException("O endereço IP é obrigatório.");

            if (!IsValidIp(ip))
                throw new BadRequestException("O endereço IP informado é inválido.");

            platform = string.IsNullOrWhiteSpace(platform)
                ? "Desconhecido"
                : platform.Trim();

            browser = string.IsNullOrWhiteSpace(browser)
                ? "Desconhecido"
                : browser.Trim();

            return new DeviceInfo(ip.Trim(), platform, browser);
        }

        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
    }
}
