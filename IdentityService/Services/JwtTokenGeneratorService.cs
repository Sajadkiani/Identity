using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using IdentityService.Consts;
using IdentityService.Entities;
using IdentityService.Options;
using IdentityService.Utils;
using IdentityService.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Services;

public class JwtTokenGeneratorService : ITokenGeneratorService
{
    private readonly IUserService userService;
    private readonly AppOptions.Jwt jwtOptions;
    private readonly IAppRandoms randoms;

    public JwtTokenGeneratorService(
        IUserService userService,
        AppOptions.Jwt jwtOptions,
        IAppRandoms randoms 
        )
    {
        this.userService = userService;
        this.jwtOptions = jwtOptions;
        this.randoms = randoms;
    }

    public async Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user)
    {
        var roles = await userService.GetUserRolesNamesAsync(user);
        var roleClaims = roles.Select(item => new Claim("roles", item));

        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
            ExpireDate = expire.Date,
            RefreshToken = randoms.GetRandom(lenght: null),
            Reference = randoms.GetRandom(lenght: null)
        };

        return token;
    }
}