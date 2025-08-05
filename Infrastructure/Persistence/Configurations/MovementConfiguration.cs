using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.ToTable("Movements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Amount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(m => m.Type)
               .IsRequired();

        builder.Property(m => m.CreatedAt)
               .IsRequired();

        builder.HasOne(m => m.Wallet)
               .WithMany(w => w.Movements)
               .HasForeignKey(m => m.WalletId);
    }
}