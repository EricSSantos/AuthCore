using AuthCore.Domain.Aggregates.ConfirmCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.EFCore.Mappings
{
    /// <summary>Representa mapeamento EF Core do código de confirmação.</summary>
    internal sealed class ConfirmCodeMap : IEntityTypeConfiguration<ConfirmCode>
    {
        /// <summary>Operação para configurar o mapeamento EF Core do código de confirmação.</summary>
        /// <param name="builder">Construtor de configuração da entidade.</param>
        public void Configure(EntityTypeBuilder<ConfirmCode> builder)
        {
            builder.ToTable("confirm_codes");

            #region Primary Key

            builder.Property<Guid>("UserId")
                   .HasColumnName("user_id");

            builder.HasKey("UserId", nameof(ConfirmCode.Type));

            #endregion

            #region Properties

            builder.Property(c => c.Type)
                   .HasConversion<int>()
                   .HasColumnName("type")
                   .IsRequired();

            builder.Property(c => c.Code)
                   .HasColumnName("code")
                   .IsRequired();

            builder.Property(c => c.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(c => c.ExpiresAt)
                   .HasColumnName("expires_at")
                   .IsRequired();

            builder.Property(c => c.Attempts)
                   .HasColumnName("attempts")
                   .IsRequired();

            builder.HasIndex(c => c.ExpiresAt)
                   .HasDatabaseName("ix_confirm_codes_expires_at");

            #endregion
        }
    }
}
