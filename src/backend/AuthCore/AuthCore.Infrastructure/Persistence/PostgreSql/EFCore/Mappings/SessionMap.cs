using AuthCore.Domain.Aggregates.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.EFCore.Mappings
{
    /// <summary>Representa mapeamento EF Core da sessão.</summary>
    internal sealed class SessionMap : IEntityTypeConfiguration<Session>
    {
        /// <summary>Operação para configurar o mapeamento EF Core da sessão.</summary>
        /// <param name="builder">Construtor de configuração da entidade.</param>
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("sessions");

            #region Primary Key

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .HasConversion(
                        id => id.Value,
                        value => SessionId.Create(value))
                   .HasColumnName("id")
                   .HasMaxLength(128);

            #endregion

            #region Properties

            builder.Property(s => s.UserId)
                   .HasColumnName("user_id")
                   .IsRequired();

            builder.Property(s => s.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(s => s.ExpiresAt)
                   .HasColumnName("expires_at")
                   .IsRequired();

            builder.Property(s => s.MaxLifetime)
                   .HasColumnName("max_lifetime")
                   .IsRequired();

            builder.Property(s => s.RevokedAt)
                   .HasColumnName("revoked_at");

            builder.HasIndex(s => new { s.UserId, s.CreatedAt })
                   .HasDatabaseName("ix_sessions_user_id_created_at");

            builder.HasIndex(s => s.ExpiresAt)
                   .HasDatabaseName("ix_sessions_expires_at");

            builder.HasIndex(s => new { s.UserId, s.RevokedAt })
                   .HasDatabaseName("ix_sessions_user_id_revoked_at");

            #endregion

            #region Value Objects

            builder.OwnsOne(s => s.DeviceInfo, vo =>
            {
                vo.Property(p => p.Ip)
                  .HasColumnName("ip")
                  .HasMaxLength(64)
                  .IsRequired();

                vo.Property(p => p.Platform)
                  .HasColumnName("platform")
                  .HasMaxLength(128)
                  .IsRequired();

                vo.Property(p => p.Browser)
                  .HasColumnName("browser")
                  .HasMaxLength(128)
                  .IsRequired();

                vo.WithOwner();
            });

            builder.Navigation(s => s.DeviceInfo)
                   .IsRequired();

            #endregion
        }
    }
}
