using Common.Services.Brokers;

namespace ProductService.Extenstions.ServiceCollection;

public static class ThisInjection
{
    public static void ConfigureInjections(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBroker, MasstransitBroker>();
    }
}