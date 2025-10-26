using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Mappings
{
    public sealed class ConfirmCodeDocument
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("code")]
        public int Code { get; init; } = default!;

        [JsonPropertyName("type")]
        public CodeType Type { get; init; } = default!;

        [JsonPropertyName("user_id")]
        public Guid UserId { get; init; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; init; }

        #region Conversion

        public static ConfirmCodeDocument ToDocument(ConfirmCode confirmCode, Guid userId)
        {
            return new ConfirmCodeDocument
            {
                Id = confirmCode.Id,
                Code = confirmCode.Code,
                Type = confirmCode.Type,
                UserId = userId,
                CreatedAt = ToUnix(confirmCode.CreatedAt)
            };
        }

        public ConfirmCode ToEntity()
        {
            return ConfirmCode.Restore(
                id: Id,
                code: Code,
                type: Type,
                createdAt: FromUnix(CreatedAt)
            );
        }

        #endregion

        #region Helpers

        private static long ToUnix(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }

        private static DateTime FromUnix(long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }

        #endregion
    }
}
