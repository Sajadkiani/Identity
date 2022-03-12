using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderService.Data;
using MassTransit;
using OrderService.Consumers.Orders;
using OrderService.Constants;

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
            // services.AddGenericRequestClient();
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<OrderConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        cfg.Host("rabbitmq://localhost", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                        // cfg.ConfigureEndpoints(context);
                    });
                    cfg.ReceiveEndpoint(QueueNames.payment_created, e =>
                    {
                        e.ConfigureConsumer<OrderConsumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();
            services.AddDbContext<OrderDbContext>(opt => opt.UseInMemoryDatabase("MyOrderDb"));
            services.AddAutoMapper(typeof(Startup));

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
}
