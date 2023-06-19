using System;
using IdentityService.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.Extensions.Configuration;

namespace Identity.Api.Infrastructure.IntegrationEventMigrations;

public class IntegrationEventLogContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
{
    public IntegrationEventLogContext CreateDbContext(string[] args)
    {
        Console.WriteLine("==============developer help================");
        Console.WriteLine(AppOptions.ApplicationOptionContext.ConnectionString);
        Console.WriteLine("=====================================");

        var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

        optionsBuilder.UseSqlServer(
            AppOptions.ApplicationOptionContext.ConnectionString,
            options =>
                options.MigrationsAssembly(GetType().Assembly.GetName().Name));

        return new IntegrationEventLogContext(optionsBuilder.Options);
    }
}