using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("UserInfo");
        builder.HasKey(x => x.Id).HasName("UserInfo_pkey");
        builder.HasIndex(x => x.FirebaseUid, "idx_userinfo_firebaseuid").IsUnique();
        builder.HasIndex(x => x.Email, "idx_userinfo_email").IsUnique();
        builder.HasIndex(x => x.Username, "UserInfo_Username_key").IsUnique();
        builder.Property(x => x.FirebaseUid).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Username).HasMaxLength(50);
        builder.Property(x => x.Theme).HasMaxLength(20).HasDefaultValueSql("'light'::character varying");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
