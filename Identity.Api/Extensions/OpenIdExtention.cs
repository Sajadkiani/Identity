using System.Reflection;
using Identity.Infrastructure.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;
using Quartz;

namespace Identity.Api.Extensions;

public static class OpenIdExtension
{
    public static void AddDbContextAndOpenIdDict(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    options => options.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext))!.GetName().Name));

                opt.UseOpenIddict();
            }
        );

        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>();

                // Enable Quartz.NET integration.
                options.UseQuartz();
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the token endpoint.
                options.SetTokenEndpointUris("connect/token");

                // Enable the password and the refresh token flows.
                options.AllowPasswordFlow()
                    .AllowRefreshTokenFlow();

                // Accept anonymous clients (i.e clients that don't send a client_id).
                options.AcceptAnonymousClients();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough();
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });
        
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }
}