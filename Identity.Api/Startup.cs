using System;
using System.Reflection;
using System.Text;
using Identity.Api.Extensions;
using Identity.Api.Grpc;
using Identity.Api.Security;
using Identity.Infrastructure.Clients.Grpc;
using Identity.Infrastructure.EF;
using Identity.Infrastructure.Extensions.Options;
using Identity.Infrastructure.ORM.EF;
using IntegrationEventLogEF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Serilog;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace Identity.Api
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            services.AddDbContextAndOpenIdDict(configuration);
            services.AddAppProblemDetail(environment);

            

            
            // services.AddAuthentication(opt =>
            // {
            //     opt.DefaultAuthenticateScheme = AppOptions.Jwt.Scheme;
            //     opt.DefaultScheme = AppOptions.Jwt.Scheme;
            //     opt.DefaultChallengeScheme = AppOptions.Jwt.Scheme;
            // }).AddScheme<AppOptions.Jwt, AppAuthenticationHandler>(AppOptions.Jwt.Scheme, opt =>
            // {
            //     opt.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateIssuer = true,
            //         ValidateAudience = true,
            //         ValidateLifetime = true,
            //         ValidateIssuerSigningKey = true,
            //         ValidIssuer = configuration["Jwt:Issuer"],
            //         ValidAudience = configuration["Jwt:Issuer"],
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            //     };
            // });
            
            services.AddServiceDiscovery(o => o.UseEureka());
            services.AddAppDependencies(configuration);
            services.AddAppOptions(configuration);
            services.AddMemoryCache();
            services.AddGrpc(opt =>
            {
                opt.Interceptors.Add<ExceptionInterceptor>();
            });
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddAppSwagger();
        }

        public static void Configure(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseAppSwagger();
        

            app.UseAppProblemDetail();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGrpcService<AuthGrpcService>();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}