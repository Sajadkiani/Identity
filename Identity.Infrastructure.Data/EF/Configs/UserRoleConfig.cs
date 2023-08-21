using Identity.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.EF.Configs;

public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles").HasKey(item => item.Id);
        
        builder
            .HasOne(item => item.User)
            .WithMany(item => item.UserRoles)
            .HasForeignKey(item => item.UserId).OnDelete(DeleteBehavior.Restrict);
        
        
        builder
            .HasOne(item => item.Role)
            .WithMany(item => item.UserRoles)
            .HasForeignKey(item => item.RoleId).OnDelete(DeleteBehavior.Restrict);
    }
}