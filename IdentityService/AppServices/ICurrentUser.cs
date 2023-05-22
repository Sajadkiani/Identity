using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Core;
using IdentityService.Consts;
using IdentityService.Exceptions;

namespace IdentityService.Services;

public class CurrentUser : ICurrentUser
{
    private ClaimsPrincipal ClaimsPrincipal { get; set; }

    public void Set(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsPrincipal = claimsPrincipal;
    }

    public int UserId
    {
        get
        {
            var usrId = ClaimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimKeys.UserId)?.Value;
            if (string.IsNullOrWhiteSpace(usrId))
                throw new IdentityException.IdentityUnauthorizedException();
            
            return Convert.ToInt32(usrId);
        }
    }
}

public interface ICurrentUser
{
    void Set(ClaimsPrincipal claimsPrincipal);
    int UserId { get; }
}