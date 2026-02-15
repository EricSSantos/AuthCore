using AuthCore.Domain.Aggregates.ConfirmCodes;
using System.Text.Json.Serialization;

namespace AuthCore.Infrastructure.Persistence.Redis.Mappings
{
    /// <summary>Representa documento Redis de código de confirmação.</summary>
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

        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; init; }

        [JsonPropertyName("attempts")]
        public int Attempts { get; init; }

        #region Conversion

        /// <summary>Operação para converter código em documento.</summary>
        /// <param name="confirmCode">Instância do código.</param>
        /// <param name="userId">Identificador do usuário.</param>
        public static ConfirmCodeDocument ToDocument(ConfirmCode confirmCode, Guid userId)
        {
            return new ConfirmCodeDocument
            {
                Id = confirmCode.Id,
                Code = confirmCode.Code,
                Type = confirmCode.Type,
                UserId = userId,
                CreatedAt = ToUnix(confirmCode.CreatedAt),
                ExpiresAt = ToUnix(confirmCode.ExpiresAt),
                Attempts = confirmCode.Attempts
            };
        }

        /// <summary>Operação para converter documento em código.</summary>
        public ConfirmCode ToEntity()
        {
            return ConfirmCode.Restore(
                id: Id,
                code: Code,
                type: Type,
                createdAt: FromUnix(CreatedAt),
                expiresAt: FromUnix(ExpiresAt),
                attempts: Attempts
            );
        }

        #endregion

        #region Helpers

        /// <summary>Operação para converter data em Unix time.</summary>
        /// <param name="date">Data a converter.</param>
        private static long ToUnix(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }

        /// <summary>Operação para converter Unix time em data.</summary>
        /// <param name="seconds">Valor em segundos.</param>
        private static DateTime FromUnix(long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }

        #endregion
    }
}
