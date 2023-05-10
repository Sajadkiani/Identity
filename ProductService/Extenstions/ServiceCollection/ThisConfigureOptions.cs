namespace ProductService.Extenstions.ServiceCollection;

public static class ThisConfigureOptions
{
    public static void ConfigureOptions(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        
        // services.Configure<MasstransitOptions>(configurationManager.GetSection("masstransit"));
    }
}