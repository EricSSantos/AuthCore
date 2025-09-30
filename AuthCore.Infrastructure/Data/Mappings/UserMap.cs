using AuthCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCore.Infrastructure.Data.Mappings
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

            builder.Property(u => u.Atictive)
                   .HasColumnName("activate")
                   .IsRequired();

            builder.Property(u => u.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(u => u.InactivatedAt)
                   .HasColumnName("inactivated_at");
        }
    }
}
