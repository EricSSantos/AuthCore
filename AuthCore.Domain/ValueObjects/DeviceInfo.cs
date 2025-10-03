namespace AuthCore.Domain.ValueObjects
{
    public sealed class DeviceInfo : ValueObject
    {
        #region Properties

        public string Ip { get; }
        public string Platform { get; }
        public string Browser { get; }
        public string Location { get; }

        #endregion

        #region Constructors

        protected DeviceInfo() { }

        private DeviceInfo(
            string ip,
            string platform,
            string browser,
            string location)
        {
            Ip = ip;
            Platform = platform;
            Browser = browser;
            Location = location;
        }

        #endregion

        #region Factory

        public static DeviceInfo Create(
            string ip,
            string platform,
            string browser,
            string location)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("IP é obrigatório.", nameof(ip));

            platform ??= "Unknown";
            browser ??= "Unknown";
            location ??= "Unknown";

            return new DeviceInfo(ip, platform, browser, location);
        }

        #endregion

        #region Behavior

        protected override IEnumerable<object> GetValues()
        {
            yield return Ip;
            yield return Platform;
            yield return Browser;
            yield return Location;
        }

        public override string ToString()
        {
            return $"{Platform} : {Browser} ({Location})";
        }

        #endregion
    }
}