using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using Npgsql;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories
{
    /// <summary>Representa repositório ADO de sessões.</summary>
    public sealed class SessionRepository : ISessionRepository
    {
        private readonly AdoDbContext _dbContext;

        /// <summary>Operação para criar instância do repositório de sessões.</summary>
        /// <param name="adoDbContext">Contexto ADO.</param>
        public SessionRepository(AdoDbContext adoDbContext)
        {
            _dbContext = adoDbContext;
        }

        /// <summary>Operação para obter sessão por identificador.</summary>
        /// <param name="id">Identificador da sessão.</param>
        public async Task<Session?> GetAsync(string id)
        {
            const string sql = @"
                    select
                        id,
                        user_id,
                        ip,
                        platform,
                        browser,
                        created_at,
                        expires_at,
                        max_lifetime,
                        revoked_at
                    from sessions
                    where id = @id;";

            return await _dbContext.QueryAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("id", id),
                async reader =>
                {
                    if (!reader.HasRows)
                        return null;

                    await reader.ReadAsync();
                    return MapSession(reader);
                });
        }

        /// <summary>Operação para obter sessões do usuário.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        public async Task<IEnumerable<Session>> GetAllByUserIdAsync(Guid userId)
        {
            const string sql = @"
                    select
                        id,
                        user_id,
                        ip,
                        platform,
                        browser,
                        created_at,
                        expires_at,
                        max_lifetime,
                        revoked_at
                    from sessions
                    where user_id = @user_id
                    order by created_at desc;";

            return await _dbContext.QueryAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("user_id", userId),
                async reader =>
                {
                    List<Session> sessions = new();

                    while (await reader.ReadAsync())
                        sessions.Add(MapSession(reader));

                    return sessions;
                }) ?? Enumerable.Empty<Session>();
        }

        /// <summary>Operação para armazenar sessão.</summary>
        /// <param name="session">Instância da sessão.</param>
        public async Task SetAsync(Session session)
        {
            const string sql = @"
                    insert into sessions
                    (
                        id,
                        user_id,
                        ip,
                        platform,
                        browser,
                        created_at,
                        expires_at,
                        max_lifetime,
                        revoked_at
                    )
                    values
                    (
                        @id,
                        @user_id,
                        @ip,
                        @platform,
                        @browser,
                        @created_at,
                        @expires_at,
                        @max_lifetime,
                        @revoked_at
                    )
                    on conflict (id) do update
                    set
                        user_id = excluded.user_id,
                        ip = excluded.ip,
                        platform = excluded.platform,
                        browser = excluded.browser,
                        created_at = excluded.created_at,
                        expires_at = excluded.expires_at,
                        max_lifetime = excluded.max_lifetime,
                        revoked_at = excluded.revoked_at;";

            await _dbContext.ExecuteAsync(sql, cmd => FillSessionParameters(cmd, session));
        }

        /// <summary>Operação para remover sessão.</summary>
        /// <param name="id">Identificador da sessão.</param>
        public async Task DeleteAsync(string id)
        {
            const string sql = @"
                    delete from sessions
                    where id = @id;";

            await _dbContext.ExecuteAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("id", id));
        }

        /// <summary>Operação para remover sessões expiradas e revogadas antigas.</summary>
        /// <param name="utcNow">Data/hora atual em UTC.</param>
        /// <param name="revokedRetentionDays">Dias de retenção após revogação.</param>
        public async Task DeleteExpiredAsync(DateTime utcNow, int revokedRetentionDays)
        {
            const string sql = @"
                    delete from sessions
                    where expires_at < @now
                       or (revoked_at is not null and revoked_at < @revoked_cutoff);";

            DateTime revokedCutoff = utcNow.AddDays(-Math.Max(1, revokedRetentionDays));

            await _dbContext.ExecuteAsync(
                sql,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("now", utcNow);
                    cmd.Parameters.AddWithValue("revoked_cutoff", revokedCutoff);
                });
        }

        /// <summary>Operação para preencher parâmetros da sessão.</summary>
        /// <param name="command">Comando SQL.</param>
        /// <param name="session">Sessão a persistir.</param>
        private static void FillSessionParameters(NpgsqlCommand command, Session session)
        {
            command.Parameters.AddWithValue("id", session.Id.Value);
            command.Parameters.AddWithValue("user_id", session.UserId);
            command.Parameters.AddWithValue("ip", session.DeviceInfo.Ip);
            command.Parameters.AddWithValue("platform", session.DeviceInfo.Platform);
            command.Parameters.AddWithValue("browser", session.DeviceInfo.Browser);
            command.Parameters.AddWithValue("created_at", session.CreatedAt);
            command.Parameters.AddWithValue("expires_at", session.ExpiresAt);
            command.Parameters.AddWithValue("max_lifetime", session.MaxLifetime);
            command.Parameters.AddWithValue("revoked_at", (object?)session.RevokedAt ?? DBNull.Value);
        }

        /// <summary>Operação para mapear sessão a partir do leitor.</summary>
        /// <param name="reader">Leitor de dados.</param>
        private static Session MapSession(NpgsqlDataReader reader)
        {
            string id = reader.GetString(reader.GetOrdinal("id"));
            Guid userId = reader.GetGuid(reader.GetOrdinal("user_id"));
            string ip = reader.GetString(reader.GetOrdinal("ip"));
            string platform = reader.GetString(reader.GetOrdinal("platform"));
            string browser = reader.GetString(reader.GetOrdinal("browser"));
            DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
            DateTime expiresAt = reader.GetDateTime(reader.GetOrdinal("expires_at"));
            DateTime maxLifetime = reader.GetDateTime(reader.GetOrdinal("max_lifetime"));

            DateTime? revokedAt = null;
            int revokedAtOrdinal = reader.GetOrdinal("revoked_at");
            if (!reader.IsDBNull(revokedAtOrdinal))
                revokedAt = reader.GetDateTime(revokedAtOrdinal);

            DeviceInfo deviceInfo = DeviceInfo.Create(ip, platform, browser);

            return Session.Restore(id, userId, deviceInfo, createdAt, expiresAt, maxLifetime, revokedAt);
        }
    }
}
