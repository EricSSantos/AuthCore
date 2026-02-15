using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Net;

namespace AuthCore.Domain.Aggregates.Sessions
{
    /// <summary>Representa dados do dispositivo associado à sessão.</summary>
    public sealed class DeviceInfo : IValueObject
    {
        public string Ip { get; private set; } = null!;
        public string Platform { get; private set; } = null!;
        public string Browser { get; private set; } = null!;

        #region Constructors

        /// <summary>Operação para criar instância de dispositivo.</summary>
        /// <param name="ip">Endereço IP do dispositivo.</param>
        /// <param name="platform">Plataforma do dispositivo.</param>
        /// <param name="browser">Navegador do dispositivo.</param>
        private DeviceInfo(string ip, string platform, string browser)
        {
            Ip = ip;
            Platform = platform;
            Browser = browser;
        }

        #endregion

        #region Factory

        /// <summary>Operação para criar dispositivo.</summary>
        /// <param name="ip">Endereço IP do dispositivo.</param>
        /// <param name="platform">Plataforma do dispositivo.</param>
        /// <param name="browser">Navegador do dispositivo.</param>
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

        #endregion

        #region Validation

        /// <summary>Operação para validar endereço IP.</summary>
        /// <param name="ip">Endereço IP do dispositivo.</param>
        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        #endregion
    }
}
