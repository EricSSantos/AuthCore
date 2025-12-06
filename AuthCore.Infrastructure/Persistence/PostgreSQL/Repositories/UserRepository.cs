using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using Npgsql;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AdoDbContext _dbContext;

        public UserRepository(AdoDbContext adoDbContext)
        {
            _dbContext = adoDbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using (NpgsqlConnection connection = _dbContext.CreateConnection())
            {
                await connection.OpenAsync();

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

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        bool hasRows = reader.HasRows;
                        if (!hasRows)
                            return null;

                        await reader.ReadAsync();

                        User user = MapUser(reader);

                        return user;
                    }
                }
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using (NpgsqlConnection connection = _dbContext.CreateConnection())
            {
                await connection.OpenAsync();

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

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("email", email);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        bool hasRows = reader.HasRows;
                        if (!hasRows)
                            return null;

                        await reader.ReadAsync();

                        User user = MapUser(reader);

                        return user;
                    }
                }
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using (NpgsqlConnection connection = _dbContext.CreateConnection())
            {
                await connection.OpenAsync();

                const string sql = @"
                    select 1
                    from users
                    where lower(email) = lower(@email)
                    limit 1;";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("email", email);

                    object? result = await command.ExecuteScalarAsync();
                    bool emailExists = result is not null;

                    return emailExists;
                }
            }
        }

        public async Task AddAsync(User user)
        {
            using (NpgsqlConnection connection = _dbContext.CreateConnection())
            {
                await connection.OpenAsync();

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

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    FillUserParameters(command, user);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(User user)
        {
            using (NpgsqlConnection connection = _dbContext.CreateConnection())
            {
                await connection.OpenAsync();

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

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    FillUserParameters(command, user);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(User user)
        {
            using (NpgsqlConnection connection = _dbContext.CreateConnection())
            {
                await connection.OpenAsync();

                const string sql = @"delete from users where id = @id;";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("id", user.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

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
