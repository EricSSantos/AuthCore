using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Infrastructure.Persistence.Redis.Mappings;
using StackExchange.Redis;
using System.Text.Json;

namespace AuthCore.Infrastructure.Persistence.Redis.Repositories
{
    public sealed class ConfirmCodeRepository : IConfirmCodeRepository
    {
        #region Constants

        private const string PREFIX = "confirm-code:";
        private static readonly TimeSpan DEFAULT_TTL = TimeSpan.FromMinutes(5);

        #endregion

        private readonly IDatabase _redis;

        public ConfirmCodeRepository(IConnectionMultiplexer connection)
        {
            _redis = connection.GetDatabase();
        }

        public async Task Set(Guid userId, ConfirmCode verificationCode)
        {
            var key = BuildKey(userId, verificationCode.Type);

            var document = new ConfirmCodeDocument
            {
                Code = verificationCode.Code,
                Type = verificationCode.Type,
                CreatedAt = verificationCode.CreatedAt
            };

            await _redis.StringSetAsync(key, JsonSerializer.Serialize(document), DEFAULT_TTL);
        }

        public async Task<object?> Get(Guid userId, CodeType type)
        {
            var key = BuildKey(userId, type);

            var value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<ConfirmCodeDocument>(value!);
        }

        #region Private Methods

        private static string BuildKey(Guid userId, CodeType type)
        {
            return $"{PREFIX}{type.ToString().ToLower()}:{userId}";
        }

        #endregion
    }
}
