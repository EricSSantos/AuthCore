using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;
using System.Net;

namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    public sealed class DeviceInfo : ValueObject
    {
        #region Properties

        public string Ip { get; }
        public string Platform { get; }
        public string Browser { get; }

        #endregion

        #region Constructors

        protected DeviceInfo() { }

        private DeviceInfo(string ip, string platform, string browser)
        {
            Ip = ip;
            Platform = platform;
            Browser = browser;
        }

        #endregion

        #region Factory

        public static DeviceInfo Create(string ip, string? platform, string? browser)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(ip))
                errors.Add("O endereço IP é obrigatório.");
            else if (!IsValidIp(ip))
                errors.Add("O endereço IP informado é inválido.");

            platform = string.IsNullOrWhiteSpace(platform) ? "Desconhecido" : platform.Trim();
            browser = string.IsNullOrWhiteSpace(browser) ? "Desconhecido" : browser.Trim();

            if (errors.Any())
                throw new BadRequestException(errors);

            return new DeviceInfo(ip.Trim(), platform.Trim(), browser.Trim());
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Verifica se o endereço IP informado é válido.
        /// </summary>
        private static bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        protected override IEnumerable<object> GetValues()
        {
            yield return Ip;
            yield return Platform;
            yield return Browser;
        }

        #endregion
    }
}
