using Identity.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.EF.Configs;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles").HasKey(item => item.Id);
        
        builder
            .HasMany(item => item.UserRoles)
            .WithOne(item => item.Role)
            .HasForeignKey(item => item.RoleId).OnDelete(DeleteBehavior.Restrict);
    }
}