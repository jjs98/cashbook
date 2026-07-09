using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configs;

public class RolesConfig : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);
        builder.Property(role => role.Id).UseIdentityColumn();

        builder.HasIndex(role => role.Name).IsUnique();

        builder.Property(role => role.Name).IsRequired();
    }
}
