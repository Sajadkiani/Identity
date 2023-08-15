using System;
using Identity.Api.Infrastructure.Extensions.Options;
using IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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