using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(u => u.CreatedAt)
               .IsRequired();

        builder.Property(u => u.DocumentId)
               .HasMaxLength(50)
               .IsRequired();
    }
}