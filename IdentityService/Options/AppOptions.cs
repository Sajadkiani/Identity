namespace IdentityService.Options;

public class AppOptions
{
    public class Jwt
    {
        public static string Section = "Jwt";
        
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int DurationInMinutes { get; set; }
    }
}