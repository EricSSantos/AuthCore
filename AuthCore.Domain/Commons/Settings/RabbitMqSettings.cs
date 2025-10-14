namespace AuthCore.Domain.Commons.Settings
{
    public sealed class RabbitMqSettings
    {
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; }
        public string User { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string EmailQueue { get; init; } = string.Empty;
        public string DeadLetterQueue { get; init; } = string.Empty;
    }
}
