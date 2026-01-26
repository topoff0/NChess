using Account.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Infrastructure.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "account");

        builder.Property(p => p.Email)
          .IsRequired()
          .HasMaxLength(150);

        builder.Property(p => p.Username)
          .IsRequired()
          .HasMaxLength(150);
    }
}
