using Identity.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.EF.Configs;

public class TokenConfig : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("Tokens").HasKey(item => item.Id);
        
        builder
            .HasOne(item => item.User)
            .WithMany(item => item.Tokens)
            .HasForeignKey(item => item.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}