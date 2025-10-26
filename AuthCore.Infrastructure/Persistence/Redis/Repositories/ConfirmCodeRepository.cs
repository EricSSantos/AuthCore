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

        public async Task<ConfirmCode?> Get(Guid userId, CodeType type)
        {
            var key = BuildKey(userId, type);
            var value = await _redis.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return null;

            var document = JsonSerializer.Deserialize<ConfirmCodeDocument>(value!);
            if (document is null)
                return null;

            return document.ToEntity();
        }

        public async Task Set(Guid userId, ConfirmCode confirmCode)
        {
            var key = BuildKey(userId, confirmCode.Type);

            var document = ConfirmCodeDocument.ToDocument(confirmCode, userId);

            var json = JsonSerializer.Serialize(document);
            await _redis.StringSetAsync(key, json, DEFAULT_TTL);
        }

        #region Helpers

        private static string BuildKey(Guid userId, CodeType type)
        {
            return $"{PREFIX}{type.ToString().ToLower()}:{userId}";
        }

        #endregion
    }
}
