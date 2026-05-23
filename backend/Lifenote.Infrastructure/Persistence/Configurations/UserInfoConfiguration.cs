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

        // Indexes matching the actual DB schema
        builder.HasIndex(x => x.FirebaseUid, "idx_userinfo_firebaseuid").IsUnique();
        builder.HasIndex(x => x.Email, "idx_userinfo_email").IsUnique();
        builder.HasIndex(x => x.Username, "UserInfo_Username_key").IsUnique();
        builder.HasIndex(x => x.IsActive, "idx_userinfo_active");

        // FirebaseUid is stored in the DB column named "AuthProviderId" (legacy column name)
        builder.Property(x => x.FirebaseUid).HasColumnName("AuthProviderId").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Username).HasMaxLength(50);
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
        builder.Property(x => x.Bio).HasMaxLength(500);
        builder.Property(x => x.ProfilePicture);
        builder.Property(x => x.Theme).HasMaxLength(20).HasDefaultValueSql("'light'::character varying");
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.LastLoginAt);
        builder.Property(x => x.DeletedAt);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        // These are C# alias properties (get/set wrappers) — no backing DB column.
        // Ignoring them prevents EF Core from generating SELECT/INSERT for them.
        builder.Ignore(x => x.DisplayName);          // not a DB column — compute from FirstName+LastName in app layer
        builder.Ignore(x => x.AuthProviderId);       // alias for FirebaseUid — mapped via HasColumnName above
        builder.Ignore(x => x.ProfilePictureUrl);    // alias for ProfilePicture
    }
}
