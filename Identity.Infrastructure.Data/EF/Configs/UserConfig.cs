using Identity.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.EF.Configs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users").HasKey(item => item.Id);
        
        builder
            .HasMany(item => item.Tokens)
            .WithOne(item => item.User)
            .HasForeignKey(item => item.UserId).OnDelete(DeleteBehavior.Cascade);

        //EF access the OrderItem collection property through its backing field
        var nav = builder.Metadata.FindNavigation(nameof(User.Tokens));
        nav!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}