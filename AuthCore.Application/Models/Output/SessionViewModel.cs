namespace AuthCore.Application.Models.Output
{
    public sealed record class SessionViewModel
    {
        public Guid Id { get; init; }
        public string Ip { get; init; } = string.Empty;
        public string Platform { get; init; } = string.Empty;
        public string Browser { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; }

        public static SessionViewModel FromEntity(Domain.Aggregates.SessionAggregate.Session session)
        {
            return new SessionViewModel
            {
                Id = session.Id,
                Ip = session.DeviceInfo.Ip,
                Platform = session.DeviceInfo.Platform,
                Browser = session.DeviceInfo.Browser,
                CreatedAt = session.CreatedAt
            };
        }
    }
}
