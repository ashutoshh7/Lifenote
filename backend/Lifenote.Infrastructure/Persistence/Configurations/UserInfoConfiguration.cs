using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirebaseUid).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.FirebaseUid).IsUnique();
        builder.Property(x => x.Email).HasMaxLength(320).IsRequired();
        builder.Property(x => x.Username).HasMaxLength(50);
        builder.HasIndex(x => x.Username).IsUnique();
        builder.Property(x => x.Theme).HasMaxLength(50);
    }
}
