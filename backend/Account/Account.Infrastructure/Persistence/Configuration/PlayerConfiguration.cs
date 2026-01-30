using Account.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configuration;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    //TODO: Create configuration for Player entity
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players", "account");

    }
}
