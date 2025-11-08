using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Identity.Api.Application.Queries.Users;
using Identity.Api.Extensions.Options;
using Identity.Api.ViewModels;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.Aggregates.Users.Enums;
using Identity.Domain.IServices;
using Identity.Infrastructure.Exceptions;
using Identity.Infrastructure.MtuBus;
using Identity.Infrastructure.Utils;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ApplicationException = Identity.Infrastructure.Exceptions.ApplicationException;

namespace Identity.Api.Application.Commands.Users;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthViewModel.GetTokenOutput>
{
    private readonly IUserStore userStore;
    private readonly IPasswordService passwordService;
    private readonly AppOptions.Jwt jwtOptions;
    private readonly IAppRandoms randoms;
    private readonly IDomainEventDispatcher eventHandler;

    public LoginCommandHandler(
        IUserStore userStore,
        IPasswordService passwordService,
        AppOptions.Jwt jwtOptions,
        IAppRandoms randoms,
        IDomainEventDispatcher eventHandler
    )
    {
        this.userStore = userStore;
        this.passwordService = passwordService;
        this.jwtOptions = jwtOptions;
        this.randoms = randoms;
        this.eventHandler = eventHandler;
    }
    
    public async Task<AuthViewModel.GetTokenOutput> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userStore.GetByUserNameAsync(request.UserName);
        if (user is null)
        {
            throw new ApplicationException.NotFound(AppMessages.UserNotFound);
        }
        
        if (user.Status != UserStatus.Active)
        {
            throw new ApplicationException.Unauthorized();
        }

        string incomePassword = request.Password;
        if (request.DoHashPassword)
        {
            incomePassword = passwordService.HashPassword(request.Password, request.UserName);
        }
        
        if (user.Password != incomePassword)
        {
            throw new ApplicationException.Unauthorized();
        }

        var token = await GenerateTokenAsync(user);
        
        user.AddTokens(token.AccessToken, token.RefreshToken, token.ExpireDate);
        
        //TODO: check for add token in database

        return token;
    }

    private async Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user)
    {
        var roles = await eventHandler.SendAsync(new GetUserRolesQuery(user.Id));
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