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
        /// Cria informações sobre o dispositivo.
        /// </summary>
        /// <param name="ip">Endereço IP do dispositivo.</param>
        /// <param name="platform">Plataforma de acesso.</param>
        /// <param name="browser">Navegador utilizado.</param>
        /// <returns>Instância criada de <see cref="DeviceInfo"/>.</returns>
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

        /// <summary>
        /// Valida o formato do endereço IP.
        /// </summary>
        /// <param name="ip">Endereço IP a validar.</param>
        /// <returns>True quando o formato é válido.</returns>
        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        #endregion

        #region Equality

        /// <summary>
        /// Compara esta instância com outro objeto.
        /// </summary>
        /// <param name="obj">Objeto a comparar.</param>
        /// <returns>True quando os valores são iguais.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not DeviceInfo other)
                return false;

            return Ip == other.Ip
                && Platform == other.Platform
                && Browser == other.Browser;
        }

        /// <summary>
        /// Gera o hash baseado nos valores do objeto.
        /// </summary>
        /// <returns>Hash calculado.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Ip, Platform, Browser);
        }

        #endregion
    }
}
