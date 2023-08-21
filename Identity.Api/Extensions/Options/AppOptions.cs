using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Identity.Infrastructure.Extensions.Options;

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
}