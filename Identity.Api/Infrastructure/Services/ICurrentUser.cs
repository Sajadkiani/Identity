using System;
using System.Linq;
using System.Security.Claims;
using Identity.Api.Infrastructure.Consts;

namespace Identity.Api.Infrastructure.Services;

public class CurrentUser : ICurrentUser
{
    private ClaimsPrincipal ClaimsPrincipal { get; set; }
    private string userId;
    public void Set(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsPrincipal = claimsPrincipal;
        userId = ClaimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimKeys.UserId)?.Value;
    }

    public Guid UserId
    {
        get => Guid.Parse(userId);
        private set => UserId = Guid.Parse(userId);
    }
}

public interface ICurrentUser
{
    void Set(ClaimsPrincipal claimsPrincipal);
    Guid UserId { get; }
}