using AuthCore.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCore.Infrastructure.Persistence.Database.Mappings
{
    internal class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .HasColumnName("id");

            builder.Property(u => u.FirstName)
                   .HasColumnName("first_name")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(u => u.LastName)
                   .HasColumnName("last_name")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(u => u.Email)
                   .HasColumnName("email")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(u => u.Password)
                   .HasColumnName("password")
                   .IsRequired();

            builder.Property(u => u.Role)
                   .HasConversion<int>()
                   .HasColumnName("role")
                   .IsRequired();

            builder.Property(u => u.Active)
                   .HasColumnName("activate")
                   .IsRequired();

            builder.Property(u => u.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(u => u.InactivatedAt)
                   .HasColumnName("inactivated_at");

            builder.OwnsOne(u => u.LoginAttempts, vo =>
            {
                vo.Property(p => p.FailedAttempts)
                  .HasColumnName("failed_attempts");

                vo.Property(p => p.LastFailedAt)
                  .HasColumnName("last_failed_at");

                vo.Property(p => p.LockedUntil)
                  .HasColumnName("locked_until");

                vo.WithOwner();
            });
        }
    }
}
