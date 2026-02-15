using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Aggregates.Users.Contracts;
using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using Npgsql;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories
{
    /// <summary>Representa repositório ADO de usuários.</summary>
    public sealed class UserRepository : IUserRepository
    {
        private readonly AdoDbContext _dbContext;

        /// <summary>Operação para criar instância do repositório de usuários.</summary>
        /// <param name="adoDbContext">Contexto ADO.</param>
        public UserRepository(AdoDbContext adoDbContext)
        {
            _dbContext = adoDbContext;
        }

        /// <summary>Operação para obter usuário por identificador.</summary>
        /// <param name="id">Identificador do usuário.</param>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            const string sql = @"
                    select
                        id,
                        first_name,
                        last_name,
                        email,
                        password,
                        role,
                        verified,
                        active,
                        created_at,
                        updated_at,
                        inactivated_at,
                        failed_attempts,
                        last_failed_at,
                        locked_until
                    from users
                    where id = @id;";

            return await _dbContext.QueryAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("id", id),
                async reader =>
                {
                    if (!reader.HasRows)
                        return null;

                    await reader.ReadAsync();
                    return MapUser(reader);
                });
        }

        /// <summary>Operação para obter usuário por e-mail.</summary>
        /// <param name="email">E-mail do usuário.</param>
        public async Task<User?> GetByEmailAsync(string email)
        {
            const string sql = @"
                    select
                        id,
                        first_name,
                        last_name,
                        email,
                        password,
                        role,
                        verified,
                        active,
                        created_at,
                        updated_at,
                        inactivated_at,
                        failed_attempts,
                        last_failed_at,
                        locked_until
                    from users
                    where lower(email) = lower(@email);";

            return await _dbContext.QueryAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("email", email),
                async reader =>
                {
                    if (!reader.HasRows)
                        return null;

                    await reader.ReadAsync();
                    return MapUser(reader);
                });
        }

        /// <summary>Operação para verificar e-mail cadastrado.</summary>
        /// <param name="email">E-mail do usuário.</param>
        public async Task<bool> EmailExistsAsync(string email)
        {
            const string sql = @"
                    select 1
                    from users
                    where lower(email) = lower(@email)
                    limit 1;";

            object? result = await _dbContext.ExecuteScalarAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("email", email));

            return result is not null;
        }

        /// <summary>Operação para adicionar usuário.</summary>
        /// <param name="user">Usuário a ser adicionado.</param>
        public async Task AddAsync(User user)
        {
            const string sql = @"
                    insert into users
                    (
                        id,
                        first_name,
                        last_name,
                        email,
                        password,
                        role,
                        verified,
                        active,
                        created_at,
                        updated_at,
                        inactivated_at,
                        failed_attempts,
                        last_failed_at,
                        locked_until
                    )
                    values
                    (
                        @id,
                        @first_name,
                        @last_name,
                        @email,
                        @password,
                        @role,
                        @verified,
                        @active,
                        @created_at,
                        @updated_at,
                        @inactivated_at,
                        @failed_attempts,
                        @last_failed_at,
                        @locked_until
                    );";

            await _dbContext.ExecuteAsync(
                sql,
                cmd => FillUserParameters(cmd, user));
        }

        /// <summary>Operação para atualizar usuário.</summary>
        /// <param name="user">Usuário a ser atualizado.</param>
        public async Task UpdateAsync(User user)
        {
            const string sql = @"
                    update users
                    set
                        first_name      = @first_name,
                        last_name       = @last_name,
                        email           = @email,
                        password        = @password,
                        role            = @role,
                        verified        = @verified,
                        active          = @active,
                        created_at      = @created_at,
                        updated_at      = @updated_at,
                        inactivated_at  = @inactivated_at,
                        failed_attempts = @failed_attempts,
                        last_failed_at  = @last_failed_at,
                        locked_until    = @locked_until
                    where id = @id;";

            await _dbContext.ExecuteAsync(
                sql,
                cmd => FillUserParameters(cmd, user));
        }

        /// <summary>Operação para remover usuário.</summary>
        /// <param name="user">Usuário a ser removido.</param>
        public async Task DeleteAsync(User user)
        {
            const string sql = @"delete from users where id = @id;";

            await _dbContext.ExecuteAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("id", user.Id));
        }

        /// <summary>Operação para preencher parâmetros de usuário.</summary>
        /// <param name="command">Comando SQL.</param>
        /// <param name="user">Usuário a persistir.</param>
        private static void FillUserParameters(NpgsqlCommand command, User user)
        {
            command.Parameters.AddWithValue("id", user.Id);
            command.Parameters.AddWithValue("first_name", user.FirstName);
            command.Parameters.AddWithValue("last_name", user.LastName);
            command.Parameters.AddWithValue("email", user.Email.Value);
            command.Parameters.AddWithValue("password", user.Password.Value);
            command.Parameters.AddWithValue("role", (int)user.Role);
            command.Parameters.AddWithValue("verified", user.Verified);
            command.Parameters.AddWithValue("active", user.Active);
            command.Parameters.AddWithValue("created_at", user.CreatedAt);

            DateTime? updatedAt = user.UpdatedAt ?? user.CreatedAt;
            command.Parameters.AddWithValue("updated_at", updatedAt);

            command.Parameters.AddWithValue("inactivated_at", (object?)user.InactivatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("failed_attempts", user.LoginAttempts.FailedAttempts);
            command.Parameters.AddWithValue("last_failed_at", (object?)user.LoginAttempts.LastFailedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("locked_until", (object?)user.LoginAttempts.LockedUntil ?? DBNull.Value);
        }

        /// <summary>Operação para mapear usuário a partir do leitor.</summary>
        /// <param name="reader">Leitor de dados.</param>
        private static User MapUser(NpgsqlDataReader reader)
        {
            Guid id = reader.GetGuid(reader.GetOrdinal("id"));
            string firstName = reader.GetString(reader.GetOrdinal("first_name"));
            string lastName = reader.GetString(reader.GetOrdinal("last_name"));
            string emailValue = reader.GetString(reader.GetOrdinal("email"));
            string passwordHash = reader.GetString(reader.GetOrdinal("password"));
            int roleValue = reader.GetInt32(reader.GetOrdinal("role"));
            bool verified = reader.GetBoolean(reader.GetOrdinal("verified"));
            bool active = reader.GetBoolean(reader.GetOrdinal("active"));
            DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));

            DateTime updatedAt = createdAt;
            int updatedAtOrdinal = reader.GetOrdinal("updated_at");
            bool isUpdatedAtNull = reader.IsDBNull(updatedAtOrdinal);
            if (!isUpdatedAtNull)
                updatedAt = reader.GetDateTime(updatedAtOrdinal);

            DateTime? inactivatedAt = null;
            int inactivatedAtOrdinal = reader.GetOrdinal("inactivated_at");
            bool isInactivatedAtNull = reader.IsDBNull(inactivatedAtOrdinal);
            if (!isInactivatedAtNull)
                inactivatedAt = reader.GetDateTime(inactivatedAtOrdinal);

            int failedAttempts = 0;
            int failedAttemptsOrdinal = reader.GetOrdinal("failed_attempts");
            bool isFailedAttemptsNull = reader.IsDBNull(failedAttemptsOrdinal);
            if (!isFailedAttemptsNull)
                failedAttempts = reader.GetInt32(failedAttemptsOrdinal);

            DateTime? lastFailedAt = null;
            int lastFailedAtOrdinal = reader.GetOrdinal("last_failed_at");
            bool isLastFailedAtNull = reader.IsDBNull(lastFailedAtOrdinal);
            if (!isLastFailedAtNull)
                lastFailedAt = reader.GetDateTime(lastFailedAtOrdinal);

            DateTime? lockedUntil = null;
            int lockedUntilOrdinal = reader.GetOrdinal("locked_until");
            bool isLockedUntilNull = reader.IsDBNull(lockedUntilOrdinal);
            if (!isLockedUntilNull)
                lockedUntil = reader.GetDateTime(lockedUntilOrdinal);

            LoginAttempts loginAttempts = LoginAttempts.Restore(
                failedAttempts,
                lastFailedAt,
                lockedUntil);

            Role role = (Role)roleValue;

            User user = User.Restore(
                id,
                firstName,
                lastName,
                emailValue,
                passwordHash,
                role,
                verified,
                active,
                createdAt,
                updatedAt,
                inactivatedAt,
                loginAttempts);

            return user;
        }
    }
}
