using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderService.Data;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
namespace OrderService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGenericRequestClient();
            services.AddAutoMapper(typeof(Startup));
            services.AddDbContext<OrderDbContext>(opt => opt.UseInMemoryDatabase("MyOrderDb"));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq();
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
                  {
                      c.SwaggerDoc("v1", new OpenApiInfo { Title = "Productcatalog", Version = "v1" });
                  });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "orderService v1"));
        }
    }

    public static class CustomConfigurationExtensions
    {
        // /// <summary>
        // /// Should be using on every UsingRabbitMq configuration
        // /// </summary>
        // /// <param name="configurator"></param>
        // public static void ApplyCustomBusConfiguration(this IBusFactoryConfigurator configurator)
        // {
        //     var entityNameFormatter = configurator.MessageTopology.EntityNameFormatter;

        //     configurator.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter(entityNameFormatter));
        // }

        /// <summary>
        /// Should be used on every AddMassTransit configuration
        /// </summary>
        /// <param name="configurator"></param>
        //public static void ApplyCustomMassTransitConfiguration(this IBusRegistrationConfigurator configurator)
        //{
        //    configurator.SetEndpointNameFormatter(new CustomEndpointNameFormatter());
        //}
    }

    //public class CustomEntityNameFormatter :
    //    IEntityNameFormatter
    //{
    //    readonly IEntityNameFormatter _entityNameFormatter;

    //    public CustomEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
    //    {
    //        _entityNameFormatter = entityNameFormatter;
    //    }

    //    public string FormatEntityName<T>()
    //    {
    //        if (typeof(T).ClosesType(typeof(Get<>), out Type[] types))
    //        {
    //            var name = (string)typeof(IEntityNameFormatter)
    //                .GetMethod("FormatEntityName")
    //                .MakeGenericMethod(types)
    //                .Invoke(_entityNameFormatter, Array.Empty<object>());

    //            var suffix = typeof(T).Name.Split('`').First();

    //            return $"{name}-{suffix}";
    //        }

    //        return _entityNameFormatter.FormatEntityName<T>();
    //    }
    //}

    // public class CustomEndpointNameFormatter :
    //    IEndpointNameFormatter
    //{
    //    readonly IEndpointNameFormatter _formatter;

    //    public CustomEndpointNameFormatter()
    //    {
    //        _formatter = KebabCaseEndpointNameFormatter.Instance;
    //    }

    //    public string TemporaryEndpoint(string tag)
    //    {
    //        return _formatter.TemporaryEndpoint(tag);
    //    }

    //    public string Consumer<T>()
    //        where T : class, IConsumer
    //    {
    //        return _formatter.Consumer<T>();
    //    }

    //    public string Message<T>()
    //        where T : class
    //    {
    //        return _formatter.Message<T>();
    //    }

    //    public string Saga<T>()
    //        where T : class, ISaga
    //    {
    //        return _formatter.Saga<T>();
    //    }

    //    public string ExecuteActivity<T, TArguments>()
    //        where T : class, IExecuteActivity<TArguments>
    //        where TArguments : class
    //    {
    //        var executeActivity = _formatter.ExecuteActivity<T, TArguments>();

    //        return SanitizeName(executeActivity);
    //    }

    //    public string CompensateActivity<T, TLog>()
    //        where T : class, ICompensateActivity<TLog>
    //        where TLog : class
    //    {
    //        var compensateActivity = _formatter.CompensateActivity<T, TLog>();

    //        return SanitizeName(compensateActivity);
    //    }

    //    public string SanitizeName(string name)
    //    {
    //        return _formatter.SanitizeName(name);
    //    }
    //}
}
