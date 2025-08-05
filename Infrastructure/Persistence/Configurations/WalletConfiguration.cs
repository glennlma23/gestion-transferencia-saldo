using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.DocumentId)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(w => w.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(w => w.Balance)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(w => w.CreatedAt)
               .IsRequired();

        builder.Property(w => w.UpdatedAt)
               .IsRequired();
    }
}