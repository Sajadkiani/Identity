using MassTransit;
using MassTransit.MultiBus;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.Database;
using ProductService.Extenstions.ServiceCollection;
using ProductService.Models.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var configuration = builder.Configuration;

//options
builder.Services.ConfigureOptions(configuration);

builder.Services.ConfigureInjections();

builder.Services.AddDbContext<ProductContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("Default"))
    );

builder.Services.AddMassTransit(x =>
{
    // var op = new MasstransitOptions();
    // var ff = configuration.GetSection("masstransit");
    var options = configuration.GetSection("masstransit").Get<MasstransitOptions>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
        config.Host(new Uri(options.RootUrl), configure =>
        {
            configure.Password(options.Password);
            configure.Username(options.UserName);
        })));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
