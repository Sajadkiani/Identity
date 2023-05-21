using IdentityService.Entities;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.EntityConfigs;

public class TokenConfig : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        // // builder.HasKey(item => item.Id);
        //
        // builder
        //     .HasOne(item => item.User)
        //     .WithMany(item => item.Tokens)
        //     .HasForeignKey(item => item.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}