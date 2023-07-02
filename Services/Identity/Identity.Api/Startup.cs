using System;
using System.Reflection;
using System.Text;
using Identity.Api.Infrastructure.Extensions;
using Identity.Api.Infrastructure.Options;
using Identity.Api.Infrastructure.Security;
using Identity.Infrastructure.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            AppOptions.ApplicationOptionContext.ConnectionString =
                configuration.GetConnectionString("DefaultConnection");
            
            Console.WriteLine("connectionStrigng =============================" 
                              + configuration.GetConnectionString("DefaultConnection"));
            
            services.AddDbContext<AppDbContext>(opt => 
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    options => options.MigrationsAssembly(Assembly.GetAssembly(typeof(Program))!.GetName().Name))
            );
            
            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration["DefaultConnection"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            services.AddAppProblemDetail(environment);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = AppOptions.Jwt.Scheme;
                opt.DefaultScheme = AppOptions.Jwt.Scheme;
                opt.DefaultChallengeScheme = AppOptions.Jwt.Scheme;
            }).AddScheme<AppOptions.Jwt, AppAuthenticationHandler>(AppOptions.Jwt.Scheme, opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });
            
            services.AddAppDependencies(configuration);
            services.AddAppOptions(configuration);
            services.AddMemoryCache();
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddAppSwagger();
        }

        public static void Configure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //TODO: swagger config must run in develop env, then put these inside of  
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.Api v1"));

            app.UseAppProblemDetail();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}