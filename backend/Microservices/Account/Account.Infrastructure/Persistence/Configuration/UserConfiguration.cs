using Account.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Users", "account");

        builder.HasMany(p => p.Friends)
        .WithMany()
        .UsingEntity(j => j.ToTable("PlayerFriends"));

        builder.Property(p => p.Email)
          .IsRequired()
          .HasMaxLength(150);

        builder.Property(p => p.Username)
          .IsRequired()
          .HasMaxLength(150);
    }
}
