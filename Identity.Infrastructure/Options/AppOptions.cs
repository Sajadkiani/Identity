using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Identity.Api.Extensions.Options;

public class AppOptions
{
    private class AppScheme { }

    public class Jwt : JwtBearerOptions 
    {
        public static string Scheme = nameof(AppScheme);
        public static string Section = "Jwt";
        
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int DurationInMinutes { get; set; }
        public int DurationInMinutesRefresh { get; set; }
    }

    public static class ApplicationOptionContext
    {
        public static string ConnectionString { get; set; }
    }
    
    public abstract class MTUBusOptions
    {
    }
    
    public class MTuRabbitMqOptions : MTUBusOptions
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public int Port { get; set; } = 5672;
    }
}