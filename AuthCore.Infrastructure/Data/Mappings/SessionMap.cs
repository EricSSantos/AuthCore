using AuthCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCore.Infrastructure.Data.Mappings
{
    internal class SessionMap : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("sessions");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                   .HasColumnName("id");

            builder.HasOne(s => s.User)
                   .WithOne()
                   .HasForeignKey<Session>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.RefreshToken)
                   .HasColumnName("refresh_token")
                   .IsRequired();

            builder.Property(u => u.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(u => u.ExpiresAt)
                   .HasColumnName("expires_at")
                   .IsRequired();

            builder.Property(u => u.RevokedAt)
                   .HasColumnName("revoked_at");
        }
    }
}
