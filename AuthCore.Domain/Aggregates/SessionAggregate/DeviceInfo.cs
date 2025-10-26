using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Net;

namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    /// <summary>
    /// Representa as informações do dispositivo associado a uma sessão de usuário.
    /// </summary>
    public sealed class DeviceInfo : IValueObject
    {
        #region Properties

        /// <summary>
        /// Endereço IP do dispositivo.
        /// </summary>
        public string Ip { get; }

        /// <summary>
        /// Plataforma ou sistema operacional identificado.
        /// </summary>
        public string Platform { get; }

        /// <summary>
        /// Navegador ou agente de usuário identificado.
        /// </summary>
        public string Browser { get; }

        #endregion

        #region Constructors

        private DeviceInfo(string ip, string platform, string browser)
        {
            Ip = ip;
            Platform = platform;
            Browser = browser;
        }

        #endregion

        #region Factory

        /// <summary>
        /// Cria uma nova instância de informações de dispositivo.
        /// </summary>
        public static DeviceInfo Create(string ip, string? platform, string? browser)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new BadRequestException("O endereço IP é obrigatório.");
            if (!IsValidIp(ip))
                throw new BadRequestException("O endereço IP informado é inválido.");

            platform = string.IsNullOrWhiteSpace(platform) ? "Desconhecido" : platform.Trim();
            browser = string.IsNullOrWhiteSpace(browser) ? "Desconhecido" : browser.Trim();

            return new DeviceInfo(ip.Trim(), platform, browser);
        }

        #endregion

        #region Helpers

        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        #endregion

        #region Equality

        public override bool Equals(object? obj)
        {
            if (obj is not DeviceInfo other)
                return false;

            return Ip == other.Ip
                && Platform == other.Platform
                && Browser == other.Browser;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Ip, Platform, Browser);
        }

        #endregion
    }
}
