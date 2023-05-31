using Identity.Domain.SeedWork;

namespace Identity.Domain.Aggregates.Users;

public class Token : Entity
{
    public Token(string accessToken, string refreshToken, DateTime expireDate)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpireDate = expireDate;
    }

    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime ExpireDate { get; private set; }
    
    public User User { get; private set; }
    public int UserId { get; private set; }
}