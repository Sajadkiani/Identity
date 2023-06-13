using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Identity.Api.Application.Queries.Users;
using Identity.Api.Infrastructure.Brokers;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using IdentityService.Options;
using IdentityService.Utils;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Infrastructure.Services;

public class JwtTokenGeneratorService : ITokenGeneratorService
{
    private readonly AppOptions.Jwt jwtOptions;
    private readonly IAppRandoms randoms;
    private readonly IEventHandler eventHandler;

    public JwtTokenGeneratorService(
        AppOptions.Jwt jwtOptions,
        IAppRandoms randoms,
        IEventHandler eventHandler
        )
    {
        this.jwtOptions = jwtOptions;
        this.randoms = randoms;
        this.eventHandler = eventHandler;
    }

    public async Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user)
    {
        var roles = await eventHandler.SendMediator(new GetUserRolesQuery(user.Id));
        var roleClaims = roles.Select(item => new Claim("roles", item.Name));

        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            }
            // .Union(userClaims)
            .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var expire = DateTime.UtcNow.AddMinutes(jwtOptions.DurationInMinutes);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: expire,
            signingCredentials: signingCredentials);

        var token = new AuthViewModel.GetTokenOutput
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            ExpireDate = expire,
            RefreshToken = randoms.GetRandom(lenght: null)
        };

        return token;
    }
}