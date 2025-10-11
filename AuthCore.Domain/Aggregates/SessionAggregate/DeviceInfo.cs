using AuthCore.Domain.Shared;

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

        private DeviceInfo(
            string ip,
            string platform,
            string browser)
        {
            Ip = ip;
            Platform = platform;
            Browser = browser;
        }

        #endregion

        #region Factory

        public static DeviceInfo Create(
            string ip,
            string platform,
            string browser)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("IP é obrigatório.", nameof(ip));

            platform ??= "Unknown";
            browser ??= "Unknown";

            return new DeviceInfo(ip, platform, browser);
        }

        #endregion

        #region Behavior

        protected override IEnumerable<object> GetValues()
        {
            yield return Ip;
            yield return Platform;
            yield return Browser;
        }

        public override string ToString()
        {
            return $"{Platform} : {Browser} ";
            return $"{Platform} : {Browser} ";
        }

        #endregion
    }
}