using AuthCore.Domain.Aggregates.ConfirmCodes;
using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using Npgsql;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories
{
    /// <summary>Representa repositório ADO de códigos de confirmação.</summary>
    public sealed class ConfirmCodeRepository : IConfirmCodeRepository
    {
        private readonly AdoDbContext _dbContext;

        /// <summary>Operação para criar instância do repositório de códigos.</summary>
        /// <param name="adoDbContext">Contexto ADO.</param>
        public ConfirmCodeRepository(AdoDbContext adoDbContext)
        {
            _dbContext = adoDbContext;
        }

        /// <summary>Operação para obter código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código.</param>
        public async Task<ConfirmCode?> GetAsync(Guid userId, CodeType type)
        {
            const string sql = @"
                    select
                        code,
                        type,
                        created_at,
                        expires_at,
                        attempts
                    from confirm_codes
                    where user_id = @user_id
                      and type = @type;";

            return await _dbContext.QueryAsync(
                sql,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("user_id", userId);
                    cmd.Parameters.AddWithValue("type", (int)type);
                },
                async reader =>
                {
                    if (!reader.HasRows)
                        return null;

                    await reader.ReadAsync();
                    return MapConfirmCode(reader);
                });
        }

        /// <summary>Operação para armazenar código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="confirmCode">Instância do código.</param>
        public async Task SetAsync(Guid userId, ConfirmCode confirmCode)
        {
            const string sql = @"
                    insert into confirm_codes
                    (
                        user_id,
                        type,
                        code,
                        created_at,
                        expires_at,
                        attempts
                    )
                    values
                    (
                        @user_id,
                        @type,
                        @code,
                        @created_at,
                        @expires_at,
                        @attempts
                    )
                    on conflict (user_id, type) do update
                    set
                        code = excluded.code,
                        created_at = excluded.created_at,
                        expires_at = excluded.expires_at,
                        attempts = excluded.attempts;";

            await _dbContext.ExecuteAsync(
                sql,
                cmd => FillConfirmCodeParameters(cmd, userId, confirmCode));
        }

        /// <summary>Operação para remover código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código.</param>
        public async Task DeleteAsync(Guid userId, CodeType type)
        {
            const string sql = @"
                    delete from confirm_codes
                    where user_id = @user_id
                      and type = @type;";

            await _dbContext.ExecuteAsync(
                sql,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("user_id", userId);
                    cmd.Parameters.AddWithValue("type", (int)type);
                });
        }

        /// <summary>Operação para remover códigos de confirmação expirados.</summary>
        public async Task DeleteExpiredAsync()
        {
            const string sql = @"
                    delete from confirm_codes
                    where expires_at < now();";

            await _dbContext.ExecuteAsync(sql);
        }

        /// <summary>Operação para preencher parâmetros do código.</summary>
        /// <param name="command">Comando SQL.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="confirmCode">Código a persistir.</param>
        private static void FillConfirmCodeParameters(
            NpgsqlCommand command,
            Guid userId,
            ConfirmCode confirmCode)
        {
            command.Parameters.AddWithValue("user_id", userId);
            command.Parameters.AddWithValue("type", (int)confirmCode.Type);
            command.Parameters.AddWithValue("code", confirmCode.Code);
            command.Parameters.AddWithValue("created_at", confirmCode.CreatedAt);
            command.Parameters.AddWithValue("expires_at", confirmCode.ExpiresAt);
            command.Parameters.AddWithValue("attempts", confirmCode.Attempts);
        }

        /// <summary>Operação para mapear código de confirmação a partir do leitor.</summary>
        /// <param name="reader">Leitor de dados.</param>
        private static ConfirmCode MapConfirmCode(NpgsqlDataReader reader)
        {
            int code = reader.GetInt32(reader.GetOrdinal("code"));
            CodeType type = (CodeType)reader.GetInt32(reader.GetOrdinal("type"));
            DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
            DateTime expiresAt = reader.GetDateTime(reader.GetOrdinal("expires_at"));
            int attempts = reader.GetInt32(reader.GetOrdinal("attempts"));

            return ConfirmCode.Restore(code, type, createdAt, expiresAt, attempts);
        }
    }
}
