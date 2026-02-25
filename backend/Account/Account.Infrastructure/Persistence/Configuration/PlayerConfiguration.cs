using Account.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configuration;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("players", "account");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.Elo)
            .IsRequired()
            .HasDefaultValue(1000);

        builder.Property(p => p.GamesIds)
            .HasColumnType("uuid[]")
            .IsRequired();

        builder.Property(p => p.TournamentsIds)
            .HasColumnType("uuid[]")
            .IsRequired();

        builder.Property(p => p.FriendsIds)
            .HasColumnType("uuid[]")
            .IsRequired();
    }
}
