using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.Aggregates.Users.Enums;
using Identity.Domain.IServices;
using Identity.Infrastructure.Exceptions;
using Identity.Infrastructure.MtuBus;
using Identity.Infrastructure.Options;
using Identity.Infrastructure.Utils;
using MediatR;
using ApplicationException = Identity.Infrastructure.Exceptions.ApplicationException;

namespace Identity.Api.Application.Commands.Users;

public class CheckPasswordSignInCommandHandler : IRequestHandler<CheckPasswordSignInCommand, bool>
{
    private readonly IUserStore userStore;
    private readonly IPasswordService passwordService;
    private readonly AppOptions.Jwt jwtOptions;
    private readonly IAppRandoms randoms;
    private readonly IDomainEventDispatcher eventHandler;

    public CheckPasswordSignInCommandHandler(
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
    
    public async Task<bool> Handle(CheckPasswordSignInCommand request, CancellationToken cancellationToken)
    {
        var user = await userStore.GetByUserNameAsync(request.UserName);
        if (user is null)
        {
            throw new ApplicationException.NotFound(AppMessages.UserNotFound);
        }
        
        if (user.Status != UserStatus.Active)
        {
            //TODO: throw proper message
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

        return true;
    }

    // private async Task<AuthViewModel.GetTokenOutput> GenerateTokenAsync(User user)
    // {
    //     var roles = await eventHandler.SendMediator(new GetUserRolesQuery(user.Id));
    //     var roleClaims = roles.Select(item => new Claim("roles", item.Name));
    //
    //     var claims = new[]
    //         {
    //             new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
    //             new Claim(JwtRegisteredClaimNames.Jti, user.ToString()),
    //             new Claim(JwtRegisteredClaimNames.Email, user.Email),
    //             new Claim("uid", user.Id.ToString())
    //         }
    //         // .Union(userClaims)
    //         .Union(roleClaims);
    //
    //     var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
    //     var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    //     var expire = DateTime.UtcNow.AddMinutes(jwtOptions.DurationInMinutes);
    //     var jwtSecurityToken = new JwtSecurityToken(
    //         issuer: jwtOptions.Issuer,
    //         audience: jwtOptions.Audience,
    //         claims: claims,
    //         expires: expire,
    //         signingCredentials: signingCredentials);
    //
    //     var token = new AuthViewModel.GetTokenOutput
    //     {
    //         AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
    //         ExpireDate = expire,
    //         RefreshToken = randoms.GetRandom(lenght: null)
    //     };
    //
    //     return token;
    // }
}